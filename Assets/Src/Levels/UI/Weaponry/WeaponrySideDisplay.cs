using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Core;
using Levels.Util;
using UnityEngine;
using VContainer;

namespace Levels.UI.Weaponry
{
    [RequireComponent(typeof(RectTransform))]
    public class WeaponrySideDisplay : MonoBehaviour
    {
        [SerializeField] private AbilityPosition position;
        [SerializeField] private GameObject abilityDisplayPrefab;
        [SerializeField] private SelectionOverlay selectionOverlay;

        private RectTransform _transform;
        private IPropertyHandle<IAbility> _selectedAbility;

        private Dictionary<IAbility, RectTransform> _abilitiesObjects;

        [Inject] private Core.Weaponry _weaponry;
        [Inject] private WeaponryUIConfig _config;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _selectedAbility = position switch
            {
                AbilityPosition.Primary => _weaponry.Primary,
                AbilityPosition.Secondary => _weaponry.Secondary,
                _ => throw new ArgumentOutOfRangeException()
            };

            _selectedAbility.Changed += OnSelectedAbilityChanged;
            _weaponry.AvailableAbilitiesChanged += Rebuild;
        }

        private void Start()
        {
            Rebuild();
        }

        private void Rebuild()
        {
            if (_abilitiesObjects != null)
            {
                foreach (var child in _abilitiesObjects.Values)
                {
                    Destroy(child.gameObject);
                }
            }

            _abilitiesObjects = new Dictionary<IAbility, RectTransform>();

            var weapons = _weaponry.Abilities.Where(w => w.Position == position).ToList();
            var space = abilityDisplayPrefab.GetComponent<RectTransform>().sizeDelta.x;

            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, space * weapons.Count);

            foreach (var weapon in weapons)
            {
                var display = Instantiate(abilityDisplayPrefab, transform);
                display.GetComponent<WeaponStateDisplay>().Init(weapon, _config, weapon.KeyName);

                var rect = display.GetComponent<RectTransform>();
                rect.anchoredPosition -= new Vector2(0, space * _abilitiesObjects.Count);

                _abilitiesObjects.Add(weapon, rect);
            }

            if (_selectedAbility.Value != null &&
                _abilitiesObjects.TryGetValue(_selectedAbility.Value, out var selected))
            {
                selectionOverlay.MoveAtop(selected);
            }
            else
            {
                selectionOverlay.MoveAtop(null);
            }
        }

        private void OnSelectedAbilityChanged(IAbility ability)
        {
            if (_abilitiesObjects.TryGetValue(ability, out var rect))
            {
                selectionOverlay.MoveAtop(rect);
            }
        }

        private void OnDisable()
        {
            _selectedAbility.Changed -= OnSelectedAbilityChanged;
            _weaponry.AvailableAbilitiesChanged -= Rebuild;
        }
    }
}