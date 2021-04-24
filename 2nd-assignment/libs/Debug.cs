using System;
using System.IO;

namespace PacketSniffer
{
    class Debug
    {
        public static void LogMethodsOfObject(object obj)
        {
            var methods = obj.GetType().GetMethods();
            foreach (var method in methods)
            {
                string methodName = method.Name;
                Console.WriteLine(methodName);
            }
        }

        public static void LogPropertiesOfObject(object obj)
        {
            var propertyInfos = obj.GetType().GetProperties();
            foreach (var pInfo in propertyInfos)
            {
                string propertyName = pInfo.Name;
                Console.WriteLine(propertyName);
            }
        }

        public static void ErrorExit(int code = 1, string message = "")
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine($"[ERR] {message}.");
            System.Environment.Exit(code);
        }
    }
}
