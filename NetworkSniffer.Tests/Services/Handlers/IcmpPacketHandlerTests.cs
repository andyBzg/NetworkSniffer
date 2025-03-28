using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class IcmpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly IcmpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;

        public IcmpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new IcmpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
            _senderIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 });
            _targetIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 2 });
        }

        [Fact]
        public void CanHandlePacket_ValidIcmpPacket_ReturnsTrue()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x08, 0x00,
                0xf7, 0xff,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            };
            var segment = new ByteArraySegment(icmpData);
            var icmpPacket = new IcmpV4Packet(segment);

            // Act
            bool result = _handler.CanHandlePacket(icmpPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_InvalidIcmpPacket_ReturnsFalse()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            // Act
            bool result = _handler.CanHandlePacket(arpPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidIcmpPacket_LogsCorrectMessage()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x08, 0x00,
                0xf7, 0xff,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            };
            var segment = new ByteArraySegment(icmpData);
            var icmpPacket = new IcmpV4Packet(segment);

            // Act
            _handler.HandlePacket(icmpPacket);

            // Assert
            _mockLogger.Verify(logger => logger.Log(It.Is<string>(s => s.Contains("ICMP")), ConsoleColor.Magenta), Times.Once);
        }

        [Fact]
        public void HandlePacket_InvalidIcmpPacket_DoesNotLogMessage()
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
