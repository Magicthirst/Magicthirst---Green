using Levels.Core;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Levels.UI.Weaponry
{
    public class WeaponStateDisplay : MonoBehaviour
    {
        [SerializeField] private SVGImage icon;
        [SerializeField] private Image reloadOverlay;
        [SerializeField] private TextMeshProUGUI keyView;

        private IAbility _weapon;

        public void Init(IAbility weapon, WeaponryUIConfig config, string key)
        {
            _weapon = weapon;
            icon.sprite = config[weapon].Sprite;
            keyView.text = key;
            Update();
        }

        private void Update()
        {
            reloadOverlay.fillAmount = 1 - _weapon.CooldownProgress;
        }
    }
}