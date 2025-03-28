using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class UdpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly UdpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public UdpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new UdpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void CanHandlePacket_ValidUdpPacket_ReturnsTrue()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            // Act
            bool result = _handler.CanHandlePacket(udpPacket);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonUdpPacket_ReturnsFalse()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            // Act
            bool result = _handler.CanHandlePacket(tcpPacket);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidUdpPacket_LogsCorrectMessage()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = udpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), ConsoleColor.Green), Times.Once);
        }

        [Fact]
        public void HandlePacket_NonUdpPacket_DoesNotLog()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _senderMac, EthernetType.IPv4);

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }

        [Fact]
        public void HandlePacket_NullUdpPacket_DoesNotLog()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _senderMac, EthernetType.IPv4);

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }
    }
}
