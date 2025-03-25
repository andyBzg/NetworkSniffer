using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class DhcpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public DhcpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<DhcpV4Packet>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var dhcpPacket = packet.Extract<DhcpV4Packet>();
            if (dhcpPacket != null)
            {
                _logger.Log(
                    "[A] [DHCP]".PadRight(15) +
                    $"Client: {dhcpPacket.ClientHardwareAddress}" +
                    $" | Requested IP: {dhcpPacket.YourAddress}" +
                    $" | Type: {dhcpPacket.MessageType}" +
                    ConsoleColor.Yellow
                    );
            }
        }
    }
}
