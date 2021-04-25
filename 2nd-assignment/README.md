# Packet Sniffer -- IPK BUT FIT Summer Course 2021

## Abstract

Network analyzer that catches and filters packets on specific interface.

## Implementation details

Time is listed as UTC timezone.

### Return codes

* Success: 0
* Error: 1

### Code base

I try to follow guidelines I gained from reading Clean Code by Rober C. Martin. 

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

## File structure (hand-in)

Makefile
PacketSniffer/
PacketSniffer/libs/
PacketSniffer/libs/ArgumentParser.cs
PacketSniffer/libs/Sniffer.cs
PacketSniffer/libs/Exceptions.cs
PacketSniffer/libs/Debug.cs
PacketSniffer/Program.cs
PacketSniffer/PacketSniffer.csproj
PacketSniffer.sln
PacketSniffer.Tests/
PacketSniffer.Tests/UnitTest1.cs
PacketSniffer.Tests/PacketSniffer.Tests.csproj
README.md

## Extensions

Additional statistics are printed out when parsing is finished.
Used filter is written out at the start of sniffing. IPv6 support.

## Restrictions

Nothing I know of.


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
