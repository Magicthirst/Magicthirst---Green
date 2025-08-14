using System.Text;
using Riptide;

namespace Web.Util
{
    internal static class RiptideMessageExtensions
    {
        internal static string ToStringOfBits(this Message message)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < message.BytesInUse; i++)
            {
                message.PeekBits(8, i * 8, out byte b);
                for (var j = 0; j < 8; j++)
                {
                    builder.Append(((b >> j) & 1) == 0 ? '0' : '1');
                }

                builder.Append(' ');
            }

            return builder.ToString();
        }
    }
}