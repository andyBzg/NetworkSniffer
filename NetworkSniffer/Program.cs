using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Config;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services;
using NetworkSniffer.Utils;

namespace NetworkSniffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a service provider
            var serviceProvider = ServiceConfigurator.ConfigureServices();

            // Display all network devices
            var device = DeviceSelector.SelectDevice();
            if (device == null) return;

            // Get the required services
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var packetProcessor = serviceProvider.GetRequiredService<IPacketProcessor>();

            // Create a packet sniffer
            var sniffer = new PacketSniffer(device, logger, packetProcessor);
            sniffer.StartCapture();
        }
    }
}
