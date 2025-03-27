﻿using Moq;
using PacketDotNet;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class ArpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly ArpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly System.Net.IPAddress _senderIp;
        private readonly System.Net.IPAddress _targetIp;

        public ArpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new ArpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
            _senderIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 1 });
            _targetIp = new System.Net.IPAddress(new byte[] { 192, 168, 0, 2 });
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
        public void HandlePacket_ValidArpPacket_LogsCorrectMessage()
        {
            // Arrange
            var arpPacket = new ArpPacket(ArpOperation.Request, _targetMac, _senderIp, _senderMac, _targetIp);

            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);
            ethernetPacket.PayloadPacket = arpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()),
                Times.Once
                );
        }

        [Fact]
        public void HandlePacket_WithNullArpPacket_DoesNotLog()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);

            // Act
            _handler.HandlePacket(tcpPacket);

            // Assert
            _mockLogger.Verify(
                logger => logger.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()),
                Times.Never
                );
        }
    }
}
