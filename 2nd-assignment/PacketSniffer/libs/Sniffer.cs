/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/24</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using SharpPcap;
using System.Globalization;
using PacketDotNet;
using System.Text;

namespace PacketSniffer
{
    class Sniffer
    {
        public Sniffer(ICaptureDevice interf, ProgramArguments args)
        {
            Interface = interf;
            ProgramArgs = args;
        }

        private ICaptureDevice Interface { get; set; }
        private ProgramArguments ProgramArgs { get; set; }
        private int ProcessedPacketCount { get; set; }
        private bool sniffingDone = false;

        public void Sniff()
        {
            int readTimeoutInMilliseconds = 1000;
            Interface.Open(DeviceMode.Promiscuous, readTimeoutInMilliseconds);
            string filter = SetInterfaceFilter();
            Console.WriteLine($"-- Sniffing on {Interface.Description}\n-- using filter: {filter}");


            Interface.OnPacketArrival += Interface_OnPacketArrival;
            Interface.StartCapture();
            while (!sniffingDone) { }  // wait until sniffing is done

            Console.WriteLine(Interface.Statistics.ToString());
            Console.WriteLine("-- Finished sniffing...");
        }

        string SetInterfaceFilter()
        {
            //icmp or icmp6 or arp or udp port 80 or tcp port 80
            //icmp or icmp6 or arp or udp port 80 or tcp port 80
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

        OutputPacket GetCorrectPacket(Packet packet)
        {
            TcpPacket tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            UdpPacket udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
            ArpPacket arpPacket = packet.Extract<PacketDotNet.ArpPacket>();
            IcmpV4Packet icmpV4Packet = packet.Extract<PacketDotNet.IcmpV4Packet>();
            IcmpV6Packet icmpV6Packet = packet.Extract<PacketDotNet.IcmpV6Packet>();
            //Console.WriteLine($"tcp: {tcpPacket}");
            //Console.WriteLine($"udp: {udpPacket}");
            //Console.WriteLine($"arp: {arpPacket}");
            //Console.WriteLine($"icmpV4: {icmpV4Packet}");
            //Console.WriteLine($"icmpV6: {icmpV6Packet}");

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

        OutputPacket GetArpPacket(ArpPacket arpPacket)
        {
            var ipPacket = (PacketDotNet.IPPacket)arpPacket.ParentPacket;
            Console.WriteLine(ipPacket);
            System.Net.NetworkInformation.PhysicalAddress srcIp = arpPacket.SenderHardwareAddress;
            System.Net.NetworkInformation.PhysicalAddress dstIp = arpPacket.TargetHardwareAddress;
            string srcPort = "no-port";
            string dstPort = "no-port";
            byte[] data = arpPacket.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            Console.WriteLine(7);
            return outputPacket;
        }

        OutputPacket GetIcmpV4Packet(IcmpV4Packet icmpV4Packet)
        {
            var ipPacket = (PacketDotNet.IPPacket)icmpV4Packet.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            string srcPort = "no-port";
            string dstPort = "no-port";
            byte[] data = icmpV4Packet.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }

        OutputPacket GetIcmpV6Packet(IcmpV6Packet icmpV6Packet)
        {
            var ipPacket = (PacketDotNet.IPPacket)icmpV6Packet.ParentPacket;
            System.Net.IPAddress srcIp = ipPacket.SourceAddress;
            System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
            string srcPort = "no-port";
            string dstPort = "no-port";
            byte[] data = icmpV6Packet.BytesSegment.Bytes;

            OutputPacket outputPacket = new OutputPacket(
                srcIp, srcPort, dstIp, dstPort, data
            );
            return outputPacket;
        }
    }
    public struct OutputPacket
    {
        public OutputPacket(object srcIp, object srcPort, object dstIp,
                            object dstPort, byte[] packetData, int length = 0, string time = "")
        {
            Time = time;
            SrcIp = srcIp.ToString();
            SrcPort = srcPort.ToString();
            DstIp = dstIp.ToString();
            DstPort = dstPort.ToString();
            Length = length;
            Data = packetData;
        }

        public string Time { get; set; }
        public string SrcIp { get; set; }
        public string SrcPort { get; set; }
        public string DstIp { get; set; }
        public string DstPort { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }

        public override string ToString() => String.Format(
            "{0} {1} : {2} > {3} : {4}, length {5} bytes\n{6}",
            Time, SrcIp, SrcPort, DstIp, DstPort, Length,
            Utils.ConvertPacketDataToOutputFormat(Data)
        );
    }
}
