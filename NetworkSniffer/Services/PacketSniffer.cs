using NetworkSniffer.Interfaces;
using PacketDotNet;
using SharpPcap;

namespace NetworkSniffer.Services
{
    internal class PacketSniffer : IPacketSniffer
    {
        private readonly ICaptureDevice _device;
        private readonly ILogger _logger;
        private readonly List<IPacketHandler> _handlers;

        public PacketSniffer(ICaptureDevice device, ILogger logger, List<IPacketHandler> handlers)
        {
            _device = device;
            _logger = logger;
            _handlers = handlers;
        }

        public void StartCapture()
        {
            _device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);
            _device.Open(DeviceModes.Promiscuous);
            _logger.Log($"Capturing packets on {_device.Description}...");
            _device.StartCapture();

            _logger.Log("Press any key to stop capturing...");
            Console.ReadKey();

            StopCapture();
        }

        public void StopCapture()
        {
            _device.StopCapture();
            _device.Close();
            _logger.Log("Capture stopped.");
        }

        private void OnPacketArrival(object sender, PacketCapture e)
        {
            var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            foreach (var handler in _handlers)
            {
                handler.HandlePacket(packet);
            }
        }
    }
}
