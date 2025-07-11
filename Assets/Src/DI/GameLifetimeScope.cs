using Screens;
using VContainer;
using VContainer.Unity;
using Web;

namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        private ConnectionRole _connectionRole = ConnectionRole.Offline;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder
                .Register<IAssignConnectionRole>(_ => new AssignConnectionRole(this), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register(_ => _connectionRole, Lifetime.Transient)
                .AsSelf();
        }

        private class AssignConnectionRole : IAssignConnectionRole
        {
            private readonly GameLifetimeScope _scope;

            public AssignConnectionRole(GameLifetimeScope scope)
            {
                _scope = scope;
            }

            public void Offline() => _scope._connectionRole = ConnectionRole.Offline;

            public void Host() => _scope._connectionRole = ConnectionRole.Host;

            public void Guest() => _scope._connectionRole = ConnectionRole.Guest;
        }
    }
}
