using SharpPcap;

namespace NetworkSniffer.Services.Interfaces
{
    internal interface IDeviceService
    {
        IList<ICaptureDevice> GetDevices();
        ICaptureDevice SelectDevice(int index);
    }
}
