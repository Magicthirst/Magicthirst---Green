using Common;
using Screens.JoinSession;
using Screens.MainMenu;
using Screens.SharedElements;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Screens.Enter.Enter;

namespace DI
{
    public class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder
                .Register<CheckSignIn>(resolver => id => resolver.Resolve<IMenuUserSession>().SignIn(id), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<SignInOrOut.Exit>(resolver => () => resolver.Resolve<IMenuUserSession>().SignOut(), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<JoinSession.AskToJoinSession>
                (
                    resolver => hostId => resolver.Resolve<IMenuUserSession>().JoinSession(hostId),
                    Lifetime.Singleton
                )
                .AsSelf();

            builder
                .Register<MainMenu.HostSession>(resolver => () => resolver.Resolve<IMenuUserSession>().HostSession(), Lifetime.Singleton)
                .AsSelf();

            builder
                .Register<SignInOrOutElement.GetSignedInName>
                (
                    resolver => () => resolver.Resolve<IMenuUserSession>().PlayerId,
                    Lifetime.Singleton
                )
                .AsSelf();

            builder
                .Register<GetSignUpUrl>
                (
                    _ => () => "https://charliebritton.github.io/website-placeholder/",
                    Lifetime.Singleton
                )
                .AsSelf();
        }
    }
}
