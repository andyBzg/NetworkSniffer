using PacketDotNet;

namespace NetworkSniffer.Interfaces
{
    internal interface IPacketHandler
    {
        void HandlePacket(Packet packet);
    }
}
