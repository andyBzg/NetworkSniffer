using Moq;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;
using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers.PacketHandlers
{
    public class IcmpPacketHandlerTests
    {
        //private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IPacketLayerHelper> _mockPacketLayerHelper;
        private readonly IcmpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;

        public IcmpPacketHandlerTests()
        {
            //_mockLogger = new Mock<ILogger>();
            _mockPacketLayerHelper = new Mock<IPacketLayerHelper>();
            _handler = new IcmpPacketHandler(_mockPacketLayerHelper.Object);
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
        public void HandlePacket_ValidIcmpPacket_PacketLayerHelperCalled()
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
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(icmpPacket, logBuilder);

            // Assert
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerLevel(icmpPacket), Times.Once);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerName(icmpPacket), Times.Once);
        }

        [Fact]
        public void HandlePacket_ValidIcmpV4Packet_BuildsCorrectLogMessage()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x08, 0x00,
                0xf7, 0xff,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            };
            var segment = new ByteArraySegment(icmpData);
            var icmpV4 = new IcmpV4Packet(segment);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(icmpV4, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Contains(
                $"Type: {icmpV4.TypeCode}" +
                $" | Checksum: {icmpV4.Checksum}" +
                $" | Identifier: {icmpV4.Id}" +
                $" | Seq No: {icmpV4.Sequence}", logMessage);
        }

        [Fact]
        public void HandlePacket_ValidIcmpV6Packet_BuildsCorrectLogMessage()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x80, 0x00,
                0x00, 0x00,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            };
            var segment = new ByteArraySegment(icmpData);
            var icmpV6 = new IcmpV6Packet(segment);
            var logBuilder = new PacketLogBuilder();

            // Act
            _handler.HandlePacket(icmpV6, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Contains(
                $"Type: {icmpV6.Type}" +
                $" | Code: {icmpV6.Code}" +
                $" | Checksum: {icmpV6.Checksum}", logMessage);
        }

        [Fact]
        public void HandlePacket_InvalidIcmpPacket_DoesNotCallPacketLayerHelper()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            var logBuilder = new PacketLogBuilder();
            // Act
            _handler.HandlePacket(arpPacket, logBuilder);
            // Assert
            string logMessage = logBuilder.Build();
            Assert.Empty(logMessage);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerLevel(It.IsAny<ArpPacket>()), Times.Never);
            _mockPacketLayerHelper.Verify(helper => helper.GetLayerName(It.IsAny<ArpPacket>()), Times.Never);
        }
    }
}
