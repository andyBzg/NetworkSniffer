using Moq;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class UdpPacketHandlerTests
    {
        //private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly UdpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public UdpPacketHandlerTests()
        {
            //_mockLogger = new Mock<ILogger>();
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new UdpPacketHandler(_mockPacketLayerHelper.Object);
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
        public void HandlePacket_ValidUdpPacket_PacketLayerHelperCalled()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(udpPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(m => m.GetLayerLevel(udpPacket), Times.Once);
            _mockPacketLayerHelper.Verify(m => m.GetLayerName(udpPacket), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidUdpPacket_LogsCorrectMessage()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(udpPacket, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Contains(
                $"{udpPacket.SourcePort} -> {udpPacket.DestinationPort}" +
                $" | Length: {udpPacket.Length} bytes" +
                $" | Checksum: {udpPacket.Checksum}",
                logMessage);
        }

        [Fact]
        public void HandlePacket_NonUdpPacket_DoesNotLog()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(tcpPacket, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Empty(logMessage);
        }
    }
}
