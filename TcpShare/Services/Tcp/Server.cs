using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using TcpShare.Views;

namespace TcpShare.Services.Tcp;

public class Server : Tcp
{
    public string Ip => GetLocalIp();
    
    public bool Connected => _listener == null || _stream == null? false : _stream.CanWrite;

    private TcpListener _listener;
    
    public Server() : this(5080) { }
    public Server(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
    }

    public async Task AcceptClientAsync()
        => _stream = (await _listener.AcceptTcpClientAsync()).GetStream();

    public async Task SendFiles(List<IStorageFile> files)
    {
        // Отправляем количество файлов (4 байта)
        await SendInt(files.Count);
        var buffer = new byte[MainView.BufferSize];
        foreach (var file in files)
            await SendFile(buffer, file);
    }

    private async Task SendFile(byte[] buffer, IStorageFile file)
    {
        await SendInt(Encoding.UTF8.GetBytes(file.Name).Length); // Отправляем длину имени файла (4 байта)
        await SendString(file.Name);  // Отправляем само имя файла
        CurrentFile = file.Name;
        using (var fs = await file.OpenReadAsync())
        {
            await SendLong(fs.Length); // Отправляем длину файла (8 килобайт)
            long bytesWrited = 0;
            int bytesRead;
            while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await _stream.WriteAsync(buffer, 0, bytesRead);
                bytesWrited += bytesRead;
                Progress = (float)(bytesWrited == 0 ? 1 : bytesWrited) / (float)fs.Length;
            }
        }
    }

    private async Task SendInt(int a)
        => await _stream?.WriteAsync(BitConverter.GetBytes(a), 0, 4);

    private async Task SendLong(long a)
        => await _stream?.WriteAsync(BitConverter.GetBytes(a), 0, 8);

    private async Task SendString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await _stream?.WriteAsync(bytes, 0, bytes.Length);
    }

    private string GetLocalIp()
    {
        var ip = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.OperationalStatus == OperationalStatus.Up
                        && x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(x => x.GetIPProperties())
            .SelectMany(x => x.UnicastAddresses)
            .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork)!
            .FirstOrDefault();
        if (ip != null) return ip.Address.ToString();
        return string.Empty;
    }

    public override void Dispose()
    {
        base.Dispose();
        _listener.Dispose();
    }
}