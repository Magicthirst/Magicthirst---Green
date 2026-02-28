using System;
using System.Collections.Generic;
using Levels.Abilities.HitScanShoot;
using Levels.Abilities.ParrySabre;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core.Passives
{
    [CreateAssetMenu(fileName = "ProjectilesParrying", menuName = "Core/Components/ProjectilesParrying", order = 1)]
    public class ProjectilesParrying : PassiveCoreObject
    {
        private const int BufferSize = 32;

        public event Action<IIntent> AttackPassed;
        public event Action<IIntent> AttackParried;

        public DeferredBroker<HitScanShootIntent> Handle => _handle;

        private float _ParryWindowStart => _lastParryTimePoint - _config.Leeway;
        private float _ParryWindowEnd => _lastParryTimePoint + _config.Duration;

        [Inject] private IParryConfig _config;
        private float _parryAngleRads;

        private float _lastParryTimePoint = float.MinValue;
        private Vector3 _lastParryDirection;

        private Queue<AttackInstance> _suspendedAttacks;

        private readonly DeferredBrokerHandle _handle;
        [Inject] private IImpactConsumer<ParryImpact> _consumer;

        public ProjectilesParrying()
        {
            _handle = new DeferredBrokerHandle(this);
        }

        public override void Init()
        {
            _parryAngleRads = (_config.AngleDegrees / 2) * Mathf.Deg2Rad;
            _lastParryTimePoint = float.MinValue;
            _suspendedAttacks = new Queue<AttackInstance>(BufferSize);
            _consumer.Impacted += Parry;
            _handle.Init();
        }

        public override void FixedUpdate() => CheckSuspendedAttacksStatus();

        public override void Dispose()
        {
            _suspendedAttacks.Clear();
            _consumer.Impacted -= Parry;
            _handle.Dispose();
        }

        private void Parry(ParryImpact impact)
        {
            Debug.Log($"Parried {impact}");
            _lastParryTimePoint = Time.fixedTime;
            _lastParryDirection = impact.Direction;

            CheckSuspendedAttacksStatus();
        }

        /// <summary>
        /// Record parryable attack attempt
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="incomingDirection">Direction relative to the victim</param>
        private void Attack(IIntent intent, Vector3 incomingDirection)
        {
            Debug.Log($"Attacked {intent}");
            var now = Time.fixedTime;

            _suspendedAttacks.Enqueue(new AttackInstance(intent, incomingDirection, now));
        }

        private void CheckSuspendedAttacksStatus()
        {
            var parryWindowStart = _ParryWindowStart;
            var parryWindowEnd = _ParryWindowEnd;

            while (_suspendedAttacks.TryPeek(out var attack) && attack.TimePoint < parryWindowStart)
            {
                Debug.Log($"Passed {attack.Intent}");
                AttackPassed?.Invoke(attack.Intent);
                _suspendedAttacks.Dequeue();
            }

            while (_suspendedAttacks.TryPeek(out var attack) && attack.TimePoint <= parryWindowEnd)
            {
                if (IsParryingByDirection(attack.IncomingDirection))
                {
                    Debug.Log($"Parried {attack.Intent}");
                    AttackParried?.Invoke(attack.Intent);
                }
                else
                {
                    Debug.Log($"Passed {attack.Intent}");
                    AttackPassed?.Invoke(attack.Intent);
                }

                _suspendedAttacks.Dequeue();
            }
        }

        private bool IsParryingByDirection(Vector3 incomingDirection)
        {
            var cos = Vector3.Dot(_lastParryDirection, incomingDirection);
            var angle = Mathf.Acos(cos);
            return angle < _parryAngleRads;
        }

        private class DeferredBrokerHandle : DeferredBroker<HitScanShootIntent>, IDisposable
        {
            private readonly ProjectilesParrying _parrying;

            public DeferredBrokerHandle(ProjectilesParrying parrying)
            {
                _parrying = parrying;
            }

            public void Init()
            {
                _parrying.AttackPassed += Pass;
                _parrying.AttackParried += Decline;
            }

            public override bool TryConsume(HitScanShootIntent intent)
            {
                if (intent.Caster != _parrying.Owner)
                {
                    _parrying.Attack(intent, -intent.Direction);
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
                _parrying.AttackPassed -= Pass;
                _parrying.AttackParried -= Decline;
            }
        }

        private readonly struct AttackInstance
        {
            public readonly IIntent Intent;
            public readonly Vector3 IncomingDirection;
            public readonly float TimePoint;

            public AttackInstance(IIntent intent, Vector3 incomingDirection, float timePoint)
            {
                Intent = intent;
                IncomingDirection = incomingDirection;
                TimePoint = timePoint;
            }
        }
    }

    public interface IParryConfig
    {
        float Leeway { get; }
        float Duration { get; }
        float AngleDegrees { get; }
    }
}