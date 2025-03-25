using PacketDotNet;

namespace NetworkSniffer.Interfaces
{
    internal interface IPacketProcessor
    {
        void ProcessPacket(Packet packet);
    }
}
