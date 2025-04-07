using Moq;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class IpPacketHandlerTests
    {
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly IpPacketHandler _handler;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public IpPacketHandlerTests()
        {
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new IpPacketHandler(_mockPacketLayerHelper.Object);
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
        public void HandlePacket_ValidIpPacket_PacketLayerHelperCalled()
        {
            // Arrange
            var ipPacket = new IPv4Packet(_senderIp, _targetIp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ipPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerLevel(ipPacket), Times.Once);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerName(ipPacket), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidIpPacket_LogsCorrectMessage()
        {
            // Arrange
            var ipPacket = new IPv4Packet(_senderIp, _targetIp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(ipPacket, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            var expectedMessage =
                $"{ipPacket.SourceAddress} -> {ipPacket.DestinationAddress}" +
                $" | Protocol: {ipPacket.Protocol}" +
                $" | TTL: {ipPacket.TimeToLive}" +
                $" | Length: {ipPacket.TotalPacketLength}";
            Assert.Contains(expectedMessage, logMessage);
        }

        [Fact]
        public void HandlePacket_NonIpPacket_DoesNotLogMessage()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(arpPacket, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Empty(logMessage);
        }
    }
}
