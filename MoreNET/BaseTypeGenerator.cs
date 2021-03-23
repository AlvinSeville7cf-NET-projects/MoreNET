using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace MoreNET.Utils
{
    [Generator]
    internal class BaseTypeGenerator : ISourceGenerator
    {
        private struct ReadMethodInfo
        {
            public Type name;
            public bool useParse;
            public bool oneChar;

            public ReadMethodInfo(Type name, bool useParse, bool oneChar)
            {
                this.name = name;
                this.useParse = useParse;
                this.oneChar = oneChar;
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var classBuilder = new StringBuilder(@"
using System;

namespace MoreNET.Utils
{
    public static class BaseType
    {
");
            ReadMethodInfo[] info = new ReadMethodInfo[]
            {
                new ReadMethodInfo(typeof(byte), true, false),
                new ReadMethodInfo(typeof(sbyte), true, false),
                new ReadMethodInfo(typeof(short), true, false),
                new ReadMethodInfo(typeof(ushort), true, false),
                new ReadMethodInfo(typeof(int), true, false),
                new ReadMethodInfo(typeof(uint), true, false),
                new ReadMethodInfo(typeof(long), true, false),
                new ReadMethodInfo(typeof(ulong), true, false),
                new ReadMethodInfo(typeof(char), false, true),
                new ReadMethodInfo(typeof(string), false, false),
            };

            foreach (var item in info)
            {
                classBuilder.AppendLine(GenerateTypeReadWithoutErrorHandler(item.name, item.useParse, item.oneChar));
                classBuilder.AppendLine();
            }

            classBuilder.Append(@"
    }
}");

            context.AddSource("baseTypeGenerator", SourceText.From(classBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private string GenerateTypeReadWithoutErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}(string prompt = \"\")", 2));
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            if (useParse)
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse({readMethod});", 3));
            else
                method.AppendLine(Helpers.Indent($"    return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }
    }
}
