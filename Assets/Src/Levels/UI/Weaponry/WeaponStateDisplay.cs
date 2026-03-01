using System;
using Levels.Core;
using Levels.Util;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Levels.UI.Weaponry
{
    public class WeaponStateDisplay : MonoBehaviour
    {
        [SerializeField] private SVGImage icon;
        [SerializeField] private Image reloadOverlay;
        [SerializeField] private AbilityPosition position;

        private IAbility _Weapon => _weapon.Value;
        private IPropertyHandle<IAbility> _weapon;

        [Inject] private Core.Weaponry _weaponry;
        [Inject] private IWeaponryUIConfig _config;

        private void OnEnable()
        {
            _weapon = position switch
            {
                AbilityPosition.Primary => _weaponry.Primary,
                AbilityPosition.Secondary => _weaponry.Secondary,
                _ => throw new ArgumentOutOfRangeException()
            };
            _weapon.Changed += UpdateDisplay;
        }

        private void Update()
        {
            reloadOverlay.fillAmount = 1 - _Weapon.CooldownProgress;
        }

        private void UpdateDisplay(IAbility weapon)
        {
            icon.sprite = _config[weapon].Sprite;
        }

        private void OnDisable()
        {
            _weapon.Changed -= UpdateDisplay;
        }
    }
}