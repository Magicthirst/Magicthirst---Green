using System;
using System.Collections;
using JetBrains.Annotations;
using Levels.Abilities.CommonImpacts;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(TrailRenderer))]
    public class SabreTrailColors : MonoBehaviour
    {
        [SerializeField] private Gradient defaultColor;

        [SerializeField] private Gradient parriedColor;
        [SerializeField] private float parryColorDuration;

        private TrailRenderer _renderer;
        [CanBeNull] private Coroutine _effectRoutine = null;

        [Inject] private IImpactConsumer<CasterParriedEffect> _parries;
        private WaitForSeconds _parryWaiter;

        private void Awake()
        {
            _renderer = GetComponent<TrailRenderer>();
            _parryWaiter = new WaitForSeconds(parryColorDuration);
        }

        private void OnEnable()
        {
            _parries.Impacted += HandleParry;
        }

        private void Start()
        {
            _renderer.colorGradient = defaultColor;
        }

        private void HandleParry(CasterParriedEffect _)
        {
            if (_effectRoutine != null)
            {
                StopCoroutine(_effectRoutine);
                _effectRoutine = null;
            }

            _effectRoutine = StartCoroutine(Routine());
            return;

            IEnumerator Routine()
            {
                _renderer.colorGradient = parriedColor;
                yield return _parryWaiter;
                _renderer.colorGradient = defaultColor;
            }
        }

        private void OnDisable()
        {
            _parries.Impacted -= HandleParry;

            if (_effectRoutine != null)
            {
                StopCoroutine(_effectRoutine);
                _effectRoutine = null;
            }
        }
    }
}