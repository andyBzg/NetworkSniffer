using NetworkSniffer.Services.Interfaces;
using SharpPcap;

namespace NetworkSniffer.Services.Implementations
{
    internal class DeviceService : IDeviceService
    {
        public IList<ICaptureDevice> GetDevices()
        {
            throw new NotImplementedException();
        }

        public ICaptureDevice SelectDevice(int index)
        {
            throw new NotImplementedException();
        }
    }
}
