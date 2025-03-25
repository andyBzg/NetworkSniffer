using NetworkSniffer.Interfaces;
using PacketDotNet;
using SharpPcap;

namespace NetworkSniffer.Services
{
    internal class PacketSniffer : IPacketSniffer
    {
        private readonly ICaptureDevice _device;
        private readonly ILogger _logger;
        private readonly IPacketProcessor _packetProcessor;

        public PacketSniffer(ICaptureDevice device, ILogger logger, IPacketProcessor packetProcessor)
        {
            _device = device;
            _logger = logger;
            _packetProcessor = packetProcessor;
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
            try
            {
                var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                _packetProcessor.ProcessPacket(packet);

            }
            catch (Exception ex)
            {
                _logger.Log($"Error processing packet: {ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
