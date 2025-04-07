using NetworkSniffer.Interfaces;
using System.Text;

namespace NetworkSniffer.Utils
{
    internal class PacketLogBuilder
    {
        private readonly StringBuilder _logBuilder = new();
        private int _indentLevel = 0;
        private const string IndentString = "  ";

        public PacketLogBuilder StartPacket(DateTime timestamp, int packetNumber)
        {
            _logBuilder.AppendLine($"[{timestamp:HH:mm:ss.fff}] Packet #{packetNumber}");
            _indentLevel = 1;
            return this;
        }

        public PacketLogBuilder AddLayer(string osiLevel, string layerName, string layerDetails)
        {

            _logBuilder.AppendLine($"{new string(' ', _indentLevel * IndentString.Length)}└─ [{osiLevel}][{layerName}] {layerDetails}");
            _indentLevel++;
            return this;
        }

        public string Build()
        {
            return _logBuilder.ToString();
        }
    }
}
