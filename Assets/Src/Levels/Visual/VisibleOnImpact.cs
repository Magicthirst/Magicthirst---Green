using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(MeshRenderer))]
    public class VisibleOnImpact : MonoBehaviour
    {
        [SerializeField] private float flashDurationSeconds;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField]
        private string impactType;
        private IImpactConsumer _consumer;

        [SerializeField] private List<Renderer> renderers;
        private int _visibilityRequestsCounter;
        private float[] _originalColorsAlpha;

        [Inject]
        private void Consumer(IObjectResolver resolver)
        {
            var tImpact = Type.GetType(impactType);
            var consumerType = typeof(IImpactConsumer<>).MakeGenericType(tImpact);
            _consumer = (IImpactConsumer) resolver.Resolve(consumerType);
        }

        private void Awake()
        {
            _originalColorsAlpha = renderers.Select(r => r.material.color.a).ToArray();
            // ReSharper disable once LocalVariableHidesMember : this property already deprecated and discouraged to use
            foreach (var renderer in renderers)
            {
                renderer.material.color = renderer.material.color.With(a: 0);
            }
        }

        private void OnEnable()
        {
            _consumer.Impacted += OnImpact;
        }

        private void OnDisable()
        {
            _consumer.Impacted -= OnImpact;
            _consumer.Dispose();
        }

        private void OnImpact() => StartCoroutine(VisibilityRoutine());

        private IEnumerator VisibilityRoutine()
        {
            _visibilityRequestsCounter++;
            for (var i = 0; i < renderers.Count; i++)
            {
                // ReSharper disable once LocalVariableHidesMember
                var renderer = renderers[i];
                renderer.material.color = renderer.material.color.With(a: _originalColorsAlpha[i]);
            }

            yield return new WaitForSeconds(flashDurationSeconds);
            
            if (_visibilityRequestsCounter == 1)
            {
                // ReSharper disable once LocalVariableHidesMember
                foreach (var renderer in renderers)
                {
                    renderer.material.color = renderer.material.color.With(a: 0);
                }
            }
            _visibilityRequestsCounter--;
        }
    }
}