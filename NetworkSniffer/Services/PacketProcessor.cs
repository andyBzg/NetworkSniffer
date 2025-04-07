using NetworkSniffer.Interfaces;
using NetworkSniffer.Utils;
using PacketDotNet;

namespace NetworkSniffer.Services
{
    internal class PacketProcessor : IPacketProcessor
    {
        private readonly IEnumerable<IPacketHandler> _packetHandlers;
        private readonly ILogger _logger;
        private int _packetCounter = 1;

        public PacketProcessor(IEnumerable<IPacketHandler> packetHandlers, ILogger logger)
        {
            _packetHandlers = packetHandlers;
            _logger = logger;
        }

        public void ProcessPacket(Packet packet)
        {
            var logBuilder = new PacketLogBuilder()
                .StartPacket(DateTime.Now, _packetCounter++);

            bool handled = false;

            foreach (var handler in _packetHandlers)
            {
                if (handler.CanHandlePacket(packet))
                {
                    handler.HandlePacket(packet, logBuilder);
                    handled = true;
                }
            }

            if (!handled)
            {
                _logger.Log("Unhandled packet type: " + packet.GetType().Name, ConsoleColor.Red);
            }
            else
            {
                _logger.Log(logBuilder.Build());
            }
        }
    }
}
