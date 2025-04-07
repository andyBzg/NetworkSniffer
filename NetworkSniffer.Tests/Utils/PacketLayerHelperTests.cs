using NetworkSniffer.Utils;
using PacketDotNet;
using PacketDotNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSniffer.Tests.Utils
{
    public class PacketLayerHelperTests
    {
        private readonly PacketLayerHelper _packetLayerHelper;
        private readonly PhysicalAddress _senderMac;
        private readonly PhysicalAddress _targetMac;
        private readonly IPAddress _senderIp;
        private readonly IPAddress _targetIp;
        private readonly IPAddress _senderIpV6;
        private readonly IPAddress _targetIpV6;

        public PacketLayerHelperTests()
        {
            _packetLayerHelper = new PacketLayerHelper();
            _senderMac = new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 });
            _targetMac = new PhysicalAddress(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF });
            _senderIp = new IPAddress(new byte[] { 192, 168, 0, 1 });
            _targetIp = new IPAddress(new byte[] { 192, 168, 0, 2 });
            _senderIpV6 = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
            _targetIpV6 = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7335");
        }

        [Fact]
        public void GetLayerLevel_EthernetPacket_ReturnsDataLinkLayer()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("D", result);
        }

        [Fact]
        public void GetLayerLevel_ArpPacket_ReturnsDataLinkLayer()
        {
            // Arrange
            var packet = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);
            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);
            // Assert
            Assert.Equal("D", result);
        }

        [Fact]
        public void GetLayerLevel_IpV4Packet_ReturnsNetworkLayer()
        {
            // Arrange
            var packet = new IPv4Packet(_senderIp, _targetIp);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("N", result);
        }

        [Fact]
        public void GetLayerLevel_IpV6Packet_ReturnsNetworkLayer()
        {
            // Arrange
            var packet = new IPv6Packet(_senderIpV6, _targetIpV6);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("N", result);
        }

        [Fact]
        public void GetLayerLevel_TcpPacket_ReturnsTransportLayer()
        {
            // Arrange
            var packet = new TcpPacket(1234, 80);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("T", result);
        }

        [Fact]
        public void GetLayerLevel_UdpPacket_ReturnsTransportLayer()
        {
            // Arrange
            var packet = new UdpPacket(1234, 80);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("T", result);
        }

        [Fact]
        public void GetLayerLevel_DhcpV4Packet_ReturnsApplicationLayer()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            var rawData = new byte[240];
            var dhcpV4Packet = new DhcpV4Packet(new ByteArraySegment(rawData), ethernetPacket);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(dhcpV4Packet);

            // Assert
            Assert.Equal("A", result);
        }

        [Fact]
        public void GetLayerLevel_IcmpV4Packet_ReturnsNetworkLayer()
        {
            // Arrange
            byte[] icmpData = [
                0x08, 0x00,
                0xf7, 0xff,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            ];
            var segment = new ByteArraySegment(icmpData);
            var packet = new IcmpV4Packet(segment);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("N", result);
        }

        [Fact]
        public void GetLayerLevel_IcmpV6Packet_ReturnsNetworkLayer()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x00, 0x01, 0x02, 0x03
            };
            var segment = new ByteArraySegment(icmpData);
            var packet = new IcmpV6Packet(segment);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("N", result);
        }

        [Fact]
        public void GetLayerLevel_UnknownPacket_ReturnsUnknown()
        {
            // Arrange
            var rawData = new byte[] { 0x01, 0x02, 0x03 };
            var segment = new ByteArraySegment(rawData);
            var packet = new RawIPPacket(segment);

            // Act
            var result = _packetLayerHelper.GetLayerLevel(packet);

            // Assert
            Assert.Equal("Unknown Layer", result);
        }

        [Fact]
        public void GetLayerName_EthernetPacket_ReturnsEthernet()
        {
            // Arrange
            var packet = new EthernetPacket(_senderMac, _targetMac, EthernetType.Arp);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("Ethernet", result);
        }

        [Fact]
        public void GetLayerName_ArpPacket_ReturnsARP()
        {
            // Arrange
            var packet = new ArpPacket(ArpOperation.Request, _senderMac, _senderIp, _targetMac, _targetIp);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("ARP", result);
        }

        [Fact]
        public void GetLayerName_IpV4Packet_ReturnsIP()
        {
            // Arrange
            var packet = new IPv4Packet(_senderIp, _targetIp);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("IP", result);
        }

        [Fact]
        public void GetLayerName_IpV6Packet_ReturnsIP()
        {
            // Arrange
            var packet = new IPv6Packet(_senderIpV6, _targetIpV6);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("IP", result);
        }

        [Fact]
        public void GetLayerName_TcpPacket_ReturnsTCP()
        {
            // Arrange
            var packet = new TcpPacket(1234, 80);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("TCP", result);
        }

        [Fact]
        public void GetLayerName_UdpPacket_ReturnsUDP()
        {
            // Arrange
            var packet = new UdpPacket(1234, 80);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("UDP", result);
        }

        [Fact]
        public void GetLayerName_DhcpV4Packet_ReturnsDHCPv4()
        {
            // Arrange
            var ethernetPacket = new EthernetPacket(_senderMac, _targetMac, EthernetType.IPv4);
            var rawData = new byte[240];
            var dhcpV4Packet = new DhcpV4Packet(new ByteArraySegment(rawData), ethernetPacket);

            // Act
            var result = _packetLayerHelper.GetLayerName(dhcpV4Packet);

            // Assert
            Assert.Equal("DHCPv4", result);
        }

        [Fact]
        public void GetLayerName_IcmpV4Packet_ReturnsICMPv4()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x08, 0x00,
                0xf7, 0xff,
                0x00, 0x01, 0x00, 0x01,
                0xde, 0xad, 0xbe, 0xef
            };
            var segment = new ByteArraySegment(icmpData);
            var packet = new IcmpV4Packet(segment);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("ICMPv4", result);
        }

        [Fact]
        public void GetLayerName_IcmpV6Packet_ReturnsICMPv6()
        {
            // Arrange
            byte[] icmpData = new byte[] {
                0x00, 0x01, 0x02, 0x03
            };
            var segment = new ByteArraySegment(icmpData);
            var packet = new IcmpV6Packet(segment);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("ICMPv6", result);
        }

        [Fact]
        public void GetLayerName_UnknownPacket_ReturnsUnknown()
        {
            // Arrange
            var rawData = new byte[] { 0x01, 0x02, 0x03 };
            var segment = new ByteArraySegment(rawData);
            var packet = new RawIPPacket(segment);

            // Act
            var result = _packetLayerHelper.GetLayerName(packet);

            // Assert
            Assert.Equal("Unknown", result);
        }
    }
}
