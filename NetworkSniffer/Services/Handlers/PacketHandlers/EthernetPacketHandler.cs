using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class EthernetPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public EthernetPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<EthernetPacket>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var ethPacket = packet.Extract<EthernetPacket>();
            if (ethPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(ethPacket);
                var layerName = _packetLayerHelper.GetLayerName(ethPacket);
                logBuilder
                    .AddLayer(layerLevel, layerName,
                        $"{ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress}" +
                        $" | EtherType: {ethPacket.Type}");
            }
        }
    }
}
