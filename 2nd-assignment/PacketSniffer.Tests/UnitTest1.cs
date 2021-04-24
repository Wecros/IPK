/// <author>Marek "Wecros" Filip (xfilip46)</author>
/// <date>2021/04/21</date>
/// <summary>IPK BUT FIT Packet Sniffer 2021</summary>

using Xunit;

namespace PacketSniffer.Tests
{
    public class ArgumentParserTests
    {
        [Theory]
        [InlineData("", 23, true, false, true, false, 5)]
        [InlineData("", 0, true, true, true, true, 0)]
        [InlineData("", 23, false, true, false, false, 5)]
        public void HandleArguments_CorrectData_ProgramArgumentsCorrectlyInitialized(string i, int p, bool tcp, bool udp, bool icmp, bool arp, int n)
        {
            ArgumentParser.HandleArgs(i, p, tcp, udp, icmp, arp, n);
            var programArgs = ArgumentParser.programArgs;

            Assert.Equal(i, programArgs.Ifname);
            Assert.Equal(p, programArgs.PortNumber);
            Assert.Equal(tcp, programArgs.Tcp);
            Assert.Equal(udp, programArgs.Udp);
            Assert.Equal(icmp, programArgs.Icmp);
            Assert.Equal(arp, programArgs.Arp);
            Assert.Equal(n, programArgs.PacketCountToDisplay);
        }

        [Fact]
        public void HandleArguments_NoBoolArgs_ProgramArgumentsCorrectlyInitialized() {
            string i = "";
            int p = 42;
            bool tcp = false;
            bool udp = false;
            bool icmp = false;
            bool arp = false;
            int n = 30;
            ArgumentParser.HandleArgs(i, p, tcp, udp, icmp, arp, n);

            var programArgs = ArgumentParser.programArgs;
            Assert.Equal(i, programArgs.Ifname);
            Assert.Equal(p, programArgs.PortNumber);
            Assert.True(programArgs.Tcp);
            Assert.True(programArgs.Udp);
            Assert.True(programArgs.Icmp);
            Assert.True(programArgs.Arp);
            Assert.Equal(n, programArgs.PacketCountToDisplay);

        }
    }
}
