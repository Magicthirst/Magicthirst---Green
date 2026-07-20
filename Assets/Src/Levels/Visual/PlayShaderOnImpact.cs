using System;
using Levels.Directorship;
using Levels.Extensions;
using Levels.IntentsImpacts;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    [RequireComponent(typeof(Renderer))]
    public class PlayShaderOnImpact : LevelBehaviour
    {
        private static readonly int StartTime = Shader.PropertyToID("_StartTime");

        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)activity;

        [SerializeField] private EditorLevelActivityMask activity;

        [SubtypeProperty(typeof(IImpact))]
        [SerializeField]
        private string impactType;

        private Renderer _renderer;
        private MaterialPropertyBlock _properties;

        private IImpactConsumer _consumer;

        [Inject]
        private void Consumer(IObjectResolver resolver)
        {
            var tImpact = Type.GetType(impactType);
            var consumerType = typeof(IImpactConsumer<>).MakeGenericType(tImpact);
            _consumer = (IImpactConsumer) resolver.Resolve(consumerType);
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _properties = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_properties);
        }

        protected override void DidEnabled() => _consumer.Impacted += Run;

        private void Run()
        {
            _renderer.UpdatePropertyBlock(_properties, b =>
            {
                b.SetFloat(StartTime, Time.time);
            });
        }

        protected override void DidDisabled() => _consumer.Impacted -= Run;
    }
}