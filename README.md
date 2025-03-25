# Network Sniffer

## Overview

This is a simple network sniffer written in C# using the **PacketDotNet** and **SharpPcap** libraries. The program captures packets from a selected network interface and displays details about Ethernet, ARP, IP, TCP, and UDP packets in real-time.

## Features

- Lists all available network interfaces.
- Captures packets in promiscuous mode.
- Displays Ethernet, ARP, IP, TCP, and UDP packet details.
- Stops capturing when a key is pressed.

## Requirements

- .NET SDK (6.0 or later recommended)
- **PacketDotNet** and **SharpPcap** libraries
- [Npcap](https://npcap.com/) (Required for SharpPcap. Make sure to install it with WinPcap API compatibility)
- Administrator privileges to access network interfaces

## Installation

1. Clone or download the repository.
2. Install dependencies using NuGet:
```
dotnet add package PacketDotNet
dotnet add package SharpPcap
```
3. Install Npcap from the official website.

## Usage

1. Run the program:
```
dotnet run
```
3. Select a network interface by entering its corresponding number.
4. The program will start capturing packets and display information in the console.
5. Press any key to stop capturing and exit.

## Example Output

```Network devices found:
[0] Ethernet Adapter
[1] Wi-Fi Adapter
Select a device to capture packets: 1
Capturing packets on Wi-Fi Adapter...
[D] [Ethernet] 00:1A:2B:3C:4D:5E -> 11:22:33:44:55:66 | Type: IPv4
[N] [IP]       192.168.1.2 -> 192.168.1.1 | Protocol: TCP
[T] [TCP]      54321 -> 80 | Flags: SYN
```

## Notes

- Running the program without administrative privileges may prevent it from accessing network interfaces.
- The program does not decrypt encrypted traffic (e.g., HTTPS).

## License

This project is open-source and available under the [MIT License](LICENSE).
