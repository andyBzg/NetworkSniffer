using Moq;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services.Handlers;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSniffer.Tests.Services.Handlers
{
    public class DhcpPacketHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly DhcpPacketHandler _handler;

        public DhcpPacketHandlerTests()
        {
            _mockLogger = new Mock<ILogger>();
            _handler = new DhcpPacketHandler(_mockLogger.Object);
        }
    }
}
