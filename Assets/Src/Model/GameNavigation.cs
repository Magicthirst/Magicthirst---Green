using System;
using Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Model
{
    [Serializable]
    public class GameNavigation : IGameNavigation
    {
        [SerializeField] private AssetReference mainMenu;
        [SerializeField] private AssetReference onlyGameplayLevel;
        [SerializeField] private AssetReference joinSession;
        [SerializeField] private AssetReference signIn;

        public void FailLevel() => mainMenu.LoadSceneAsync();

        public void GoMainMenu() => mainMenu.LoadSceneAsync();
        
        public void GoGame() => onlyGameplayLevel.LoadSceneAsync();
        
        public void GoJoinSession() => joinSession.LoadSceneAsync();

        public void GoSignIn() => signIn.LoadSceneAsync();

        public void QuitGame() => Application.Quit(0);
    }
}