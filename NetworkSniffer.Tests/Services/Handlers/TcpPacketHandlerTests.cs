using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class TcpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly TcpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public TcpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new TcpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void CanHandlePacket_ValidTcpPacket_ReturnsTrue()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);

            // Act
            bool result = _handler.CanHandlePacket(tcpPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonTcpPacket_ReturnsFalse()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);

            // Act
            bool result = _handler.CanHandlePacket(udpPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidTcpPacket_LogsCorrectMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80)
            {
                SequenceNumber = 123456,
                AcknowledgmentNumber = 654321,
                WindowSize = 1234,
                Checksum = 1234
            };
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), ConsoleColor.Green), Times.Once);
        }

        [Fact]
        public void HandlePacket_NonTcpPacket_DoesNotLog()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _senderMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = udpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }

        [Fact]
        public void HandlePacket_NullTcpPacket_DoesNotLog()
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
