using PacketDotNet;

namespace NetworkSniffer.Interfaces
{
    internal interface IPacketHandler
    {
        bool CanHandlePacket(Packet packet);
        void HandlePacket(Packet packet);
    }
}
