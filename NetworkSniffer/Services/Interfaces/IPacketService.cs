namespace NetworkSniffer.Services.Interfaces
{
    internal interface IPacketService
    {
        void StartCapture();
        void StopCapture();
    }
}
