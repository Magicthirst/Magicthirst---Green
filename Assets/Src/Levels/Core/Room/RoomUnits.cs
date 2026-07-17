using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Abilities.KillAndDown;
using Levels.IntentsImpacts;
using VContainer;

namespace Levels.Core.Room
{
    public class RoomUnits
    {
        public event Action<DownedImpact> Downed;
        public event Action<TargetIsDeadImpact> Killed;
        public event Action Cleared; // in gaming meaning: "room is clear" = none left

        public bool IsCleared;

        public IEnumerable<Entity> Entities => _entities.Select(e => e.Entity);

        private readonly List<ManagedEntity> _entities = new();

        [Inject] private IntentsImpacts.IntentsImpacts _intentsImpacts;

        public RoomUnits()
        {
            Killed += _ =>
            {
                if (_entities.All(e => e.Entity.Dead))
                {
                    Cleared?.Invoke();
                }
            };
            Cleared += () => IsCleared = true;
        }

        public void Register(Entity entity)
        {
            var downed = _intentsImpacts.GetImpactConsumerFor<DownedImpact>(entity.Owner, null);
            var killed = _intentsImpacts.GetImpactConsumerFor<TargetIsDeadImpact>(entity.Owner, null);

            downed.Impacted += OnDowned;
            killed.Impacted += OnKilled;

            _entities.Add(new ManagedEntity
            {
                Entity = entity,
                Downed = downed,
                Killed = killed
            });
        }

        public void Clear()
        {
            foreach (var entity in _entities)
            {
                entity.Downed.Impacted -= OnDowned;
                entity.Downed.Dispose();
                entity.Killed.Impacted -= OnKilled;
                entity.Killed.Dispose();
            }

            _entities.Clear();
        }

        private void OnDowned(DownedImpact impact) => Downed?.Invoke(impact);
        
        private void OnKilled(TargetIsDeadImpact impact) => Killed?.Invoke(impact);

        private struct ManagedEntity
        {
            public Entity Entity;
            public IImpactConsumer<DownedImpact> Downed;
            public IImpactConsumer<TargetIsDeadImpact> Killed;
        }
    }
}