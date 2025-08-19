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

public class UdpLocator
{
    public Action<UdpDevice> DeviceLocated;
    
    private readonly float _time;
    private readonly int _port;
    private readonly string _name;
    
    public UdpLocator() : this(5, 11000, Environment.MachineName){ }
    public UdpLocator(string machineName) : this(5, 11000, Environment.MachineName){ }
    public UdpLocator(float timeLocate, int port) : this(timeLocate, port, Environment.MachineName){ }
    public UdpLocator(float timeLocate, int port, string machineName)
    {
        _time = timeLocate;
        _port = port;
        _name = machineName;
    }
    
    public async Task<List<UdpDevice>> LocateDevices()
    {
        var devices = new List<UdpDevice>();
        using (UdpClient client = CreateUdpClient())
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_time));
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var device = await LocateDevice(client, cts);
                    if (!devices.Any(x => x.IpAdrress == device.IpAdrress))
                    {
                        devices.Add(device);
                        DeviceLocated?.Invoke(device);
                    }
                }
                catch (OperationCanceledException)
                {   //время вышло
                    break;
                }
            }
        }

        return devices;
    }

    private UdpClient CreateUdpClient()
    {
        UdpClient client;
        int attemptPort = _port;
    
        while (true)
        {
            try
            {
                client = new UdpClient(attemptPort);
                return client;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                attemptPort++;
                if (attemptPort > _port + 100) // Лимит попыток
                    throw new InvalidOperationException("No available port found");
            }
        }
    }
    private async Task<UdpDevice> LocateDevice(UdpClient client, CancellationTokenSource cts)
    {
        var result = await client.ReceiveAsync(cts.Token);
        return new UdpDevice(
            result.RemoteEndPoint.ToString().Split(":")[0],
            Encoding.UTF8.GetString(result.Buffer)
        );
    }

    public async Task SendBroadcastMessage()
    {
        using (var client = new UdpClient(_port))
        {
            client.EnableBroadcast = true;
            await SendMessage(client, new IPEndPoint(IPAddress.Broadcast, _port), Environment.MachineName);
        }
    }

    private async Task SendMessage(UdpClient client, IPEndPoint point, string message)
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