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

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<ArpPacket>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var arpPacket = packet.Extract<ArpPacket>();
            if (arpPacket != null)
            {
                _logger.Log(
                    "[D] [ARP]".PadRight(15) +
                    $"{arpPacket.Operation}: " +
                    $"{arpPacket.SenderProtocolAddress} ({arpPacket.SenderHardwareAddress}) -> " +
                    $"{arpPacket.TargetProtocolAddress} ({arpPacket.SenderHardwareAddress})"
                    );
            }
        }
    }
}
