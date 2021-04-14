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

            for (int i = 0; i < info.Length; i++)
            {
                classBuilder.AppendLine(GenerateTypeReadWithoutErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
                classBuilder.AppendLine(GenerateTypeReadWithParameterlessActionErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
                classBuilder.AppendLine(GenerateTypeReadWithActionErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
                classBuilder.AppendLine(GenerateTypeReadWithParameterlessFuncErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
                if (i < info.Length - 1)
                    classBuilder.AppendLine(GenerateTypeReadWithFuncErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
                else
                    classBuilder.Append(GenerateTypeReadWithFuncErrorHandler(info[i].name, info[i].useParse, info[i].oneChar));
            }

            classBuilder.Append(@"    }
}");

            context.AddSource("baseTypeGenerator", SourceText.From(classBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private string GenerateTypeReadWithoutErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}(String prompt = \"\")", 2));
            method.AppendLine();
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            if (useParse)
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse({readMethod});", 3));
            else
                method.AppendLine(Helpers.Indent($"return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }

        private string GenerateTypeReadWithParameterlessActionErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}<TException>(Action handler, String prompt = \"\")", 2));
            method.AppendLine();
            method.AppendLine(Helpers.Indent("where TException : Exception", 3));
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            if (useParse)
            {
                method.AppendLine(Helpers.Indent("try", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse({readMethod});", 4));
                method.AppendLine(Helpers.Indent("}", 3));
                method.AppendLine(Helpers.Indent("catch (TException)", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent("handler();", 4));
                method.AppendLine(Helpers.Indent("return default;", 4));
                method.AppendLine(Helpers.Indent("}", 3));
            }
            else
                method.AppendLine(Helpers.Indent($"return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }

        private string GenerateTypeReadWithActionErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}<TException>(Action<String> handler, String prompt = \"\")", 2));
            method.AppendLine();
            method.AppendLine(Helpers.Indent("where TException : Exception", 3));
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            method.AppendLine(Helpers.Indent($"var input = {readMethod};", 3));
            if (useParse)
            {
                method.AppendLine(Helpers.Indent("try", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse(input);", 4));
                method.AppendLine(Helpers.Indent("}", 3));
                method.AppendLine(Helpers.Indent("catch (TException)", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent("handler(input);", 4));
                method.AppendLine(Helpers.Indent("return default;", 4));
                method.AppendLine(Helpers.Indent("}", 3));
            }
            else
                method.AppendLine(Helpers.Indent($"return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }

        private string GenerateTypeReadWithParameterlessFuncErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}<TException>(Func<{type.Name}> handler, String prompt = \"\")", 2));
            method.AppendLine();
            method.AppendLine(Helpers.Indent("where TException : Exception", 3));
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            if (useParse)
            {
                method.AppendLine(Helpers.Indent("try", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse({readMethod});", 4));
                method.AppendLine(Helpers.Indent("}", 3));
                method.AppendLine(Helpers.Indent("catch (TException)", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent("return handler();", 4));
                method.AppendLine(Helpers.Indent("}", 3));
            }
            else
                method.AppendLine(Helpers.Indent($"return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }

        private string GenerateTypeReadWithFuncErrorHandler(Type type, bool useParse = true, bool oneChar = true)
        {
            StringBuilder method = new StringBuilder(Helpers.Indent($"public static {type.Name} Read{type.Name}<TException>(Func<String, {type.Name}> handler, String prompt = \"\")", 2));
            method.AppendLine();
            method.AppendLine(Helpers.Indent("where TException : Exception", 3));
            method.AppendLine(Helpers.Indent("{", 2));
            method.AppendLine(Helpers.Indent("Console.Write(prompt);", 3));
            string readMethod = oneChar ? "Console.ReadKey(true).KeyChar" : "Console.ReadLine()";
            method.AppendLine(Helpers.Indent($"var input = {readMethod};", 3));
            if (useParse)
            {
                method.AppendLine(Helpers.Indent("try", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent($"return {type.Name}.Parse(input);", 4));
                method.AppendLine(Helpers.Indent("}", 3));
                method.AppendLine(Helpers.Indent("catch (TException)", 3));
                method.AppendLine(Helpers.Indent("{", 3));
                method.AppendLine(Helpers.Indent("return handler(input);", 4));
                method.AppendLine(Helpers.Indent("}", 3));
            }
            else
                method.AppendLine(Helpers.Indent($"return {readMethod};", 3));
            method.AppendLine(Helpers.Indent("}", 2));
            return method.ToString();
        }
    }
}
