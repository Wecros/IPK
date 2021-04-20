using System;

namespace PacketSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sniffing packets...");
            var argparse = new Parser();
            argparse.Parse();
        }
    }
}
