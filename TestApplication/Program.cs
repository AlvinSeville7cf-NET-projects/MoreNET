using System;
using MoreNET.Utils;

namespace TestApplication
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var result = BaseType.ReadInt32<Exception>(input => Console.WriteLine($"Error because {input} is not a number."), "Number: ");
            Console.WriteLine(result);
        }
    }
}
