using VContainer;
using VContainer.Unity;

namespace DI
{
    public partial class LevelLifetimeScope : LifetimeScope
    {

        protected override void Awake()
        {
            var gameScope = FindAnyObjectByType<GameLifetimeScope>();
            if (gameScope != null)
            {
                EnqueueParent(gameScope);
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
