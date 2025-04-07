using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class TcpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public TcpPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<TcpPacket>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(tcpPacket);
                var layerName = _packetLayerHelper.GetLayerName(tcpPacket);
                logBuilder.AddLayer(layerLevel, layerName,
                    $"{tcpPacket.SourcePort} -> {tcpPacket.DestinationPort}" +
                    $" | Seq: {tcpPacket.SequenceNumber}" +
                    $" | Ack: {tcpPacket.AcknowledgmentNumber}" +
                    $" | Flags: {tcpPacket.Flags}" +
                    $" | Window: {tcpPacket.WindowSize}" +
                    $" | Checksum: {tcpPacket.Checksum}");
            }
        }
    }
}
