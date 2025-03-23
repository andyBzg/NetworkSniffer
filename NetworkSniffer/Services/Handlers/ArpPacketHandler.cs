using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class ArpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public ArpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void HandlePacket(Packet packet)
        {
            var arpPacket = packet.Extract<ArpPacket>();
            if (arpPacket != null)
            {
                _logger.Log($"ARP: {arpPacket.SenderHardwareAddress} -> {arpPacket.TargetHardwareAddress}", ConsoleColor.Yellow);
            }
        }
    }
}
