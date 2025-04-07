using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class UdpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;
        private readonly IPayloadProcessor _payloadProcessor;

        public UdpPacketHandler(IPacketLayerHelper packetLayerHelper, IPayloadProcessor payloadProcessor)
        {
            _packetLayerHelper = packetLayerHelper;
            _payloadProcessor = payloadProcessor;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<UdpPacket>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var udpPacket = packet.Extract<UdpPacket>();
            if (udpPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(udpPacket);
                var layerName = _packetLayerHelper.GetLayerName(udpPacket);
                logBuilder.AddLayer(layerLevel, layerName,
                        $"{udpPacket.SourcePort} -> {udpPacket.DestinationPort}" +
                        $" | Length: {udpPacket.Length} bytes" +
                        $" | Checksum: {udpPacket.Checksum}");

                if (udpPacket.HasPayloadData && udpPacket.PayloadData.Length > 0)
                {
                    _payloadProcessor.ProcessPayload(udpPacket.PayloadData, logBuilder);
                }
            }

        }
    }
}
