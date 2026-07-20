using System;
using System.Collections;
using System.Collections.Generic;
using Levels.AI.Util;
using Levels.Directorship;
using Levels.Util;
using Levels.Util.MasksRegistry;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using static Levels.AI.Bandit.ScreeningSquadBrainUtil;

namespace Levels.AI.Bandit
{
    public partial class ScreenerSquadBrain : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay;

        [SerializeField] private PublishContacts startFightArea;
        [SerializeField] private PublishContacts stopFightArea;

        [SerializeField] private float memberRadius;
        [SerializeField] private float distanceFromEnemy;
        [SerializeField] private float tacticUpdatePeriod;
        [SerializeField] private float squadShotCooldown;
        [SerializeField] private float lookAheadTime;

        private InterruptionQueue _squadCooldown;
        private InterruptionQueue _SquadCooldown => _squadCooldown ??= new InterruptionQueue(this, null);

        private Enemy _enemy;
        private int _nextMemberI = 0;
        private readonly Dictionary<int, Vector3> _membersPositions = new();

        private WaitForSeconds _tacticUpdateWaiter;

        [Inject] private MasksRegistry _registry;
        private Transform _camera;

        [Inject]
        public void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _enemy = new Enemy { LookAheadTime = lookAheadTime };
        }

        protected override void DidEnabled()
        {
            startFightArea.ContactEntered += OnPlayerCameClose;
            stopFightArea.ContactExited += OnPlayerGotAway;
        }

        private void Start()
        {
            StartCoroutine(Run().WithInterruptions(_LevelLifecycle));
            return;

            IEnumerator Run()
            {
                while (true)
                {
                    yield return null;

                    while (_enemy.IsSet)
                    {
                        PlaceMembersAroundEnemy();
                        yield return InterruptableWait.ForSeconds(tacticUpdatePeriod);
                    }
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }

        protected override void DidDisabled()
        {
            startFightArea.ContactEntered -= OnPlayerCameClose;
            stopFightArea.ContactExited -= OnPlayerGotAway;
        }

        public (ScreenerSquadMemberMovement Movement, InterruptionQueue ShotsCooldown) RegisterMember(NavMeshAgent agent)
        {
            var id = _nextMemberI++;
            _membersPositions[id] = agent.transform.position;

            return
            (
                Movement: new ScreenerSquadMemberMovement
                (
                    id: id,
                    agent: agent,
                    membersPositions: _membersPositions,
                    tacticUpdatePeriod: tacticUpdatePeriod
                ),
                ShotsCooldown: _SquadCooldown
            );
        }

        public void NotifyShot()
        {
            _SquadCooldown.Interrupt(InterruptableWait.ForSeconds(squadShotCooldown));
        }

        private void OnPlayerCameClose(Collider other)
        {
            if (_registry.Is(other.gameObject, Mask.PlayerCharacter))
            {
                _enemy.SetTo(other.transform);
            }
        }

        private void OnPlayerGotAway(Collider other)
        {
            if (_enemy.Is(other))
            {
                _enemy.Unset();
            }
        }

        private void PlaceMembersAroundEnemy()
        {
            Span<Vector3> frontPositions = stackalloc Vector3[_membersPositions.Count];

            CalculateFrontline(in frontPositions, _camera, _enemy.EstimatedPosition, memberRadius, distanceFromEnemy);
            AssignFrontPlaces(in frontPositions, _membersPositions);
        }
    }
}