# Packet Sniffer -- IPK BUT FIT Summer Course 2021

## File structure (hand-in)

Makefile
PacketSniffer/
PacketSniffer/libs/
PacketSniffer/libs/ArgumentParser.cs
PacketSniffer/libs/Sniffer.cs
PacketSniffer/libs/Debug.cs
PacketSniffer/libs/Utils.cs
PacketSniffer/Program.cs
PacketSniffer/PacketSniffer.csproj
PacketSniffer.sln
PacketSniffer.Tests/
PacketSniffer.Tests/UnitTest1.cs
PacketSniffer.Tests/PacketSniffer.Tests.csproj
README.md

## Abstract

Network analyzer that catches and filters packets on a specific interface.
Depending on the provided arguments, either *TCP*, *UDP*, *ICMP* or *ARP*
number of packets are sniffed and info about them including byte data is printed
out to the `stdout` in a hex/ascii format.

## Implementation details

- RFC3339 Time is listed as UTC timezone.
- Information about ARP and ICMP packets is printed without their ports.
- ARP packet's IP address is printed out as hardware address instead.

### Return codes

- Success: 0
- Error: 1

### Code base

I try to follow guidelines I gained from reading Clean Code by Rober C. Martin.
Relevant `ipk-sniffer` source codes are located in the `PacketSniffer` directory.
Entry point is in `Program.cs`. Other file names should be self-explanatory.

Unit tests using *xUnit* are implemented in `PacketSniffer.Tests` directory,
primary testing the utils functions.

## Extensions

- Additional statistics are printed out when parsing is finished.
- Used filter is written out at the start of sniffing.
- IPv6 support.

## Restrictions

- Nothing I know of.

## Script usage

```
ipk-sniffer [options]

Options:
  -i, --interface <interface>  Interface to listen to. If empty or without argument,
                               print out all available interfaces.
  -p <p>                       Packets will be filtered by this port.
                               If empty, all ports are considered.
  -t, --tcp                    Filter packets by TCP.
  -u, --udp                    Filter packets by UDP.
  --icmp                       Filter packets by ICMPv4 and ICMPv6.
  --arp                        Filter packets by ARP frames.
  -n <n>                       Number of packets to display. If empty, only one packet is displayed.
  --version                    Show version information
  -?, -h, --help               Show help and usage information
```

### Examples

#### Calls

```
./ipk-sniffer -i eth0 -p 23 --tcp -n 2
./ipk-sniffer -i eth0 --udp
./ipk-sniffer -i eth0 -n 10
./ipk-sniffer -i eth0 -p 22 --tcp --udp --icmp --arp
./ipk-sniffer -i eth0 -p 22
./ipk-sniffer -i eth0
```

#### Output

```
./ipk-sniffer
./ipk-sniffer -i

lo0
eth0
```

```
./ipk-sniffer -i any --tcp

-- Sniffing on Pseudo-device that captures on all interfaces
-- using filter: tcp
2021-04-25T18:43:52Z 162.159.135.234 : 443 > 192.168.1.14 : 45048, length 316 bytes
0x0000: 00 00 00 01 00 06 98 f4  28 de b8 64 00 00 08 00 ........ (..d....
0x0010: 45 00 01 2c ef 97 40 00  3b 06 62 f4 a2 9f 87 ea E..,..@. ;.b.....
0x0020: c0 a8 01 0e 01 bb af f8  e3 0f 3b 5c 63 4c a7 11 ........ ..;\cL..
0x0030: 50 18 00 46 ee fa 00 00  17 03 03 00 ff c3 42 a1 P..F.... ......B.
0x0040: 74 24 2c 45 44 61 0c 67  5b b4 53 6f a3 ae 52 37 t$,EDa.g [.So..R7
0x0050: 2c 24 d3 47 07 84 7b 2d  66 38 36 40 1a 4c 5c 9c ,$.G..{- f86@.L\.
0x0060: fd 37 e8 6e 5b cb bd e7  fe 56 d2 48 e6 76 29 6f .7.n[... .V.H.v)o
0x0070: 37 3f 0d ea b9 a2 30 56  c2 c1 67 dd 76 37 79 28 7.....0V ..g.v7y(
0x0080: 9e 91 d7 e5 a6 97 31 df  06 15 b1 b7 04 f2 03 3f ......1. ........
0x0090: ea b3 89 a1 7f 3a 6c 47  7d fa 4f 61 dd 21 7c 2c .....:lG }.Oa.!|,
0x00a0: 06 f3 6c 48 17 5a ac ab  0c 90 61 85 7f 61 38 16 ..lH.Z.. ..a..a8.
0x00b0: a1 a5 3c 3a c1 e9 1f 01  b0 9f b3 02 cd 4e 0e 7a ..<:.... .....N.z
0x00c0: e2 e5 18 6a 05 e7 92 2d  10 59 aa 5e a6 a6 d9 76 ...j...- .Y.^...v
0x00d0: 3b 8f 7b e7 47 b8 b8 44  c7 d3 cd 08 f1 31 83 9b ;.{.G..D .....1..
0x00e0: cd 19 fe 20 3f c7 cb c8  5e ff a4 29 30 a5 96 58 ... .... ^..)0..X
0x00f0: 73 01 ef 19 04 0e 75 6b  5e 3a f1 94 66 48 b0 c2 s.....uk ^:..fH..
0x0100: 26 b2 16 dd ff 1f fa 88  43 f6 8f 51 b8 85 e1 7e &....... C..Q...~
0x0110: cb d3 e5 a3 cd 9d a6 47  1b 35 8d 98 5f 10 12 e7 .......G .5.._...
0x0120: 6a b3 f6 11 f6 16 ae da  d2 31 c0 5d 23 e7 dd ef j....... .1.]#...
0x0130: ed 09 05 a2 87 14 f8 8f  5d 63 96 03             ........ ]c..

[PcapStatistics: ReceivedPackets=22, DroppedPackets=0, InterfaceDroppedPackets=0]
-- Finished sniffing...
```

## Makefile targets (installation and development)

To build the final binary, just use `make`/`make publish` from the root
directory. The `ipk-sniffer` binary will be then placed in the root directory.

```
$ make help
make
    Execute 'make run' command.
make run
    Build and run the program.
make build
    Build the project. Built files are placed in bin/ and obj/ directories.
make test
    Build and test the project.
make pack
    Create a zip file with the project.
make clean
    Remove all unneeded files (e.g. binaries).
make format
    Format all of the C# source files using roslynator.
make analyze
    Analyze all of the C# source files statically using roslynator.
make analyze-fix
    Fix all of diagnostics found by analyzer using roslynator.
make install-roslynator
    Install roslynator tool using dotnet, used as linter/formatter.
make help
    Show this screen.
```
