using System;
using System.Collections;
using System.Collections.Generic;
using Levels.Abilities.CommonImpacts;
using Levels.Core.Statuses;
using Levels.IntentsImpacts;
using UnityEngine;
using VContainer;

namespace Levels.Core
{
    [CreateAssetMenu(fileName = "StatusesRepository", menuName = "Core/Components/StatusesRepository", order = 1)]
    [Serializable]
    public class StatusesRepository : CoreObject, IModifyingImpacts
    {
        public event Action<IStatus> StatusApplied;
        public event Action<IStatus> StatusDisappeared;

        [SerializeReference]
        [SubclassSelector]
        private IStatus[] initialStatuses = {};

        IEnumerable<IModifierStatus> IModifyingImpacts.Modifiers => _modifiers;

        [Inject]
        private IImpactConsumer<ReceivedStatusImpact> _newStatuses;

        [Inject] private Entity _entity;
        [Inject] private IObjectResolver _resolver;

        private readonly HashSet<IModifierStatus> _modifiers = new();

        private List<Coroutine> _routines = new();

        public override void Init()
        {
            _newStatuses.Impacted += OnNewStatusImpact;

            foreach (var status in initialStatuses)
            {
                ApplyStatus(status);
            }
        }

        private void OnNewStatusImpact(ReceivedStatusImpact impact) => ApplyStatus(impact.Status);

        private void ApplyStatus(IStatus status)
        {
            _resolver.Inject(status);

            if (status is IModifierStatus modifier)
            {
                _modifiers.Add(modifier);
            }

            StatusApplied?.Invoke(status);
            _entity.Runner?.StartCoroutine(Run(status));
        }

        private IEnumerator Run(IStatus status)
        {
            yield return status.Run(_entity);

            if (status is IModifierStatus modifier)
            {
                _modifiers.Remove(modifier);
            }
            StatusDisappeared?.Invoke(status);
        }

        public override void Dispose()
        {
            _newStatuses.Impacted -= OnNewStatusImpact;

            foreach (var routine in _routines)
            {
                _entity.Runner?.StopCoroutine(routine);
            }
            _routines.Clear();
        }
    }
}