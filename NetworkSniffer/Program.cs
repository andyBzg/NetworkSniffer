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
            // Parse the packet
            var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
            var ethPacket = packet.Extract<EthernetPacket>();
            if (ethPacket != null)
            {
                Console.WriteLine($"Ethernet: {ethPacket.SourceHardwareAddress} -> {ethPacket.DestinationHardwareAddress} | Type: {ethPacket.Type}");

                // Extract ARP and IP packets
                var arpPacket = packet.Extract<ArpPacket>();
                if (arpPacket != null)
                {
                    Console.WriteLine($"ARP: {arpPacket.SenderHardwareAddress} -> {arpPacket.TargetHardwareAddress}");
                }

                var ipPacket = packet.Extract<IPPacket>();
                if (ipPacket != null)
                {
                    Console.WriteLine($"IP: {ipPacket.SourceAddress} -> {ipPacket.DestinationAddress} | Protocol: {ipPacket.Protocol}");

                    // Extract TCP and UDP packets
                    if (ipPacket.Protocol == ProtocolType.Tcp)
                    {
                        var tcpPacket = packet.Extract<TcpPacket>();
                        if (tcpPacket != null)
                        {
                            Console.WriteLine($"TCP: {tcpPacket.SourcePort} -> {tcpPacket.DestinationPort} | Flags: {tcpPacket.Flags}");
                        }
                    }
                    else if (ipPacket.Protocol == ProtocolType.Udp)
                    {
                        var udpPacket = packet.Extract<UdpPacket>();
                        if (udpPacket != null)
                        {
                            Console.WriteLine($"UDP: {udpPacket.SourcePort} -> {udpPacket.DestinationPort} | Length: {udpPacket.Length}");
                        }
                    }
                }
            }
        }
    }
}
