using PacketDotNet;

namespace NetworkSniffer.Utils
{
    internal class PacketLayerHelper : IPacketLayerHelper
    {
        public string GetLayerLevel(Packet packet)
        {
            return packet switch
            {
                EthernetPacket => "D", // Data Link Layer
                ArpPacket => "D", // Data Link Layer
                IPPacket => "N", // Network Layer
                IcmpV4Packet => "N", // Network Layer
                IcmpV6Packet => "N", // Network Layer
                TcpPacket => "T", // Transport Layer
                UdpPacket => "T", // Transport Layer
                DhcpV4Packet => "A", // Application Layer
                _ => "Unknown Layer"
            };
        }

        public string GetLayerName(Packet packet)
        {
            return packet switch
            {
                EthernetPacket => "Ethernet",
                ArpPacket => "ARP",
                IPPacket => "IP",
                IcmpV4Packet => "ICMPv4",
                IcmpV6Packet => "ICMPv6",
                TcpPacket => "TCP",
                UdpPacket => "UDP",
                DhcpV4Packet => "DHCPv4",
                _ => "Unknown"
            };
        }
    }
}
