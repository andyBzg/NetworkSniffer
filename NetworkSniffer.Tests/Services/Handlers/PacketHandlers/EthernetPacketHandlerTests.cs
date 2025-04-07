using Moq;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class EthernetPacketHandlerTests
    {
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly EthernetPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;

        public EthernetPacketHandlerTests()
        {
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new EthernetPacketHandler(_mockPacketLayerHelper.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
            _senderIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 });
            _targetIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 2 });
        }

        [Fact]
        public void CanHandlePacket_ValidEthernetPacket_ReturnsTrue()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandleEthernetPacket_ReturnsFalse()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            // Act
            bool result = _handler.CanHandlePacket(arpPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidEthernetPacket_PacketLayerHelperCalled()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ethernetPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerLevel(It.IsAny<EthernetPacket>()), Times.Once);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerName(It.IsAny<EthernetPacket>()), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidEthernetPacket_LogsCorrectMessage()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ethernetPacket, logBuilder);

            // Assert
            var logMessage = logBuilder.Build();
            Assert.Contains($"{_senderMac} -> {_targetMac} | EtherType: {EthernetType.Arp}", logMessage);
        }

        [Fact]
        public void HandlePacket_NonEthernetPacket_DoesNotLog()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(arpPacket, logBuilder);

            // Assert
            var logMessage = logBuilder.Build();
            Assert.Empty(logMessage);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerLevel(It.IsAny<ArpPacket>()), Times.Never);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerName(It.IsAny<ArpPacket>()), Times.Never);
        }
    }
}
