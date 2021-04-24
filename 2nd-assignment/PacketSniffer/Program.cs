/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.Linq;

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

            // Exit if no interface has been specified
            if (String.IsNullOrEmpty(programArgs.Ifname))
            {
                return Code.Success;
            }

            var interf = interfaceList.Where(i => i.Name == programArgs.Ifname).FirstOrDefault();
            Sniffer sniffer = new Sniffer(interf, ArgumentParser.programArgs);

            sniffer.Sniff();

            Console.WriteLine(programArgs);
            Console.WriteLine(interf);

            return Code.Success;
        }
    }
}
