using System;

namespace Web.Util
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan AtLeast(this TimeSpan timeSpan, TimeSpan minimum) =>
            timeSpan >= minimum ? timeSpan : minimum;
    }
}
