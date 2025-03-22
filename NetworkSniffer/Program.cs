using SharpPcap;

namespace NetworkSniffer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Display all network devices
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No network devices found.");
                return;
            }

            Console.WriteLine("Select a device to capture packets:");
            int i = 0;
            foreach (var dev in devices)
            {
                Console.WriteLine($"[{i}] {dev.Description}");
                i++;
            }
        }
    }
}
