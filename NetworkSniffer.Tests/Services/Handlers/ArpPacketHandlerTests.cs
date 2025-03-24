using Moq;
using PacketDotNet;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class ArpPacketHandlerTests
    {
        [Fact]
        public void HandlePacket_ValidArpPacket_LogsCorrectMessage()
        {
            // Arrnge
            var mockLogger = new Mock<ILogger>();
            var handler = new ArpPacketHandler(mockLogger.Object);

            // Create an ARP packet
            var senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            var targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });

            var arpPacket = new ArpPacket(
                ArpOperation.Request,
                targetMac,
                new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 }),
                senderMac,
                new System.Net.IPAddress(new byte[] { 192, 168, 0, 2 })
                );

            // Wrap the ARP packet in an Ethernet packet
            var ethernetPacket = new EthernetPacket(senderMac, targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpPacket;

            // Act
            handler.HandlePacket(ethernetPacket);

            // Assert
            mockLogger.Verify(
                logger => logger.Log($"[Network Layer] ARP: {senderMac} -> {targetMac}", ConsoleColor.Yellow),
                Times.Once
                );
        }

        [Fact]
        public void HandlePacket_WithNullArpPacket_DoesNotLog()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var handler = new ArpPacketHandler(mockLogger.Object);

            // Create an Ethernet packet with no ARP payload
            var tcpPacket = new TcpPacket(1234, 80);

            // Act
            handler.HandlePacket(tcpPacket);

            // Assert
            mockLogger.Verify(
                logger => logger.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()),
                Times.Never
                );
        }
    }
}
