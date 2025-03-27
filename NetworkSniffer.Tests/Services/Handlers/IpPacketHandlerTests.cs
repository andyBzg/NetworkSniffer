using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class IpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly IpPacketHandler _handler;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public IpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new IpPacketHandler(_mockLogger.Object);
            _senderIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 });
            _targetIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 2 });
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void CanHandlePacket_ValidIpPacket_ReturnsTrue()
        {
            // Arrange
            var ipPacket = new IPv4Packet(_senderIp, _targetIp);

            // Act
            bool result = _handler.CanHandlePacket(ipPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonIpPacket_ReturnsFalse()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            // Act
            bool result = _handler.CanHandlePacket(arpPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidIpPacket_LogsCorrectMessage()
        {
            // Arrange
            var ipPacket = new IPv4Packet(_senderIp, _targetIp);

            // Act
            _handler.HandlePacket(ipPacket);

            // Assert
            _mockLogger.Verify(logger => logger.Log(It.IsAny<string>(), ConsoleColor.Blue), Times.Once);
        }

        [Fact]
        public void HandlePacket_InvalidIpPacket_DoesNotLogMessage()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            // Act
            _handler.HandlePacket(arpPacket);

            // Assert
            _mockLogger.Verify(logger => logger.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }
    }
}
