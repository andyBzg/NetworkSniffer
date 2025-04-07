using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;

namespace NetworkSniffer.Services
{
    internal class PayloadProcessor : IPayloadProcessor
    {
        private readonly IEnumerable<IPayloadHandler> _payloadHandlers;

        public PayloadProcessor(IEnumerable<IPayloadHandler> payloadHandlers)
        {
            _payloadHandlers = payloadHandlers;
        }

        public void ProcessPayload(byte[] payloadData, PacketLogBuilder logBuilder)
        {
            foreach (var handler in _payloadHandlers)
            {
                if (handler.CanHandlePayload(payloadData))
                {
                    handler.HandlePayload(payloadData, logBuilder);
                    return;
                }
            }

            logBuilder.AddLayer("A", "Unhandled Payload", "Payload coul not be parsed.");
        }
    }
}
