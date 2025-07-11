using System.Threading.Tasks;
using Screens.JoinSession;
using Screens.MainMenu;
using Screens.SharedElements;
using VContainer;
using VContainer.Unity;
using static Screens.Enter.Enter;

namespace DI
{
    public class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var player = new PlayerStorage();

            builder
                .Register<JoinSession.AskToJoinSession>(_ => hostId => Task.FromResult(hostId == "thishostexists"), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<MainMenu.HostSession>(_ => () => Task.FromResult(true), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<MainMenuElement.IAuthenticatedState>(_ => player, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<SignInOrOutElement.GetSignedInName>(_ => () => player.Id, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<SignInOrOut.Exit>(_ => player.Exit, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<CheckSignIn>(_ => id =>
                {
                    player.Id = id;
                    return Task.FromResult<SignInResult>(new SignInResult.Success());
                }, Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<GetSignUpUrl>(_ => () => "https://charliebritton.github.io/website-placeholder/", Lifetime.Singleton)
                .AsSelf();
        }
    }
}
