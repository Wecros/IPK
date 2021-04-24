/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/22</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using System;

namespace PacketSniffer
{
    class PacketSnifferException : Exception
    {
        public PacketSnifferException()
        {
        }

        public PacketSnifferException(string message) : base(message)
        {
        }

        public PacketSnifferException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class InvalidArgException : PacketSnifferException
    {
        public InvalidArgException()
        {
        }

        public InvalidArgException(string message) : base(message)
        {
        }

        public InvalidArgException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class NoInterfaceFound : PacketSnifferException
    {
        public NoInterfaceFound()
        {
        }

        public NoInterfaceFound(string message) : base(message)
        {
        }

        public NoInterfaceFound(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
