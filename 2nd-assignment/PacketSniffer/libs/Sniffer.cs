/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/24</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using SharpPcap;
using PacketDotNet;

namespace PacketSniffer
{
    class Sniffer
    {
        /// <summary>
        /// Main handling of the code logic is done here.
        /// </summary>
        public Sniffer(ICaptureDevice interf, ProgramArguments args)
        {
            Interface = interf;
            ProgramArgs = args;
        }

        private ICaptureDevice Interface { get; set; }
        private ProgramArguments ProgramArgs { get; set; }
        private int ProcessedPacketCount { get; set; }
        private bool sniffingDone = false;

        /// <summary>
        /// Main method for doing the heavy work.
        /// </summary>
        public void Sniff()
        {
            // Open interface in promicuous mode, default time out 1000 milliseconds.
            int readTimeoutInMilliseconds = 1000;
            Interface.Open(DeviceMode.Promiscuous, readTimeoutInMilliseconds);
            string filter = SetInterfaceFilter();

            Console.WriteLine($"-- Sniffing on {Interface.Description}\n-- using filter: {filter}");

            // Set interface handling for packet arrival
            Interface.OnPacketArrival += Interface_OnPacketArrival;
            Interface.StartCapture();
            while (!sniffingDone) { }  // wait until sniffing is done

            Console.WriteLine(Interface.Statistics.ToString());
            Console.WriteLine("-- Finished sniffing...");
        }


        /// <summary>
        /// Set interface filter and return the created filter.
        /// </summary>
        /// <example>
        /// icmp or icmp6 or arp or udp port 80 or tcp port 80
        /// </example>
        string SetInterfaceFilter()
        {
            string filter = "";
            if (ProgramArgs.Icmp)
            {
                filter += "icmp or icmp6";
            }
            if (ProgramArgs.Arp)
            {
                if (!String.IsNullOrWhiteSpace(filter))
                {
                    filter += " or ";
                }
                filter += "arp";
            }
            if (ProgramArgs.Udp)
            {
                if (!String.IsNullOrWhiteSpace(filter))
                {
                    filter += " or ";
                }
                filter += "udp";
                if (ProgramArgs.PortNumber != -1)
                {
                    filter += $" port {ProgramArgs.PortNumber}";
                }
            }
            if (ProgramArgs.Tcp)
            {
                if (!String.IsNullOrWhiteSpace(filter))
                {
                    filter += " or ";
                }
                filter += "tcp";
                if (ProgramArgs.PortNumber != -1)
                {
                    filter += $" port {ProgramArgs.PortNumber}";
                }
            }

            Interface.Filter = filter;
            return filter;
        }

        /// <summary>
        /// Function that is called every time a packet arrives.
        /// </summary>
        void Interface_OnPacketArrival(object sender, CaptureEventArgs evnt)
        {
            var interf = (ICaptureDevice)sender;
            ProcessPacket(evnt.Packet);

            if (ProcessedPacketCount >= ProgramArgs.PacketCountToDisplay)
            {
                interf.StopCapture();
                sniffingDone = true;
            }
        }

        /// <summary>
        /// Main function for processsing packets.
        /// </summary>
        void ProcessPacket(RawCapture rawPacket)
        {
            DateTime time = rawPacket.Timeval.Date;
            string rfc3339Time = Utils.ConvertTimeToRfc3339(time);

            Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            OutputPacket outputPacket = GetCorrectPacket(packet);
            outputPacket.Length = rawPacket.Data.Length;
            outputPacket.Time = rfc3339Time;
            outputPacket.Data = rawPacket.Data;

            Console.WriteLine(outputPacket);
        }

        /// <summary>
        /// Get correct packet depending on the filter.
        /// Either tcp, udp, arp, icmp
        /// All packets are gained both in ipv4 and ipv6 form.
        /// </summary>
        OutputPacket GetCorrectPacket(Packet packet)
        {
            TcpPacket tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            UdpPacket udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
            ArpPacket arpPacket = packet.Extract<PacketDotNet.ArpPacket>();
            IcmpV4Packet icmpV4Packet = packet.Extract<PacketDotNet.IcmpV4Packet>();
            IcmpV6Packet icmpV6Packet = packet.Extract<PacketDotNet.IcmpV6Packet>();

            OutputPacket outputPacket;
            bool isInstantiazed = false;
            if (tcpPacket != null)
            {
                outputPacket = GetTcpPacket(tcpPacket);
                isInstantiazed = true;
            }
            else if (udpPacket != null)
            {
                outputPacket = GetUdpPacket(udpPacket);
                isInstantiazed = true;
            }
            else if (arpPacket != null)
            {
                outputPacket = GetArpPacket(arpPacket);
                isInstantiazed = true;
            }
            else if (icmpV4Packet != null)
            {
                outputPacket = GetIcmpV4Packet(icmpV4Packet);
                isInstantiazed = true;
            }
            else
            {
                outputPacket = GetIcmpV6Packet(icmpV6Packet);
                isInstantiazed = true;
            }

            if (isInstantiazed)
            {
                ProcessedPacketCount++;
            }

            return outputPacket;
        }

        /// <summary>
        /// Construct OutputPacket from a TCP packet.
        /// </summary>
        OutputPacket GetTcpPacket(TcpPacket tcpPacket)
        {
            var ipPacket = (PacketDotNet.IPPacket)tcpPacket.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            int srcPort = tcpPacket.SourcePort;
            int dstPort = tcpPacket.DestinationPort;
            byte[] data = tcpPacket.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }

        /// <summary>
        /// Construct OutputPacket from an UDP packet.
        /// </summary>
        OutputPacket GetUdpPacket(UdpPacket udpPacket)
        {
            var ipPacket = (PacketDotNet.IPPacket)udpPacket.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            int srcPort = udpPacket.SourcePort;
            int dstPort = udpPacket.DestinationPort;
            byte[] data = udpPacket.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }

        /// <summary>
        /// Construct OutputPacket from an ARP packet.
        /// ARP packet has no ports.
        /// </summary>
        OutputPacket GetArpPacket(ArpPacket arpPacket)
        {
            var ipPacket = (PacketDotNet.IPPacket)arpPacket.ParentPacket;
            Console.WriteLine(ipPacket);
            System.Net.NetworkInformation.PhysicalAddress srcIp = arpPacket.SenderHardwareAddress;
            System.Net.NetworkInformation.PhysicalAddress dstIp = arpPacket.TargetHardwareAddress;
            int srcPort = -1;
            int dstPort = -1;
            byte[] data = arpPacket.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            Console.WriteLine(7);
            return outputPacket;
        }

        /// <summary>
        /// Construct OutputPacket from an ICMPv4 packet.
        /// ICMPv4 packet has no ports.
        /// </summary>
        OutputPacket GetIcmpV4Packet(IcmpV4Packet icmpV4Packet)
        {
            var ipPacket = (PacketDotNet.IPPacket)icmpV4Packet.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            int srcPort = -1;
            int dstPort = -1;
            byte[] data = icmpV4Packet.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }

        /// <summary>
        /// Construct OutputPacket from an ICMPv6 packet.
        /// ICMPv6 packet has no ports.
        /// </summary>
        OutputPacket GetIcmpV6Packet(IcmpV6Packet icmpV6Packet)
        {
            var ipPacket = (PacketDotNet.IPPacket)icmpV6Packet.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            int srcPort = -1;
            int dstPort = -1;
            byte[] data = icmpV6Packet.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }
    }
    /// <summary>
    /// Structure for holding information about outputted packet.
    /// Calling Console.WriteLine causes it to print the final packet
    /// as specified by the assignemnt.
    /// </summary>
    public struct OutputPacket
    {
        public OutputPacket(object srcIp, int srcPort, object dstIp,
                            int dstPort, byte[] packetData, int length = 0, string time = "")
        {
            Time = time;
            SrcIp = srcIp.ToString();
            SrcPort = srcPort;
            DstIp = dstIp.ToString();
            DstPort = dstPort;
            Length = length;
            Data = packetData;
        }

        public string Time { get; set; }
        public string SrcIp { get; set; }
        public int SrcPort { get; set; }
        public string DstIp { get; set; }
        public int DstPort { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// Function that outputs information about the packet in
        /// a human-readable form as specified by the assignment.
        /// </summary>
        public override string ToString() {
            if (SrcPort == -1 || DstPort == -1) {
                return String.Format(
                    "{0} {1} > {2}, length {3} bytes\n{4}",
                    Time, SrcIp, DstIp, Length,
                    Utils.ConvertPacketDataToOutputFormat(Data)
                );
            } else {
                return String.Format(
                    "{0} {1} : {2} > {3} : {4}, length {5} bytes\n{6}",
                    Time, SrcIp, SrcPort, DstIp, DstPort, Length,
                    Utils.ConvertPacketDataToOutputFormat(Data)
                );
            }
        }
    }
}
