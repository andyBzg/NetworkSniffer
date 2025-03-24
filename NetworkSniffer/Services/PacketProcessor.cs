using NetworkSniffer.Interfaces;
using PacketDotNet;

namespace NetworkSniffer.Services
{
    internal class PacketProcessor : IPacketProcessor
    {
        private readonly IEnumerable<IPacketHandler> _packetHandlers;
        private readonly ILogger _logger;

        public PacketProcessor(IEnumerable<IPacketHandler> packetHandlers, ILogger logger)
        {
            _packetHandlers = packetHandlers;
            _logger = logger;
        }

        public void ProcessPacket(Packet packet)
        {
            bool handled = false;

            foreach (var handler in _packetHandlers)
            {
                if (handler.CanHandlePacket(packet))
                {
                    handler.HandlePacket(packet);
                    handled = true;
                }
            }

            if (!handled)
            {
                _logger.Log("Unhandled packet type: " + packet.GetType().Name);
            }
        }
    }
}
