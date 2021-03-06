/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using SharpPcap;

namespace PacketSniffer
{
    public static class ArgumentParser
    {
        public static ProgramArguments programArgs;
        public static CaptureDeviceList interfaceList = GetListOfInterfaces();
        public static List<string> ifnameList = GetListOfInterfaceNames();

        /// <summary>
        /// Parse the program arguments and return structure holding them.
        /// </summary>
        public static ProgramArguments Parse(string[] args)
        {
            // Handle optional -i || --interface with no value
            if (args.Length > 0)
            {
                if (args[args.Length - 1] == "-i" || args[args.Length - 1] == "--interface")
                {
                    HandleInterfaceNamePrinting();
                    return programArgs;
                }
            }

            var rootCommand = SetUpRootCommand();
            InvokeArgumentHandler(rootCommand, args);
            return programArgs;
        }

        static RootCommand SetUpRootCommand()
        {
            RootCommand rootCommand = new RootCommand
            {
                new Option<string>(
                    new string[] { "--interface", "-i" },
                    "Interface to listen to. If empty or without argument, " +
                    "print out all available interfaces."
                ),
                new Option<int?>(
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
                new Option<int?>(
                    new string[] { "-n" },
                    "Number of packets to display. If empty, only one packet is displayed."
                )
            };

            rootCommand.Description = @"IPK BUT FIT Summer Course 2021: Packet Sniffer
Network analyzer that catches and filters packets on specific interface.
            ";

            return rootCommand;
        }

        /// <summary>
        /// Invoke the System.CommandLine argument handler.
        /// </summary>
        static void InvokeArgumentHandler(RootCommand rootCommand, string[] args)
        {
            rootCommand.Handler = CommandHandler.Create<string, int?, bool, bool, bool, bool, int?>(HandleArgs);
            int argReturnCode = rootCommand.Invoke(args);
            ValidateArgumentReturnCode(argReturnCode);
        }

        /// <summary>
        /// When no protocol is specified, assume all are selected.
        /// When n or p is not specified, assigne them default value (1 packet and all ports respectively).
        /// </summary>
        public static void HandleArgs(string i, int? p, bool tcp, bool udp, bool icmp, bool arp, int? n)
        {
            ValidateArguments(p, n);
            HandleInterfaceArg(i);

            if (n == null)
            {
                n = 1;
            }
            if (p == null)
            {
                p = -1;
            }
            if (!(tcp || udp || icmp || arp))
            {
                tcp = true;
                udp = true;
                icmp = true;
                arp = true;
            }

            programArgs = new ProgramArguments(i, p, tcp, udp, icmp, arp, n);
        }

        static void ValidateArguments(int? p, int? n)
        {
            if (p < 0)
            {
                Debug.ErrorExit(Code.Error, "'p' must be greater or equal to 0");
            }
            else if (n <= 0)
            {
                Debug.ErrorExit(Code.Error, "'n' must be greater than 0");
            }
        }

        static void HandleInterfaceArg(string i)
        {
            var ifnameList = GetListOfInterfaceNames();

            if (String.IsNullOrEmpty(i))
            {
                HandleInterfaceNamePrinting();
            }
            else if (!ifnameList.Contains(i))
            {
                Debug.ErrorExit(Code.Error, $"No interface named '{i}' on the machine");
            }

        }

        static CaptureDeviceList GetListOfInterfaces()
        {
            return CaptureDeviceList.Instance;
        }

        static List<string> GetListOfInterfaceNames()
        {
            List<string> ifnameList = new List<string>();
            var interfaces = GetListOfInterfaces();

            foreach (ICaptureDevice interf in interfaces)
            {
                ifnameList.Add(interf.Name);
            }
            return ifnameList;
        }

        static void HandleInterfaceNamePrinting()
        {
            if (ifnameList.Count == 0)
            {
                Debug.ErrorExit(Code.Error, "No interface found on the machine");
            }

            foreach (string ifname in ifnameList)
            {
                Console.WriteLine(ifname);
            }
        }

        static void ValidateArgumentReturnCode(int code)
        {
            if (code != 0)
            {
                Debug.ErrorExit(Code.Error, "Arguments entered incorrectly");
            }
        }
    }
}

/// <summary>
/// Structure holding the program arguments, is needed for sniffing.
/// </summary>
public struct ProgramArguments
{
    public ProgramArguments(string ifname, int? port, bool tcp, bool udp, bool icmp, bool arp, int? n)
    {
        Ifname = ifname;
        PortNumber = port;
        Tcp = tcp;
        Udp = udp;
        Icmp = icmp;
        Arp = arp;
        PacketCountToDisplay = n;
    }

    public string Ifname { get; set; }
    public int? PortNumber { get; set; }
    public bool Tcp { get; set; }
    public bool Udp { get; set; }
    public bool Icmp { get; set; }
    public bool Arp { get; set; }
    public int? PacketCountToDisplay { get; set; }

    /// <summary>
    /// Useful for debugging purposes.
    /// </summary>
    public override string ToString() => $@"ifname: {Ifname}
port:   {PortNumber}
tcp:    {Tcp}
udp:    {Udp}
icmp:   {Icmp}
arp:    {Arp}
n:      {PacketCountToDisplay}";
}
