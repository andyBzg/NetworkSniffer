using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class IpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public IpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<IPPacket>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var ipPacket = packet.Extract<IPPacket>();
            if (ipPacket != null)
            {
                _logger.Log(
                    $"[{DateTime.Now:HH:mm:ss.fff}] [N] [IP]".PadRight(30) +
                    $"{ipPacket.SourceAddress} -> {ipPacket.DestinationAddress}".PadRight(35) +
                    $" | Protocol: {ipPacket.Protocol}" +
                    $" | TTL: {ipPacket.TimeToLive}" +
                    $" | Length: {ipPacket.TotalPacketLength}",
                    ConsoleColor.Blue
                    );
            }
        }
    }
}
