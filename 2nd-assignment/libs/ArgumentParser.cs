/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace PacketSniffer
{
    static class ArgumentParser
    {
        public static void Parse(string[] args)
        {
            var rootCommand = SetUpRootCommand();
            int argReturnCode = InvokeArgumentHandler(rootCommand, args);
        }

        static RootCommand SetUpRootCommand()
        {
            RootCommand rootCommand = new RootCommand
            {
                new Option<string>(
                    new string[] { "--interface", "-i" },
                    "Interface to listen to. If empty, all available interfaces are listed."
                ),
                new Option<int>(
                    new string[] { "-p" },
                    "Packets will be filtered by this port. If empty, all ports are considered."
                ),
                new Option<bool>(
                    new string[] { "--tcp", "-t" },
                    "Filter packets by TCP."
                ),
                new Option<bool>(
                    new string[] { "--udp", "-u" },
                    "Filter packets by UDP."
                ),
                new Option<bool>(
                    new string[] { "--icmp" },
                    "Filter packets by ICMPv4 and ICMPv6."
                ),
                new Option<bool>(
                    new string[] { "--arp" },
                    "Filter packets by ARP frames."
                ),
                new Option<int>(
                    new string[] { "-n" },
                    "Number of packets to display. If empty, only one packet is displayed."
                )
            };

            rootCommand.Description = @"IPK BUT FIT Summer Course 2021: Packet Sniffer
Network analyzer that catches and filters packets on specific interface.
            ";

            return rootCommand;
        }

        static int InvokeArgumentHandler(RootCommand rootCommand, string[] args)
        {
            rootCommand.Handler = CommandHandler.Create<string, int, bool, bool, bool, bool, int>(HandleArgs);
            int argReturnCode = rootCommand.InvokeAsync(args).Result;
            ValidateArgumentReturnCode(argReturnCode);

            return 0;
        }

        static void HandleArgs(string i, int p, bool tcp, bool udp, bool icmp, bool arp, int n)
        {
            Console.WriteLine($"ifname: {i}");
            Console.WriteLine($"port: {p}");
            Console.WriteLine($"tcp: {tcp}");
            Console.WriteLine($"udp: {udp}");
            Console.WriteLine($"icmp: {icmp}");
            Console.WriteLine($"arp: {arp}");
            Console.WriteLine($"packetCount: {n}");
        }

        static void ValidateArgumentReturnCode(int code) {
            if (code != 0)
            {
                throw new InvalidArgException("Invalid Program Arguments present");
            }
        }
    }
}
