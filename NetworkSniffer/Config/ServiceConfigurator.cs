using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Loggers;
using NetworkSniffer.Services.Handlers;
using NetworkSniffer.Services;

namespace NetworkSniffer.Config
{
    internal class ServiceConfigurator
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILogger, ConsoleLogger>();

            services.AddSingleton<IPacketHandler, EthernetPacketHandler>();
            services.AddSingleton<IPacketHandler, ArpPacketHandler>();
            services.AddSingleton<IPacketHandler, IpPacketHandler>();
            services.AddSingleton<IPacketHandler, TcpPacketHandler>();
            services.AddSingleton<IPacketHandler, UdpPacketHandler>();
            services.AddSingleton<IPacketHandler, IcmpPacketHandler>();
            services.AddSingleton<IPacketHandler, DhcpPacketHandler>();
            services.AddSingleton<IPacketHandler, HttpPacketHandler>();

            services.AddSingleton<IPacketProcessor, PacketProcessor>();

            return services.BuildServiceProvider();
        }
    }
}
