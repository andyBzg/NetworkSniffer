using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class HttpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly HttpPacketHandler _handler;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;

        public HttpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new HttpPacketHandler(_mockLogger.Object);
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
        }

        [Fact]
        public void CanHandlePacket_ValidHttpPacket_ReturnsTrue()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            tcpPacket.PayloadData = Encoding.UTF8.GetBytes("GET / HTTP/1.1");
            
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePacket_NonHttpPacket_ReturnsFalse()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = udpPacket;

            // Act
            bool result = _handler.CanHandlePacket(ethernetPacket);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePacket_ValidHttpPacket_LogsCorrectMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            tcpPacket.PayloadData = Encoding.UTF8.GetBytes("GET / HTTP/1.1");
            
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), ConsoleColor.Yellow), Times.Once);
        }

        [Fact]
        public void HandlePacket_NonHttpPacket_DoesNotLogMessage()
        {
            // Arrange
            var udpPacket = new UdpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = udpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }

        [Fact]
        public void HandlePacket_HttpPacketWithEmptyPayload_DoesNotLogMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            tcpPacket.PayloadData = new byte[0];

            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }

        [Fact]
        public void HandlePacket_HttpPacketWithNullPayload_DoesNotLogMessage()
        {
            // Arrange
            var tcpPacket = new TcpPacket(1234, 80);
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            ethernetPacket.PayloadPacket = tcpPacket;

            // Act
            _handler.HandlePacket(ethernetPacket);

            // Assert
            _mockLogger.Verify(l => l.Log(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.Never);
        }
    }
}
