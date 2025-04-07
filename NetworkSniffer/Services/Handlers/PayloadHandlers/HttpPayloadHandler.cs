using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using System.Text;

namespace NetworkSniffer.Services.Handlers.PayloadHandlers
{
    internal class HttpPayloadHandler : IPayloadHandler
    {
        public bool CanHandlePayload(byte[] payloadData)
        {
            string text = Encoding.UTF8.GetString(payloadData);
            return text.Contains("HTTP/") || text.Contains("GET") || text.Contains ("POST") || text.Contains("PUT") || text.Contains("DELETE");
        }

        public void HandlePayload(byte[] payloadData, PacketLogBuilder logBuilder)
        {
            string text = Encoding.UTF8.GetString(payloadData);
            var reader = new StringReader(text);
            string? firstLine = reader.ReadLine();

            logBuilder.AddLayer("A", "HTTP", $"Payload: {firstLine}");
        }
    }
}
