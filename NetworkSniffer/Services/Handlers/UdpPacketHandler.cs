using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class UdpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public UdpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<UdpPacket>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var udpPacket = packet.Extract<UdpPacket>();
            if (udpPacket != null)
            {
                _logger.Log($"[Transport Layer] UDP: {udpPacket.SourcePort} -> {udpPacket.DestinationPort}", ConsoleColor.Cyan);
            }
        }
    }
}
