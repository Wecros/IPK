/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using SharpPcap;

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
            ValidateArguments(p, n);

            if (String.IsNullOrEmpty(i)) {
                var devices = CaptureDeviceList.Instance;
                foreach (var dev in devices) {
                    Console.WriteLine($"{dev}\n");
                }
            }

            if (!(tcp || udp || icmp || arp)) {
                tcp = true; 
                udp = true; 
                icmp = true; 
                arp = true; 
            }
            var args = new ProgramArguments(i, p, tcp, udp, icmp, arp, n);
            Console.WriteLine(args);
        }

        static void ValidateArguments(int p, int n){
            if (p < 0) {
                throw new InvalidArgException("'n' must be greater than 0");
            }
            else if (n < 0) {
                throw new InvalidArgException("'n' must be greater than 0");
            }
        }


        static void ValidateArgumentReturnCode(int code) {
            if (code != 0)
            {
                throw new InvalidArgException("Arguments entered incorrectly");
            }
        }
    }
}

public struct ProgramArguments {
    public ProgramArguments(string ifname, int port, bool tcp, bool udp, bool icmp, bool arp, int n) {
        Ifname = ifname;
        PortNumber = port;
        Tcp = tcp;
        Udp = udp;
        Icmp = icmp;
        Arp = arp;
        PacketCountToDisplay = n;
    }

    public string Ifname { get; set; }
    public int PortNumber { get; set; }
    public bool Tcp { get; set; }
    public bool Udp { get; set; }
    public bool Icmp { get; set; }
    public bool Arp { get; set; }
    public int PacketCountToDisplay { get; set; }

    public override string ToString() => $@"ifname: {Ifname}
port:   {PortNumber}
tcp:    {Tcp}
udp:    {Udp}
icmp:   {Icmp}
arp:    {Arp}
n:      {PacketCountToDisplay}";
}

