using System;
using System.Collections.Generic;
using Levels.Abilities.CommonImpacts;
using Levels.Core.Statuses;
using Levels.IntentsImpacts;
using VContainer;

namespace Levels.Core
{
    using DisposeAction = Action;

    public class StatusesRepository : CoreObject, IModifyingImpacts
    {
        public event Action<IStatus> StatusApplied;
        public event Action<IStatus> StatusDisappeared;

        IEnumerable<IModifierStatus> IModifyingImpacts.Modifiers => _modifiers;

        [Inject]
        private IImpactConsumer<ReceivedStatusImpact> _newStatuses;

        [Inject] private Entity _entity;

        private readonly HashSet<IModifierStatus> _modifiers = new();

        private DisposeAction _dispose;

        public override void Init()
        {
            _newStatuses.Impacted += OnNewStatus;
        }

        private void OnNewStatus(ReceivedStatusImpact impact)
        {
            var status = impact.Status;

            if (status is IModifierStatus modifier)
            {
                _modifiers.Add(modifier);
            }

            StatusApplied?.Invoke(status);
            _dispose += status.RunIn(_entity) + (() => StatusDisappeared?.Invoke(status));
        }

        public override void Dispose()
        {
            _newStatuses.Impacted -= OnNewStatus;
            _dispose.Invoke();
            _dispose = null;
        }
    }
}