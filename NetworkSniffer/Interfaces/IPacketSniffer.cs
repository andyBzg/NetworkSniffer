namespace NetworkSniffer.Interfaces
{
    internal interface IPacketSniffer
    {
        void StartCapture();
        void StopCapture();
    }
}
