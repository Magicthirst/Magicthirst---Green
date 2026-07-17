using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shared
{
    [Serializable]
    public class GameNavigation
    {
        [SerializeField] private AssetReference mainMenu;
        [SerializeField] private AssetReference onlyGameplayLevel;

        private GameNavigation() {}

        public void FailLevel() => mainMenu.LoadSceneAsync();

        public void GoMainMenu() => mainMenu.LoadSceneAsync();
        
        public void GoGame() => onlyGameplayLevel.LoadSceneAsync();

        public void QuitGame() => Application.Quit(0);
    }
}