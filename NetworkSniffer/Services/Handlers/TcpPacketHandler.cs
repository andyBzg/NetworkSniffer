using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class TcpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public TcpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<TcpPacket>() != null;
        }

        public void HandlePacket(Packet packet)
        {
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null)
            {
                _logger.Log($"[Transport Layer] TCP: {tcpPacket.SourcePort} -> {tcpPacket.DestinationPort} | Flags: {tcpPacket.Flags}", ConsoleColor.Green);
            }
        }
    }
}
