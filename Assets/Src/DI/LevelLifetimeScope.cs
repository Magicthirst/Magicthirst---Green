using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public partial class LevelLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameLifetimeScope debugFallbackGameScope;

        protected override void Awake()
        {
            var gameScopes = FindObjectsByType<GameLifetimeScope>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

            var trueScope = gameScopes.FirstOrDefault(s => s != debugFallbackGameScope);
            var scope = trueScope ?? debugFallbackGameScope;
            if (scope != null)
            {
                EnqueueParent(scope);
            }

            Build();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureCore(builder);
            ConfigureSync(builder);            
            ConfigureIntentsImpacts(builder);
        }

    }
}
