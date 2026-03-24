using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Config;
using Levels.Core;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Abilities.Shared
{
    public partial class SpellCastPreview : MonoBehaviour
    {
        [SerializeField] private PreviewableSpell[] previews;

        private Transform _activePreview;
        private Dictionary<Type, Transform> _previews;

        [Inject] private ISharedSpellConfig _config;
        [Inject] private Weaponry _weaponry;
        private Transform _camera;

        [Inject]
        private void Construct(Camera injectedCamera) => _camera = injectedCamera.transform;

        private void Awake()
        {
            _previews = previews.ToDictionary
            (
                keySelector: spell => spell.Spell,
                elementSelector: spell => spell.PreviewObject
            );
        }

        private void OnEnable() => _weaponry.Equipped += OnEquipped;

        private void Update()
        {
            if (_activePreview is null)
            {
                return;
            }

            _activePreview.position = SpellCastAnchor.GetAnchorPosition
            (
                origin: transform.position,
                direction: _camera.forward,
                distance: _config.MaxDistance
            );
        }

        private void OnEquipped(IAbility ability)
        {
            if (!_previews.ContainsKey(ability.Type))
            {
                return;
            }

            _activePreview.gameObject.SetActive(false);

            _activePreview = _previews[ability.Type];
            _activePreview.gameObject.SetActive(true);
        }

        private void OnDisable() => _weaponry.Equipped -= OnEquipped;
    }

    [Serializable]
    public class PreviewableSpell
    {
        [SerializeField] private Transform previewObject;

        [SerializeField] [SubtypeProperty(typeof(ISpell))]
        private string spellType;

        public Transform PreviewObject => previewObject;
        public Type Spell => _spell ??= Type.GetType(spellType);

        private Type _spell;
    }
}