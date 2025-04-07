using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services.Handlers
{
    internal class DhcpPacketHandler : IPacketHandler
    {
        private readonly IPacketLayerHelper _packetLayerHelper;

        public DhcpPacketHandler(IPacketLayerHelper packetLayerHelper)
        {
            _packetLayerHelper = packetLayerHelper;
        }

        public bool CanHandlePacket(Packet packet)
        {
            return packet.Extract<DhcpV4Packet>() != null;
        }

        public void HandlePacket(Packet packet, PacketLogBuilder logBuilder)
        {
            var dhcpPacket = packet.Extract<DhcpV4Packet>();
            if (dhcpPacket != null)
            {
                var layerLevel = _packetLayerHelper.GetLayerLevel(dhcpPacket);
                var layerName = _packetLayerHelper.GetLayerName(dhcpPacket);
                logBuilder.AddLayer(layerLevel, layerName,
                    $"Client: {dhcpPacket.ClientHardwareAddress}" +
                    $" | Requested IP: {dhcpPacket.YourAddress}" +
                    $" | Type: {dhcpPacket.MessageType}");
            }
        }
    }
}
