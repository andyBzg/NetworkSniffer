using PacketDotNet;

namespace NetworkSniffer
{
    internal interface IPacketLayerHelper
    {
        string GetLayerLevel(Packet packet);
        string GetLayerName(Packet packet);
    }
}