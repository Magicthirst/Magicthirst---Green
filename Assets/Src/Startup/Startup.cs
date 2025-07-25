using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Startup
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private AssetReference bootstrapScene;
        [SerializeField] private AssetReference mainMenuScene;

        private async void Awake()
        {
            await bootstrapScene.LoadSceneAsync(LoadSceneMode.Additive).Task;
            await mainMenuScene.LoadSceneAsync(LoadSceneMode.Additive).Task;
            Destroy(gameObject);
        }
    }
}
