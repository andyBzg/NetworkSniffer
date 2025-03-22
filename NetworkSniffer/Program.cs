using PacketDotNet;
using SharpPcap;

namespace NetworkSniffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Display all network devices
            Console.WriteLine("Network devices found:");
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No network devices found.");
                return;
            }

            int i = 0;
            foreach (var dev in devices)
            {
                Console.WriteLine($"[{i}] {dev.Description}");
                i++;
            }

            Console.Write("Select a device to capture packets: ");
            int index = int.Parse(Console.ReadLine() ?? "0");
            var device = devices[index];

            // Open the device for capturing
            device.OnPacketArrival += new PacketArrivalEventHandler(PacketHandler);
            device.Open(DeviceModes.Promiscuous);
            Console.WriteLine($"Capturing packets on {device.Description}...");
            device.StartCapture();

            Console.WriteLine("Press any key to stop capturing...");
            Console.ReadKey();

            device.StopCapture();
            device.Close();
        }

        private static void PacketHandler(object sender, PacketCapture e)
        {
            var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            var ethPacket = packet.Extract<EthernetPacket>();
            if (ethPacket != null)
            {
                Console.WriteLine($"Ethernet: {ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress}");
            }
        }
    }
}
