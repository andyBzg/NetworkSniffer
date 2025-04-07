using NetworkSniffer.Utils;

namespace NetworkSniffer.Interfaces
{
    internal interface IPayloadHandler
    {
        bool CanHandlePayload(byte[] payloadData);
        void HandlePayload(byte[] payloadData, PacketLogBuilder logBuilder);
    }
}
