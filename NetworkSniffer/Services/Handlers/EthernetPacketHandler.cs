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

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<EthernetPacket>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var ethPacket = packet.Extract<EthernetPacket>();
            if (ethPacket != null)
            {
                _logger.Log($"[Link Layer] Ethernet: {ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress} | Type: {ethPacket.Type}");
            }
        }
    }
}
