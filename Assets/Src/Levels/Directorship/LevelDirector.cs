using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using VContainer;
using static Levels.Directorship.LevelActivityMask;

namespace Levels.Directorship
{
    public delegate void LAMChangedCallback((LevelActivityMask previous, LevelActivityMask current) change);

    public sealed partial class LevelDirector : MonoBehaviour
    {
        public static bool IsStarted;

        public static float GameplayTime;
        public static float GameplayFixedTime;
        public static float GameplayDeltaTime => Time.deltaTime * GameplayTimeSpeed;
        public static float GameplayFixedDeltaTime => Time.fixedDeltaTime * GameplayTimeSpeed;
        public static float GameplayTimeSpeed = 1f;

        public static event Action FixedUpdated;
        public static event Action Updated;

        public static event LAMChangedCallback ActivityMaskChanged;

        public static ILifecycleInterruptions Interruptions => _interruptions ??= new LifecycleInterruptions(_instance);
        private static LifecycleInterruptions _interruptions;

        private static LevelActivityMask _activityMask;
        public static LevelActivityMask ActivityMask
        {
            get => _activityMask;
            set
            {
                var previous = _activityMask;
                _activityMask = value;
                _interruptions.UpdateWithMask(value);
                ActivityMaskChanged?.Invoke((previous, _activityMask));
            }
        }

        private static LevelDirector _instance;

        [SerializeReference]
        [SubclassSelector]
        private ILevelScenarioPlayer[] scenariosQueue;
        [SerializeField] private EditorLevelActivityMask initialMask;
        [SerializeField] private GameObject player;

        private Coroutine _scenariosRoutine;
        private IImpactConsumer<DownedImpact> _playerDied;
        private bool _playerIsDead = false;

        [Inject] private IGameNavigation _navigation;

        [Inject]
        public void Construct(IntentsImpacts.IntentsImpacts intentsImpacts)
        {
            _playerDied = intentsImpacts.GetImpactConsumerFor<DownedImpact>(player, null);
        }

        public LevelDirector()
        {
            _instance = this;
            ActivityMaskChanged += OnActivityMaskChanged;
        }

        private void OnEnable()
        {
            _playerDied.Impacted += OnDead;
        }

        private void Start()
        {
            GameplayTime = Time.time;
            ActivityMask = (LevelActivityMask)initialMask;
            IsStarted = true;
            _scenariosRoutine = StartCoroutine(scenariosQueue.Link());
        }

        private void Update()
        {
            if ((ActivityMask & (Gameplay | Tutorial)) != 0)
            {
                GameplayTime += Time.deltaTime * GameplayTimeSpeed;
            }
            Updated?.Invoke();
        }

        private void FixedUpdate()
        {
            if ((ActivityMask & (Gameplay | Tutorial)) != 0)
            {
                GameplayFixedTime += Time.fixedDeltaTime * GameplayTimeSpeed;
            }
            FixedUpdated?.Invoke();
        }

        private void OnDisable()
        {
            _playerDied.Impacted -= OnDead;
            if (_scenariosRoutine != null)
            {
                StopCoroutine(_scenariosRoutine);
            }
        }

        private void OnDead(DownedImpact _)
        {
            if (_playerIsDead)
            {
                return;
            }

            _playerIsDead = true;
            _navigation.FailLevel();
        }

        private void OnActivityMaskChanged((LevelActivityMask, LevelActivityMask mask) p)
        {
            if (p.mask == Pause)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private class LifecycleInterruptions : ILifecycleInterruptions
        {
            private readonly Dictionary<LevelActivityMask, InterruptionQueue> _interruptions = new();

            private readonly MonoBehaviour _owner;

            public LifecycleInterruptions(MonoBehaviour owner)
            {
                _owner = owner;
            }

            public InterruptionQueue this[LevelActivityMask mask]
            {
                get
                {
                    if (!_interruptions.TryGetValue(mask, out var queue))
                    {
                        _interruptions[mask] = queue = new InterruptionQueue(_owner, null);
                    }

                    return queue;
                }
            }

            public void UpdateWithMask(LevelActivityMask mask)
            {
                foreach (var key in _interruptions.Keys.Where(key => (mask & key) == 0))
                {
                    try
                    {
                        _interruptions[key].Interrupt(new WaitUntil(() => (key & ActivityMask) != 0));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e); // TODO hunt all whose somewhy cannot be interrupted
                    }
                }
            }
        }

        public interface ILifecycleInterruptions
        {
            public InterruptionQueue this[LevelActivityMask mask] { get; }
        }

        [Serializable]
        public class OnLevelScenario : ILevelScenarioPlayer
        {
            [Header("ILevelScenarioPlayer")]
            public MonoBehaviour scenario;

            public ILevelScenarioPlayer Scenario => (ILevelScenarioPlayer) scenario;

            public IEnumerator GetRoutine() => Scenario.GetRoutine();
        }
    }
}