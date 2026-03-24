using System.Collections.Generic;
using Levels.Config;
using Levels.Core;
using Levels.Core.Statuses;
using UnityEngine;
using VContainer;

namespace Levels.Visual
{
    public class StatusesPlayer : MonoBehaviour
    {
        [Inject] private StatusesVfxsConfig _config;
        [Inject] private StatusesRepository _statuses;

        private readonly Dictionary<IStatus, GameObject> _playingStatuses = new();

        private void OnEnable()
        {
            _statuses.StatusApplied += OnStatusApplied;
            _statuses.StatusDisappeared += OnStatusDisappeared;
        }

        private void OnStatusApplied(IStatus status)
        {
            // TODO POOLING
            var vfx = Instantiate(_config.StatusVfxs[status.GetType()], transform);
            _playingStatuses[status] = vfx;
        }

        private void OnStatusDisappeared(IStatus status)
        {
            Destroy(_playingStatuses[status]);
            _playingStatuses.Remove(status);
        }

        private void OnDisable()
        {
            _statuses.StatusApplied -= OnStatusApplied;
            _statuses.StatusDisappeared -= OnStatusDisappeared;

            foreach (var (_, vfx) in _playingStatuses)
            {
                Destroy(vfx);
            }
            _playingStatuses.Clear();
        }
    }
}