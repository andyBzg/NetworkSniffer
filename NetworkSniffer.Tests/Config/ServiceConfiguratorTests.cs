using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Config;
using NetworkSniffer.Interfaces;
using PacketDotNet;
using System.Net.NetworkInformation;

namespace NetworkSniffer.Tests.Config
{
    public class ServiceConfiguratorTests
    {
        [Fact]
        public void ConfigureServices_ShouldRegisterServices()
        {
            // Arrange
            var provider = ServiceConfigurator.ConfigureServices();

            // Act & Assert
            Assert.NotNull(provider.GetService<ILogger>());
            Assert.NotNull(provider.GetService<IPacketProcessor>());
            Assert.NotEmpty(provider.GetServices<IPacketHandler>());
        }

        [Fact]
        public void ConfigureServices_ShouldResolveDependenciesCorrectly()
        {
            // Arrange
            var provider = ServiceConfigurator.ConfigureServices();
            var testPacket = new EthernetPacket(new PhysicalAddress(new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 }),
                                                new PhysicalAddress(new byte[] { 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB }),
                                                EthernetType.IPv4);

            // Act
            var packetProcessor = provider.GetService<IPacketProcessor>();
            var handlers = provider.GetServices<IPacketHandler>();

            // Assert
            Assert.NotNull(packetProcessor);
            Assert.NotEmpty(handlers);
            Assert.Contains(handlers, h => h.CanHandlePacket(testPacket));
        }
    }
}
