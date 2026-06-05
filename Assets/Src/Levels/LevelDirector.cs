using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using Levels.Util;
using UnityEngine;
using VContainer;

namespace Levels
{
    public class LevelDirector : MonoBehaviour
    {
        public static float GameplayTime;

        public static event Action<(LevelActivityMask previous, LevelActivityMask current)> ActivityMaskChanged;

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

        [SerializeField] private LevelActivityMask initialMask; // TODO enum for steps: compiling shaders, loading maybe something else, only then gaming
        [SerializeField] private GameObject player;

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
            ActivityMask = initialMask;
        }

        private void Update()
        {
            GameplayTime += Time.deltaTime;
        }

        private void OnDisable()
        {
            _playerDied.Impacted -= OnDead;
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

        private void OnActivityMaskChanged((LevelActivityMask, LevelActivityMask) _)
        {
            if ((_activityMask & LevelActivityMask.Gameplay) != 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
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
    }
}