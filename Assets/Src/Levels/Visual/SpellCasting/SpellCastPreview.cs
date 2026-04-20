using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core;
using UnityEngine;
using Util;
using VContainer;

namespace Levels.Visual.SpellCasting
{
    public partial class SpellCastPreview : MonoBehaviour
    {
        [SerializeField] private PreviewableSpell[] previews;

        private Active _active = new() { Ability = null, Preview = null };
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

            foreach (var (_, preview) in _previews)
            {
                preview.gameObject.SetActive(false);
            }
        }

        private void OnEnable() => _weaponry.Equipped += OnEquipped;

        private void Update()
        {
            if (!_active.Exists)
            {
                return;
            }

            if (_active.Ability.CooldownProgress < 1f)
            {
                if (_active.Preview.gameObject.activeSelf)
                {
                    _active.Preview.gameObject.SetActive(false);
                }
                return;
            }

            if (!_active.Preview.gameObject.activeSelf)
            {
                _active.Preview.gameObject.SetActive(true);
            }

            _active.Preview.position = SpellCastAnchor.GetAnchorPosition
            (
                origin: transform.position,
                direction: _camera.forward,
                distance: _config.MaxDistance
            );
        }

        private void OnEquipped(IAbility ability)
        {
            _active.Preview?.gameObject.SetActive(false);

            if (_previews.TryGetValue(ability.Type, out var preview))
            {
                _active = new Active
                {
                    Ability = ability,
                    Preview = preview
                };
                _active.Preview.gameObject.SetActive(true);
            }
            else
            {
                _active = new Active
                {
                    Ability = null,
                    Preview = null
                };
            }
        }

        private void OnDisable() => _weaponry.Equipped -= OnEquipped;

        private struct Active
        {
            public IAbility Ability;
            public Transform Preview;

            public bool Exists => Ability != null && Preview != null;
        }
    }

    [Serializable]
    public class PreviewableSpell
    {
        [SerializeField] private Transform previewObject;

        [SerializeField]
        [SubtypeProperty(typeof(ISpell))]
        private string spellType;

        public Transform PreviewObject => previewObject;
        public Type Spell => _spell ??= Type.GetType(spellType);

        private Type _spell;
    }
}