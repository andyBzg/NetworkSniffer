using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Config;
using NetworkSniffer.Interfaces;

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
    }
}
