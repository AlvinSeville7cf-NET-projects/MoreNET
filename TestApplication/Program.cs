using System;
using MoreNET.Utils;

namespace TestApplication
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var result = BaseType.ReadChar();
            Console.WriteLine(result);
        }
    }
}
