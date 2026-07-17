using System;

namespace Util.UI
{
    [Serializable]
    public class MonospaceFix
    {
        public bool isEven;

        public string Fix(string text) => text.Length % 2 == 0 == isEven ? text : " " + text;
    }
}