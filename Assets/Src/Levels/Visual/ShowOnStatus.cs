using System;
using Levels.Core;
using Levels.Core.Statuses;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(Renderer))]
    public class ShowOnStatus : MonoBehaviour
    {   
        [SubtypeProperty(typeof(IStatus))]
        [SerializeField] private string statusType;
        private Type _StatusType => _statusType ??= Type.GetType(statusType);
        private Type _statusType;

        private Renderer _renderer;

        [Inject] private StatusesRepository _statuses;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
        }

        private void OnEnable()
        {
            _statuses.StatusApplied += OnApplied;
            _statuses.StatusDisappeared += OnDisappeared;
        }

        private void OnApplied(IStatus status)
        {
            if (status.GetType() == _StatusType)
            {
                _renderer.enabled = true;
            }
        }

        private void OnDisappeared(IStatus status)
        {
            if (status.GetType() == _StatusType)
            {
                _renderer.enabled = false;
            }
        }

        private void OnDisable()
        {
            _statuses.StatusApplied -= OnApplied;
            _statuses.StatusDisappeared -= OnDisappeared;
        }
    }
}