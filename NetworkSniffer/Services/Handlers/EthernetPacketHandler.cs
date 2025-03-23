using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class EthernetPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public EthernetPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void HandlePacket(Packet packet)
        {
            var ethPacket = packet.Extract<EthernetPacket>();
            if (ethPacket != null)
            {
                _logger.Log($"Ethernet: {ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress} | Type: {ethPacket.Type}");
            }

            // Extract ARP and IP packets
            var arpPacket = packet.Extract<ArpPacket>();
            if (arpPacket != null)
            {
                _logger.Log($"ARP: {arpPacket.SenderHardwareAddress} -> {arpPacket.TargetHardwareAddress}", ConsoleColor.Yellow);
            }

            var ipPacket = packet.Extract<IPPacket>();
            if (ipPacket != null)
            {
                _logger.Log($"IP: {ipPacket.SourceAddress} -> {ipPacket.DestinationAddress} | Protocol: {ipPacket.Protocol}");
                HandleIpPacket(ipPacket);
            }
        }

        private void HandleIpPacket(IPPacket ipPacket)
        {
            // Check the protocol of the IP packet
            switch (ipPacket.Protocol)
            {
                case ProtocolType.Icmp:
                    var icmpPacket = ipPacket.Extract<IcmpV4Packet>();
                    if (icmpPacket != null)
                    {
                        _logger.Log($"ICMP: Type {icmpPacket.TypeCode} | Checksum {icmpPacket.Checksum}", ConsoleColor.Magenta);
                    }
                    break;
                case ProtocolType.Tcp:
                    var tcpPacket = ipPacket.Extract<TcpPacket>();
                    if (tcpPacket != null)
                    {
                        _logger.Log($"TCP: {tcpPacket.SourcePort} -> {tcpPacket.DestinationPort} | Flags: {tcpPacket.Flags}", ConsoleColor.Green);
                    }
                    break;
                case ProtocolType.Udp:
                    var udpPacket = ipPacket.Extract<UdpPacket>();
                    if (udpPacket != null)
                    {
                        _logger.Log($"UDP: {udpPacket.SourcePort} -> {udpPacket.DestinationPort}", ConsoleColor.Cyan);
                    }
                    break;
                default:
                    _logger.Log("Unknown protocol", ConsoleColor.Red);
                    break;
            }
        }
    }
}
