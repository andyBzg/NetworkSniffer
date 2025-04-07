using NetworkSniffer.Utils;

namespace NetworkSniffer.Interfaces
{
    internal interface IPayloadProcessor
    {
        void ProcessPayload(byte[] payloadData, PacketLogBuilder logBuilder);
    }
}
