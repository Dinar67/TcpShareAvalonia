using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpShare.Services;

public static class UdpLocator
{
    #region CONSTANTS

    public const int PORT = 11000;
    public const float TIME_LOCATE = 5f;

    #endregion


    public static Action<UdpDevice> DeviceLocated;

    public static List<UdpDevice> Devices { get; private set; } = new List<UdpDevice>();

    public static async Task<List<UdpDevice>> LocateDevices() => await LocateDevices(Devices);
    public static async Task<List<UdpDevice>> LocateDevices(List<UdpDevice> devices)
    {
        Devices.Clear();
        using (UdpClient client = new UdpClient(PORT))
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TIME_LOCATE));
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var device = await LocateDevice(client, cts);
                    if (!devices.Any(x => x.IpAdrress == device.IpAdrress))
                    {
                        devices.Add(device);
                        DeviceLocated?.Invoke(device);
                        Debug.WriteLine($"Найдено устройство: {device.DeviceName} ({device.IpAdrress})");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Время вышло
                    break;
                }
            }
        }

        return devices;
    }
    private static async Task<UdpDevice> LocateDevice(UdpClient client, CancellationTokenSource cts)
    {
        var result = await client.ReceiveAsync(cts.Token);
        return new UdpDevice(
            result.RemoteEndPoint.ToString().Split(":")[0],
            Encoding.UTF8.GetString(result.Buffer)
        );
    }

    public static async Task SendBroadcastMessage()
    {
        using (var client = new UdpClient(PORT))
        {
            client.EnableBroadcast = true;
            await SendMessage(client, new IPEndPoint(IPAddress.Broadcast, PORT), Environment.MachineName);
        }
    }

    private static async Task SendMessage(UdpClient client, IPEndPoint point, string message)
    {
        var data = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(data, data.Length, point);
    }
}

public class UdpDevice
{
    public string IpAdrress { get; private set; }
    public string DeviceName { get; private set; }

    public UdpDevice(string ipAdrress, string deviceName)
    {
        IpAdrress = ipAdrress;
        DeviceName = deviceName;
    }
}