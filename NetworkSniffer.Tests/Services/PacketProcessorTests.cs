using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services;
using NetworkSniffer.Utils;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services
{
    public class PacketProcessorTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IPacketHandler> _packetHandlerMock;
        private readonly Mock<IPacketHandler> _secondPacketHandlerMock;
        private readonly IEnumerable<IPacketHandler> _packetHandlers;
        private readonly PacketProcessor _packetProcessor;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public PacketProcessorTests()
        {
            _mockLogger = new Mock<ILogger>();
            _packetHandlerMock = new Mock<IPacketHandler>();
            _secondPacketHandlerMock = new Mock<IPacketHandler>();
            _packetHandlers = new List<IPacketHandler> { _packetHandlerMock.Object, _secondPacketHandlerMock.Object };
            _packetProcessor = new PacketProcessor(_packetHandlers, _mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void ProcessPacket_HandlerCanHandlePacket_ShouldCallHandlePacket()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            _packetHandlerMock.Setup(h => h.CanHandlePacket(packet)).Returns(true);

            // Act
            _packetProcessor.ProcessPacket(packet);

            // Assert
            _packetHandlerMock.Verify(h => h.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Once);
        }

        [Fact]
        public void ProcessPacket_MultipleHandlers_AllCalled()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            _packetHandlerMock.Setup(h => h.CanHandlePacket(packet)).Returns(true);
            _secondPacketHandlerMock.Setup(h2 => h2.CanHandlePacket(packet)).Returns(true);

            // Act
            _packetProcessor.ProcessPacket(packet);

            // Assert
            _packetHandlerMock.Verify(h => h.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Once);
            _secondPacketHandlerMock.Verify(h2 => h2.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Once);
        }

        [Fact]
        public void ProcessPacket_HandlerCannotHandlePacket_ShouldLog()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            _packetHandlerMock.Setup(h => h.CanHandlePacket(packet)).Returns(false);

            // Act
            _packetProcessor.ProcessPacket(packet);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Once);
            _packetHandlerMock.Verify(h => h.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Never);
        }

        [Fact]
        public void ProcessPacket_FirstHandlerCannotHandle_SecondHandlerProcessesPacket()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            _packetHandlerMock.Setup(h => h.CanHandlePacket(packet)).Returns(false);
            _secondPacketHandlerMock.Setup(h2 => h2.CanHandlePacket(packet)).Returns(true);

            // Act
            _packetProcessor.ProcessPacket(packet);

            // Assert
            _packetHandlerMock.Verify(h => h.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Never);
            _secondPacketHandlerMock.Verify(h2 => h2.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Once);
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Once);
        }

        [Fact]
        public void ProcessPacket_NoHandlersCanHandle_ShouldLogUnhandledPacket()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            _packetHandlerMock.Setup(h => h.CanHandlePacket(packet)).Returns(false);
            _secondPacketHandlerMock.Setup(h2 => h2.CanHandlePacket(packet)).Returns(false);

            // Act
            _packetProcessor.ProcessPacket(packet);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Once);
            _packetHandlerMock.Verify(h => h.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Never);
            _secondPacketHandlerMock.Verify(h2 => h2.HandlePacket(packet, It.IsAny<PacketLogBuilder>()), Times.Never);
        }
    }
}
