using Levels.Core;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Levels.UI.Weaponry
{
    public class WeaponStateDisplay : MonoBehaviour
    {
        [SerializeField] private SVGImage icon;
        [SerializeField] private Image reloadOverlay;

        private IAbility _weapon;

        public void Init(IAbility weapon, IWeaponryUIConfig config)
        {
            _weapon = weapon;
            icon.sprite = config[weapon].Sprite;
            Update();
        }

        private void Update()
        {
            reloadOverlay.fillAmount = 1 - _weapon.CooldownProgress;
        }
    }
}