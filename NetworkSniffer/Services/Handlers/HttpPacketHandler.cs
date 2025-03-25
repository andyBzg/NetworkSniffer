using NetworkSniffer.Interfaces;
using PacketDotNet;
using System.Text;

namespace NetworkSniffer.Services.Handlers
{
    internal class HttpPacketHandler : IPacketHandler
    {
        private readonly ILogger _logger;

        public HttpPacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<TcpPacket>()?.PayloadData != null;
        }

        public void HandlePacket(Packet packet)
        {
            DateTime.Now.ToString("HH:mm:ss");
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null)
            {
                var httpPayload = Encoding.UTF8.GetString(tcpPacket.PayloadData);
                if (httpPayload.Contains("HTTP"))
                {
                    _logger.Log(
                        $"[{DateTime.Now:HH:mm:ss.fff}] [A] [HTTP]".PadRight(30) +
                        $"Payload: {httpPayload}",
                        ConsoleColor.Yellow
                        );
                }
            }
        }
    }
}
