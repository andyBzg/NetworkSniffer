using SharpPcap;

namespace NetworkSniffer.Utils
{
    internal class DeviceSelector
    {
        public static ICaptureDevice? SelectDevice()
        {
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No network devices found.");
                return null;
            }
            else
            {
                Console.WriteLine("Network devices found:");
            }

            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine($"[{i}] {devices[i].Description}");
            }

            Console.Write("Select a device to capture packets: ");
            int index;
            while (!int.TryParse(Console.ReadLine(), out index) || index < 0 || index >= devices.Count)
            {
                Console.WriteLine("Invalid input. Please try again.");
                Console.Write("Select a device: ");
            }

            return devices[index];
        }
    }
}
