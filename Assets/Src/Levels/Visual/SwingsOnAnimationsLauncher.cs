using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Levels.Directorship;
using Levels.Extensions;
using Levels.Util;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual
{
    /// <summary>
    /// Spawns visual "swing" or "slash" objects when specific sprite sequences are played.
    /// This is typically used to give visual feedback for melee attacks.
    /// </summary>
    public class SwingsOnAnimationsLauncher : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => LevelActivityMask.Gameplay | LevelActivityMask.Tutorial;

        [Header("Swing Properties")]
        [Tooltip("How long the swing visual lasts in seconds.")]
        [SerializeField] private float lifeTime = 0.5f;
        [Tooltip("How fast the swing moves away from the camera/origin.")]
        [SerializeField] private float speed = 5f;
        [Tooltip("Curve defining the scale over the swing's lifetime.")]
        [SerializeField] private AnimationCurve scaleCurve;
        [Tooltip("Curve defining the transparency (alpha) over the swing's lifetime.")]
        [SerializeField] private AnimationCurve alphaCurve;

        [Header("References")]
        [Tooltip("The component that broadcasts when the current sprite changes.")]
        [SerializeField] private SpriteChangeSource spriteSource;
        
        [Tooltip("Mappings defining which sequence of sprites triggers which swing visual.")]
        [SerializeField] private SwingMapping[] mappings;

        private Transform _camera;
        
        private readonly List<Sprite> _spriteHistory = new();
        private int _maxRequiredHistoryLength = 0;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _maxRequiredHistoryLength = mappings.Max(mapping => mapping.TriggerSequence.Count);

            foreach (var mapping in mappings)
            {
                mapping.Init();
            }
        }

        protected override void DidEnabled()
        {
            spriteSource.SpriteChanged += OnSpriteChanged;
            _spriteHistory.Clear();
        }

        protected override void DidDisabled()
        {
            spriteSource.SpriteChanged -= OnSpriteChanged;
        }

        private void OnSpriteChanged(Sprite newSprite)
        {
            if (_maxRequiredHistoryLength == 0) return;

            _spriteHistory.Add(newSprite);

            if (_spriteHistory.Count > _maxRequiredHistoryLength)
            {
                _spriteHistory.RemoveAt(0);
            }

            CheckForSwingTriggers();
        }

        private void CheckForSwingTriggers()
        {
            if (mappings.TryGetFirst(out var mapping, m => _spriteHistory.EndsWith(m.TriggerSequence)))
            {
                LaunchSwing(mapping);
            }
        }

        private void LaunchSwing(SwingMapping mapping)
        {
            if (mapping.CurrentCoroutine != null)
            {
                StopCoroutine(mapping.CurrentCoroutine);
            }

            mapping.CurrentCoroutine = StartCoroutine(AnimateSwingRoutine(mapping).WithInterruptions(_LevelLifecycle));
        }

        private IEnumerator AnimateSwingRoutine(SwingMapping mapping)
        {
            var startTime = LevelDirector.GameplayTime;
            var endTime = startTime + lifeTime;

            var swingTransform = mapping.Transform;
            var swingRenderer = mapping.Renderer;

            var initialDirection = _camera.forward;
            swingTransform.localPosition = mapping.StartPosition;
            swingTransform.localScale = mapping.StartScale;
            
            var initialColor = Color.white;
            var initialAlpha = alphaCurve.Evaluate(0f);
            swingRenderer.color = initialColor.With(a: initialAlpha);
            
            mapping.SwingPrefab.SetActive(true);

            while (LevelDirector.GameplayTime < endTime)
            {
                var elapsedTime = LevelDirector.GameplayTime - startTime;
                var normalizedTime = Mathf.Clamp01(elapsedTime / lifeTime);

                var currentAlpha = alphaCurve.Evaluate(normalizedTime);
                var currentScale = scaleCurve.Evaluate(normalizedTime);

                swingRenderer.color = initialColor.With(a: currentAlpha);
                swingTransform.localScale = mapping.StartScale * currentScale;
                
                var movement = initialDirection * (LevelDirector.GameplayDeltaTime * speed);
                swingTransform.position += movement;

                yield return null;
            }

            swingRenderer.color = initialColor.With(a: 0f);
            mapping.SwingPrefab.SetActive(false);
            mapping.CurrentCoroutine = null;
        }

        [Serializable]
        private class SwingMapping
        {
            [Tooltip("The exact sequence of sprites that must play in order to trigger this swing.")]
            [field: SerializeField] public List<Sprite> TriggerSequence { get; set; }
            
            [Tooltip("The GameObject (a sprite) to animate. Note: this script currently reuses this object.")]
            [field: SerializeField] public GameObject SwingPrefab { get; set; }

            public Vector3 StartPosition { get; private set; }
            public Vector3 StartScale { get; private set; }
            
            public Transform Transform { get; private set; }
            public SpriteRenderer Renderer { get; private set; }
            
            public Coroutine CurrentCoroutine { get; set; }

            public void Init()
            {
                Transform = SwingPrefab.transform;
                Renderer = SwingPrefab.GetComponent<SpriteRenderer>();

                StartPosition = Transform.localPosition;
                StartScale = Transform.localScale;

                Renderer.color = Renderer.color.With(a: 0f);
                SwingPrefab.SetActive(false);
            }
        }
    }
}