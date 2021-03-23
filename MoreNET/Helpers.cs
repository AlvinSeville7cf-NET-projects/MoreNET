using System;

namespace MoreNET
{
    internal static class Helpers
    {
        public static string Indent(string input, int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException($"{nameof(level)} can't be less than zero", nameof(level));
            
            return new string('\t', level) + input;
        }
    }
}
