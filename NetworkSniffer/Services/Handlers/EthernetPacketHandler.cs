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
                _logger.Log(
                    "[D] [Ethernet]".PadRight(15) +
                    $"{ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress}".PadRight(35) +
                    $" | EtherType: {ethPacket.Type}"
                    );
            }
        }
    }
}
