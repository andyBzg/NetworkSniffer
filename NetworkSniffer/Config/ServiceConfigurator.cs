using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Loggers;
using NetworkSniffer.Services;
using NetworkSniffer.Services.Handlers.PacketHandlers;
using NetworkSniffer.Utils;

namespace NetworkSniffer.Config
{
    internal class ServiceConfigurator
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register logging services
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<IPacketLayerHelper, PacketLayerHelper>();

            // Register packet handlers
            services.AddSingleton<IPacketHandler, EthernetPacketHandler>();
            services.AddSingleton<IPacketHandler, ArpPacketHandler>();
            services.AddSingleton<IPacketHandler, IpPacketHandler>();
            services.AddSingleton<IPacketHandler, TcpPacketHandler>();
            services.AddSingleton<IPacketHandler, UdpPacketHandler>();
            services.AddSingleton<IPacketHandler, IcmpPacketHandler>();

            // Register payload handlers

            // Register packet processor and payload processor
            services.AddSingleton<IPacketProcessor, PacketProcessor>();
            services.AddSingleton<IPayloadProcessor, PayloadProcessor>();

            return services.BuildServiceProvider();
        }
    }
}
