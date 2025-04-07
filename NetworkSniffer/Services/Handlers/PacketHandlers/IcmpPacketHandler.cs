using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class IcmpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public IcmpPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<IcmpV4Packet>() != null || packet.Extract<IcmpV6Packet>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var icmpV4 = packet.Extract<IcmpV4Packet>();
            if (icmpV4 != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(icmpV4);
                var layerName = _packetLayerHelper.GetLayerName(icmpV4);
                logBuilder.AddLayer(layerLevel, layerName,
                        $"Type: {icmpV4.TypeCode}" +
                        $" | Checksum: {icmpV4.Checksum}" +
                        $" | Identifier: {icmpV4.Id}" +
                        $" | Seq No: {icmpV4.Sequence}");
            }

            var icmpV6 = packet.Extract<IcmpV6Packet>();
            if (icmpV6 != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(icmpV6);
                var layerName = _packetLayerHelper.GetLayerName(icmpV6);
                logBuilder.AddLayer(layerLevel, layerName,
                        $"Type: {icmpV6.Type}" +
                        $" | Code: {icmpV6.Code}" +
                        $" | Checksum: {icmpV6.Checksum}");
            }
        }
    }
}
