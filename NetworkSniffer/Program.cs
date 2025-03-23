using NetworkSniffer.Interfaces;
using NetworkSniffer.Loggers;
using NetworkSniffer.Services;
using NetworkSniffer.Services.Handlers;
using NetworkSniffer.Utils;

namespace NetworkSniffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Display all network devices
            var device = DeviceSelector.SelectDevice();
            if (device == null) return;

            ILogger logger = new ConsoleLogger();

            // Create a list of packet handlers
            var handlers = new List<IPacketHandler>
            {
                new EthernetPacketHandler(logger),
                new ArpPacketHandler(logger)
            };

            // Create a packet sniffer
            IPacketSniffer sniffer = new PacketSniffer(device, logger, handlers);
            sniffer.StartCapture();
        }
    }
}
