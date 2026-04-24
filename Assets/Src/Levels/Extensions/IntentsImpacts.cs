using System;
using Levels.IntentsImpacts;

namespace Levels.Extensions
{
    public static class IntentsImpacts
    {
        public static PublishIntent<T> WithSideeffect<T>(this PublishIntent<T> publishIntent, Action<T> action) where T : IIntent
        {
            return intent =>
            {
                action(intent);
                return publishIntent(intent);
            };
        }
    }
}