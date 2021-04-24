/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/24</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using SharpPcap;
using SharpPcap.LibPcap;

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

        public void Sniff()
        {
            int readTimeoutInMilliseconds = 5000;
            Interface.Open(DeviceMode.Promiscuous, readTimeoutInMilliseconds);

            Console.WriteLine($"-- Listening on {Interface.Description}");

            RawCapture packet;

            while ((packet = Interface.GetNextPacket()) != null &&
                    ProcessedPacketCount != ProgramArgs.PacketCountToDisplay)
            {
                DateTime time = packet.Timeval.Date;
                int len = packet.Data.Length;
                Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                    time.Hour, time.Minute, time.Second,
                    time.Millisecond, len);
            }

            Console.WriteLine(Interface.Statistics.ToString());

            Console.WriteLine("Pres 'enter' to stop listening...");
            Console.ReadLine();
        }
    }
}
