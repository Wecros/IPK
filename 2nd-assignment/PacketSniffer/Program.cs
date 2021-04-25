/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;
using System.Linq;
using SharpPcap;

namespace PacketSniffer
{
    /// <summary>
    /// Class holding the possible return codes.
    /// 0: success,
    /// 1: error.
    /// </summary>
    static class Code
    {
        public const int Success = 0;
        public const int Error = 1;
    }

    static class Program
    {
        /// <summary>
        /// Script's entry point.
        /// </summary>
        static int Main(string[] args)
        {
            try
            {
                return StartProgram(args);
            }
            catch (Exception)
            {
                Debug.ErrorExit(Code.Error, "Could not sniff packets, please try different interface " +
                                            "or filter combination");
            }
            return Code.Success;
        }

        static int StartProgram(string[] args)
        {
            ArgumentParser.Parse(args);

            var programArgs = ArgumentParser.programArgs;
            var interfaceList = ArgumentParser.interfaceList;

            // Exit if no interface has been specified
            if (String.IsNullOrEmpty(programArgs.Ifname))
            {
                return Code.Success;
            }

            var interf = GetInterfaceByName(interfaceList, programArgs.Ifname);
            Sniffer sniffer = new Sniffer(interf, ArgumentParser.programArgs);

            sniffer.Sniff();

            return Code.Success;
        }

        static ICaptureDevice GetInterfaceByName(CaptureDeviceList interfaceList, string ifname) {
            var interf = interfaceList.Where(i => i.Name == ifname).FirstOrDefault();
            return interf;
        }
    }
}
