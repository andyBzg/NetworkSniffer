using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class IcmpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public IcmpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<IcmpV4Packet>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var icmpPacket = packet.Extract<IcmpV4Packet>();
            if (icmpPacket != null)
            {
                _logger.Log(
                    "[N] [ICMP]".PadRight(15) +
                    $"Type {icmpPacket.TypeCode}" +
                    $" | Checksum {icmpPacket.Checksum}" +
                    $" | Identifier: {icmpPacket.Id}" +
                    $" | Seq No: {icmpPacket.Sequence}",
                    ConsoleColor.Magenta
                    );
            }
        }
    }
}
