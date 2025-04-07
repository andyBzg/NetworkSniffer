using Moq;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class TcpPacketHandlerTests
    {
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly TcpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public TcpPacketHandlerTests()
        {
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new TcpPacketHandler(_mockPacketLayerHelper.Object);
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
        public void HandlePacket_ValidTcpPacket_PacketLayerHelperCalled()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(tcpPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(m => m.GetLayerLevel(tcpPacket), Times.Once);
            _mockPacketLayerHelper.Verify(m => m.GetLayerName(tcpPacket), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidTcpPacket_LogsCorrectMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80)
            {
                SequenceNumber = 1,
                AcknowledgmentNumber = 2,
                WindowSize = 8192,
                Checksum = 12345
            };
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(tcpPacket, logBuilder);

            // Assert
            var logOutput = logBuilder.Build();
            Assert.Contains("1234 -> 80", logOutput);
            Assert.Contains("Seq: 1", logOutput);
            Assert.Contains("Ack: 2", logOutput);
            Assert.Contains("Window: 8192", logOutput);
            Assert.Contains("Checksum: 12345", logOutput);
        }

        [Fact]
        public void HandlePacket_NonTcpPacket_DoesNotLogMessage()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(udpPacket, logBuilder);

            // Assert
            var logOutput = logBuilder.Build();
            Assert.Empty(logOutput);
        }
    }
}
