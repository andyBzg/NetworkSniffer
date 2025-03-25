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
                _logger.Log(
                    $"[{DateTime.Now:HH:mm:ss.fff}] [T] [TCP]".PadRight(30) +
                    $"{tcpPacket.SourcePort} -> {tcpPacket.DestinationPort}" +
                    $" | Seq: {tcpPacket.SequenceNumber}" +
                    $" | Ack: {tcpPacket.AcknowledgmentNumber}" +
                    $" | Flags: {tcpPacket.Flags}" +
                    $" | Window: {tcpPacket.WindowSize}" +
                    $" | Checksum: {tcpPacket.Checksum}",
                    ConsoleColor.Green
                    );
            }
        }
    }
}
