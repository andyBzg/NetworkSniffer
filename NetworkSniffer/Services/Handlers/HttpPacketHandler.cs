using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
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

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null && tcpPacket.HasPayloadData)
            {
                var httpPayload = Encoding.UTF8.GetString(tcpPacket.PayloadData);
                //var layer = PacketLayerHelper.GetLayerLevel(tcpPacket);
                if (httpPayload.Contains("HTTP"))
                {
                    logBuilder.AddLayer("A", "HTTP", httpPayload);
                    /*_logger.Log(
                        $"[{DateTime.Now:HH:mm:ss.fff}] [A] [HTTP]".PadRight(30) +
                        $"Payload: {httpPayload}",
                        ConsoleColor.Yellow
                        );*/
                }
            }
        }
    }
}
