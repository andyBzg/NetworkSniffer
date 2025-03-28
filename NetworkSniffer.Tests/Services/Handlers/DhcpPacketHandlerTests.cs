using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class DhcpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly DhcpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public DhcpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new DhcpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void CanHandlePacket_ValidDhcpPacket_ReturnsTrue()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            var rawData = new byte[240];
            var dhcpV4Packet = new DhcpV4Packet(new ByteArraySegment(rawData), ethernetPacket);
            ethernetPacket.PayloadPacket = dhcpV4Packet;

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonDhcpPacket_ReturnsFalse()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidDhcpPacket_LogsCorrectMessage()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            byte[] rawData = new byte[244]; // DHCP header 240 bytes + 4 bytes of padding
            rawData[0] = 1;  // BOOTP Message Type (1 = Request)
            rawData[236] = 53; // DHCP Option 53 (DHCP Message Type)
            rawData[237] = 1;  // Length of DHCP Message Type
            rawData[238] = (byte)DhcpV4MessageType.Ack; // DHCP Message Type
            var dhcpV4Packet = new DhcpV4Packet(new ByteArraySegment(rawData), ethernetPacket);
            ethernetPacket.PayloadPacket = dhcpV4Packet;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), ConsoleColor.Yellow), Times.Once);
        }

        [Fact]
        public void HandlePacket_NonDhcpPacket_DoesNotLogMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }

        [Fact]
        public void HandlePacket_NullPacket_DoesNotLogMessage()
        {
            // Arrange
            EthernetPacket ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }
    }
}
