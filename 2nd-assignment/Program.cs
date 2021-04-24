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
using System.Reflection;

namespace PacketSniffer
{
    static class Code
    {
        public const int Success = 0;
        public const int Error = 1;
    }

    static class Program
    {
        static int Main(string[] args)
        {
            ArgumentParser.Parse(args);

            var programArgs = ArgumentParser.programArgs;
            var interfaceList = ArgumentParser.interfaceList;
            var ifnameList = ArgumentParser.ifnameList;

            if (String.IsNullOrEmpty(programArgs.Ifname))
            {
                return Code.Success;
            }

            Console.WriteLine(programArgs);

            return Code.Success;
        }
    }
}
