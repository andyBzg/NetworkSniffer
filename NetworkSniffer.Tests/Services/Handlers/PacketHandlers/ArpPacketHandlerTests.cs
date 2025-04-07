using Moq;
using PacketDotNet;
using System.Net.NetworkInformation;
using System.Net;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class ArpPacketHandlerTests
    {
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly ArpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly IPAddress _senderIp;
        private readonly IPAddress _targetIp;

        public ArpPacketHandlerTests()
        {
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new ArpPacketHandler(_mockPacketLayerHelper.Object);
            _senderMac = new PhysicalAddress([0x00, 0x11, 0x22, 0x33, 0x44, 0x55]);
            _targetMac = new PhysicalAddress([0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF]);
            _senderIp = new IPAddress([192, 168, 0, 1]);
            _targetIp = new IPAddress([192, 168, 0, 2]);
        }

        [Fact]
        public void CanHandlePacket_ValidArpPacket_ReturnsTrue()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpPacket;

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonArpPacket_ReturnsFalse()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);

            // Act
            bool result = _handler.CanHandlePacket(tcpPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidArpPacket_PacketLayerHelperCalled()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpPacket;
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ethernetPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(h => h.GetLayerLevel(arpPacket), Times.Once);
            _mockPacketLayerHelper.Verify(h => h.GetLayerName(arpPacket), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidArpPacket_LogsCorrectMessage()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpPacket;
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ethernetPacket, logBuilder);

            // Assert
            var logMessage = logBuilder.Build();
            Assert.Contains(
                $"{arpPacket.Operation} | " +
                $"{arpPacket.SenderProtocolAddress} ({arpPacket.SenderHardwareAddress}) -> " +
                $"{arpPacket.TargetProtocolAddress} ({arpPacket.TargetHardwareAddress})", logMessage
                );
        }

        [Fact]
        public void HandlePacket_NonArpPacket_DoesNotLog()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(tcpPacket, logBuilder);

            // Assert
            var logMessage = logBuilder.Build();
            Assert.Empty(logMessage);
            _mockPacketLayerHelper.Verify(h => h.GetLayerLevel(It.IsAny<TcpPacket>()), Times.Never);
            _mockPacketLayerHelper.Verify(h => h.GetLayerName(It.IsAny<TcpPacket>()), Times.Never);
        }
    }
}
