using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;

namespace NetworkSniffer.Services.Handlers.PayloadHandlers
{
    internal class RawPayloadHandler : IPayloadHandler
    {
        public bool CanHandlePayload(byte[] payloadData)
        {
            return payloadData.Length > 0;
        }
        public void HandlePayload(byte[] payloadData, PacketLogBuilder logBuilder)
        {
            string hexDump = BitConverter.ToString(payloadData).Replace("-", " ");
            hexDump = hexDump.Substring(0, Math.Min(200, hexDump.Length));

            string asciiText = new string(payloadData.Select(b => b >= 32 && b <= 126 ? (char)b : '.').ToArray());
            asciiText = asciiText.Substring(0, Math.Min(100, asciiText.Length));

            logBuilder.AddLayer("RAW", "Unknown Payload", $"Hex: {hexDump} | ASCII: {asciiText}");
        }
    }
}
