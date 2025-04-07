using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class IpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public IpPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<IPPacket>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var ipPacket = packet.Extract<IPPacket>();
            if (ipPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(ipPacket);
                var layerName = _packetLayerHelper.GetLayerName(ipPacket);
                logBuilder
                    .AddLayer(layerLevel, layerName,
                        $"{ipPacket.SourceAddress} -> {ipPacket.DestinationAddress}" +
                        $" | Protocol: {ipPacket.Protocol}" +
                        $" | TTL: {ipPacket.TimeToLive}" +
                        $" | Length: {ipPacket.TotalPacketLength}");
            }
        }
    }
}
