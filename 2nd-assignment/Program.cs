/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>
#nullable enable

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

using System.Net.Sockets;
using System.Collections.Generic;

namespace PacketSniffer
{
    static class Code {
        public const int Success = 0;
        public const int Error = 1;
    }

    static class Program
    {
        static int Main(string[] args)
        {
            try {
                ArgumentParser.Parse(args);
            } catch (InvalidArgException e) {
                ErrorExit(Code.Error, e.Message);
             }
            TestStuff();

            return Code.Success;
        }

        static void TestStuff()
        {
            Console.WriteLine("IPK Sniffer 🤓");
        }

        static void ErrorExit(int code=1, string message="") {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine($"[ERR] {message}.");
            System.Environment.Exit(code);
    }


    }
}
