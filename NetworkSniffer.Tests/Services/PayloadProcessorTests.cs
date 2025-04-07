using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services;
using NetworkSniffer.Utils;

namespace NetworkSniffer.Tests.Services
{
    public class PayloadProcessorTests
    {
        private readonly IEnumerable<IPayloadHandler> _payloadHandlers;
        private readonly Mock<IPayloadHandler> _payloadHandlerMock;
        private readonly Mock<IPayloadHandler> _secondPayloadHandlerMock;
        private readonly PayloadProcessor _payloadProcessor;

        public PayloadProcessorTests()
        {
            _payloadHandlerMock = new Mock<IPayloadHandler>();
            _secondPayloadHandlerMock = new Mock<IPayloadHandler>();
            _payloadHandlers = new List<IPayloadHandler> { _payloadHandlerMock.Object, _secondPayloadHandlerMock.Object };
            _payloadProcessor = new PayloadProcessor(_payloadHandlers);
        }

        [Fact]
        public void ProcessPayload_HandlerCanHandlePayload_ShouldCallHandlePayload()
        {
            // Arrange
            var payloadData = new byte[] { 0x01, 0x02, 0x03 };
            var logBuilder = new PacketLogBuilder();
            _payloadHandlerMock.Setup(h => h.CanHandlePayload(payloadData)).Returns(true);

            // Act
            _payloadProcessor.ProcessPayload(payloadData, logBuilder);

            // Assert
            _payloadHandlerMock.Verify(h => h.HandlePayload(payloadData, logBuilder), Times.Once);
        }

        [Fact]
        public void ProcessPayload_MultipleHandlers_CalledOnce()
        {
            // Arrange
            var payloadData = new byte[] { 0x01, 0x02, 0x03 };
            var logBuilder = new PacketLogBuilder();
            _payloadHandlerMock.Setup(h => h.CanHandlePayload(payloadData)).Returns(true);
            _secondPayloadHandlerMock.Setup(h2 => h2.CanHandlePayload(payloadData)).Returns(true);

            // Act
            _payloadProcessor.ProcessPayload(payloadData, logBuilder);

            // Assert
            _payloadHandlerMock.Verify(h => h.HandlePayload(payloadData, logBuilder), Times.Once);
            _secondPayloadHandlerMock.Verify(h2 => h2.HandlePayload(payloadData, logBuilder), Times.Never);
        }

        [Fact]
        public void ProcessPayload_FirstHandlerReturnsFalse_ShouldCallSecondHandler()
        {
            // Arrange
            var payloadData = new byte[] { 0x01, 0x02, 0x03 };
            var logBuilder = new PacketLogBuilder();
            _payloadHandlerMock.Setup(h => h.CanHandlePayload(payloadData)).Returns(false);
            _secondPayloadHandlerMock.Setup(h2 => h2.CanHandlePayload(payloadData)).Returns(true);

            // Act
            _payloadProcessor.ProcessPayload(payloadData, logBuilder);

            // Assert
            _payloadHandlerMock.Verify(h => h.HandlePayload(payloadData, logBuilder), Times.Never);
            _secondPayloadHandlerMock.Verify(h2 => h2.HandlePayload(payloadData, logBuilder), Times.Once);
        }

        [Fact]
        public void ProcessPayload_NoHandlerCanHandlePayload_ShouldAddUnhandledLayer()
        {
            // Arrange
            var payloadData = new byte[] { 0x01, 0x02, 0x03 };
            var logBuilder = new PacketLogBuilder();
            _payloadHandlerMock.Setup(h => h.CanHandlePayload(payloadData)).Returns(false);
            _secondPayloadHandlerMock.Setup(h2 => h2.CanHandlePayload(payloadData)).Returns(false);

            // Act
            _payloadProcessor.ProcessPayload(payloadData, logBuilder);

            // Assert
            string logMessage = logBuilder.Build();
            Assert.Contains("Unhandled Payload", logMessage);
        }
    }
}
