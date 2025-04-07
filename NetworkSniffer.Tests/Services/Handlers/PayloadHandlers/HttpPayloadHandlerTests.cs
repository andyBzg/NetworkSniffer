using NetworkSniffer.Services.Handlers.PayloadHandlers;
using NetworkSniffer.Utils;
using System.Text;

namespace NetworkSniffer.Tests.Services.Handlers.PayloadHandlers
{
    public class HttpPayloadHandlerTests
    {
        private readonly HttpPayloadHandler _httpPayloadHandler;

        public HttpPayloadHandlerTests()
        {
            _httpPayloadHandler = new HttpPayloadHandler();
        }

        [Fact]
        public void CanHandlePayload_WhenPayloadContainsHttp_ReturnsTrue()
        {
            // Arrange
            byte[] payloadData = Encoding.UTF8.GetBytes("GET / HTTP/1.1");

            // Act
            bool result = _httpPayloadHandler.CanHandlePayload(payloadData);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandlePayload_WhenPayloadDoesNotContainHttp_ReturnsFalse()
        {
            // Arrange
            byte[] payloadData = Encoding.UTF8.GetBytes("Some random data");

            // Act
            bool result = _httpPayloadHandler.CanHandlePayload(payloadData);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HandlePayload_PayloadContainsHttp_ShouldAddHttpLayer()
        {
            // Arrange
            byte[] payloadData = Encoding.UTF8.GetBytes("GET / HTTP/1.1");
            var logBuilder = new PacketLogBuilder();

            // Act
            _httpPayloadHandler.HandlePayload(payloadData, logBuilder);

            // Assert
            var logMessage = logBuilder.Build();
            Assert.Contains("HTTP", logMessage);
            Assert.Contains("GET / HTTP/1.1", logMessage);
        }
    }
}
