using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.KillAndDown;
using UnityEngine;
using VContainer;

namespace Levels.Core.Room
{
    public class RoomHealing
    {
        public event Action HelpRequested;

        [Inject] private RoomUnits _roomUnits;

        private readonly HashSet<Entity> _healers = new();
        private readonly HashSet<Entity> _downedUnits = new();
        private readonly Dictionary<Entity, Entity> _downedToHealersMap = new();

        public void Init()
        {
            _roomUnits.Downed += OnUnitDowned;
            _roomUnits.Killed += OnUnitKilled;
        }

        public void RegisterHealer(Entity healer)
        {
            _healers.Add(healer);
        }

        public void UnregisterHealer(Entity healer)
        {
            _healers.Remove(healer);
            ReleaseHealerClaims(healer);
        }

        public IEnumerable<Entity> AttendDowned(Entity healer)
        {
            while (TryClaimDowned(healer, out var downed))
            {
                yield return downed;
                ReleaseClaim(healer, downed);
            }
        }

        public void ReleaseHealerClaims(Entity healer)
        {
            var unattendedDowned = _downedToHealersMap
                .Select(pair => (Downed: pair.Key, Healer: pair.Value))
                .Where(pair => pair.Healer == healer)
                .Select(pair => pair.Downed)
                .ToArray();

            foreach (var key in unattendedDowned)
            {
                _downedToHealersMap.Remove(key);
                HelpRequested?.Invoke();
            }
        }

        public void ResolveHeal(Entity downed)
        {
            _downedUnits.Remove(downed);
            _downedToHealersMap.Remove(downed);
        }

        public void Clear()
        {
            _roomUnits.Downed -= OnUnitDowned;
            _roomUnits.Killed -= OnUnitKilled;
        }

        public bool IsDowned(Entity entity) => _downedUnits.Contains(entity);

        private bool TryClaimDowned(Entity healer, out Entity target)
        {
            target = _downedUnits.FirstOrDefault(unit => unit != healer && _downedToHealersMap.TryAdd(unit, healer));
            return target is not null;
        }

        private void ReleaseClaim(Entity healer, Entity downed)
        {
            if (_downedToHealersMap.TryGetValue(downed, out var assignedHealer) && assignedHealer == healer)
            {
                _downedToHealersMap.Remove(downed);
            }
        }

        private void OnUnitDowned(DownedImpact impact)
        {
            var entity = _roomUnits.Entities.FirstOrDefault(e => e.Owner == impact.Target);
            if (entity is null)
            {
                Debug.LogError($"Entity not found for DownedImpact: {impact.Target}");
                return;
            }

            if (_downedUnits.Add(entity))
            {
                HelpRequested?.Invoke();
            }
        }

        private void OnUnitKilled(KilledImpact impact)
        {
            var entity = _downedUnits.FirstOrDefault(e => e.Owner == impact.Target);

            if (entity is null)
            {
                return;
            }

            _downedUnits.Remove(entity);
            _downedToHealersMap.Remove(entity);

            if (_healers.Contains(entity))
            {
                UnregisterHealer(entity);
            }
        }
    }
}