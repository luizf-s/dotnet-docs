using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dnd
{
    public struct Config
    {
        public String AssemblyName { get; init; }
        public String TypeName { get; init; }
    }

    class Program
    {
        private static string[] DefaultDirectories = new string[]
        {
            "/usr/share/dotnet/shared/Microsoft.NETCore.App/5.0.7",
            "/usr/share/dotnet/sdk/5.0.301/Microsoft/Microsoft.NET.Build.Extensions/net461/lib"
        };

        const string DOTNET_DLLS_FOLDER = "/usr/share/dotnet/shared/Microsoft.NETCore.App/5.0.7";

        static void Main(string[] args)
        {
            Config config = SetUp(args);
            string dllFilePath = FindDllOf(config.AssemblyName);
            var assembly  = Assembly.LoadFile(dllFilePath);
            Type type     = assembly.GetType(config.TypeName);

            if (type == null) {
                Console.WriteLine($"Could not find {config.TypeName} on {dllFilePath}");
                Environment.Exit(0);
            }
            Console.WriteLine(assembly.FullName);
            Console.WriteLine(type.FullName);

            var interfaces    = type.GetInterfaces();
            var fields        = type.GetFields();
            var properties    = type.GetProperties().Where(t => t.PropertyType.IsPublic);
            var methods       = type.GetMethods().Where(t => t.IsPublic);

            PrintInterfaces(interfaces);
            PrintFields(fields);
            PrintProperties(properties);
            PrintMethods(methods);
        }

        private static string FindDllOf(string assemblyName)
        {
            foreach (string dir in DefaultDirectories) {
                var files = Directory.EnumerateFiles(dir, $"{assemblyName}.dll");
                // TODO: should print options and exit
                if (files.Count() > 0)
                {
                    return files.First();
                }

                foreach (string subdir in Directory.EnumerateDirectories(dir)) {
                    string file = FindDllOf(assemblyName);
                    if (!String.IsNullOrEmpty(file))
                        return file;
                }
            }
            return "";
        }

        private static Config SetUp(string[] args)
        {
            if (args.Count() < 2)
            {
                Console.WriteLine("Please provide a typename. Eg: System.String");
                Environment.Exit(1);
            }
            return new Config
            {
                AssemblyName = args[0],
                TypeName = args[1]
            };
        }

        private static void PrintInterfaces(Type[] interfaces)
        {
            Console.WriteLine("=== implements interfaces ===");
            foreach (Type interf in interfaces)
                Console.WriteLine("  {0}", interf);
        }

        private static void PrintFields(IEnumerable<FieldInfo> fields)
        {
            Console.WriteLine("=== fields ===");
            foreach (FieldInfo field in fields)
                Console.WriteLine("  {0}", field);
        }

        private static void PrintProperties(IEnumerable<PropertyInfo> properties)
        {
            Console.WriteLine("=== properties ===");
            foreach (PropertyInfo property in properties)
                Console.WriteLine("  {0}", property);
        }

        private static void PrintMethods(IEnumerable<MethodInfo> methods)
        {
            Console.WriteLine("=== methods ===");
            foreach (MethodInfo method in methods)
            {
                // TODO: write a better way of doing this
                string staticKeyword   = method.IsStatic   ? "[static] "   : "";
                string finalKeyword    = method.IsFinal    ? "[final] "    : "";
                string abstractKeyword = method.IsAbstract ? "[abstract] " : "";
                Console.WriteLine(
                    "  {0}{1}{2} {3}",
                    staticKeyword,
                    finalKeyword,
                    abstractKeyword,
                    method
                );
            }
        }
    }
}
