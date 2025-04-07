using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers.PacketHandlers
{
    internal class ArpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public ArpPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<ArpPacket>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var arpPacket = packet.Extract<ArpPacket>();
            if (arpPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(arpPacket);
                var layerName = _packetLayerHelper.GetLayerName(arpPacket);
                logBuilder
                    .AddLayer(layerLevel, layerName,
                        $"{arpPacket.Operation} | " +
                        $"{arpPacket.SenderProtocolAddress} ({arpPacket.SenderHardwareAddress}) -> " +
                        $"{arpPacket.TargetProtocolAddress} ({arpPacket.TargetHardwareAddress})");
            }
        }
    }
}
