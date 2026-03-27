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
        [Inject] private IWeaponryUIConfig _config;

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
                _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
            };

            _selectedAbility.Changed += OnSelectedAbilityChanged;
        }

        private void Start()
        {
            var weapons = _weaponry.Abilities.Where(weapon => weapon.Position == position).ToList();

            var space = abilityDisplayPrefab.GetComponent<RectTransform>().sizeDelta.x;
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, space * weapons.Count);

            _abilitiesObjects = new Dictionary<IAbility, RectTransform>();
            foreach (var weapon in weapons)
            {
                var display = Instantiate(abilityDisplayPrefab, transform);
                display.GetComponent<WeaponStateDisplay>().Init(weapon, _config);
                _abilitiesObjects[weapon] = display.GetComponent<RectTransform>();
                display.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, space * (_abilitiesObjects.Count - 1));
            }
        }

        private void OnSelectedAbilityChanged(IAbility ability)
        {
            selectionOverlay.MoveAtop(_abilitiesObjects[ability]);
        }

        private void OnDisable()
        {
            _selectedAbility.Changed -= OnSelectedAbilityChanged;
        }
    }
}