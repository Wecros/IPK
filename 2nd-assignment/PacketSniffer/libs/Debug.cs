/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/24</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.IO;

namespace PacketSniffer
{
    /// <summary>
    /// Debug class for logging and exiting the program.
    /// </summary>
    static class Debug
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

        /// <summary>
        /// Default way of exiting the program when error is encountered.
        /// </summary>
        public static void ErrorExit(int code = 1, string message = "")
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine($"[ERR] {message}.");
            System.Environment.Exit(code);
        }
    }
}
