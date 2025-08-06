using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace TcpShare.Services.Tcp;

public class Client : IDisposable
{
    #region EVENTS
    public Action<float> ProgressChanged;
    #endregion
    
    
    private Regex _ipRegex =
        new Regex(@"^((25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$");

    private TcpClient _client;
    private NetworkStream _stream;
    private bool _disposed = false;
    
    private float _progress;
    public float Progress
    {
        get => _progress;
        private set
        {
            _progress = Math.Clamp(value, 0, 1);
            ProgressChanged?.Invoke(_progress);
        } 
    }
    public string CurrentFile { get; private set; }
    public bool Connected => _client == null ? false : _client.Connected;

    private string _ip;
    private int _port;

    public Client(string ipServer, int portServer)
    {
        Validate(ipServer, portServer);
        _client = new TcpClient();
        _ip = ipServer;
        _port = portServer;
    }

    public Client(string ipServer) : this(ipServer, 5080)
    {
    }

    public async Task ConnectAsync()
    {
        await _client.ConnectAsync(_ip, _port);
        _stream = _client.GetStream();
    } 

    public async Task ReceiveFiles()
    {
        int countFiles = await ReceiveInt(); // Читаем длину количества файлов (4 байта)
        var buffer = new byte[4096];
        int i = 0;
        while (i < countFiles)
        {
            try
            {
                i++;
                await ReceiveFile(buffer);
            }
            catch (Exception ex)
            {
                if(ex is NullReferenceException) continue;
                Logger.Show($"Ошибка при получении файла: {ex.Message}");
                break;
            }
        }
    }

    private async Task ReceiveFile(byte[] buffer)
    {
        var fileNameLength = await ReceiveInt(); // Читаем длину имени файла (4 байта)
        var fileName = await ReceiveString(fileNameLength); // Читаем само имя файла
        CurrentFile = fileName;
        long fileLength = await ReceiveLong(); // Читаем длину файла (8 байт)
        
        var file = await Storage.SaveFile(fileName);
        if (file == null) throw new NullReferenceException();
        using (var fs = await file.OpenWriteAsync())
        {
            long bytesReceived = 0;
            while (bytesReceived < fileLength)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, fileLength - bytesReceived);
                int bytesRead = await _stream.ReadAsync(buffer, 0, bytesToRead);

                if (bytesRead == 0)
                    throw new IOException("Connection closed unexpectedly");

                await fs.WriteAsync(buffer, 0, bytesRead);
                bytesReceived += bytesRead;
                        
                Progress = (float)bytesReceived / (float)fileLength;
            }
        }
    }
    private async Task<int> ReceiveInt()
    {
        byte[] buffer = new byte[4];
        await _stream.ReadAsync(buffer, 0, 4);
        return BitConverter.ToInt32(buffer, 0);
    }

    private async Task<long> ReceiveLong()
    {
        byte[] buffer = new byte[8];
        await _stream.ReadAsync(buffer, 0, 8);
        return BitConverter.ToInt64(buffer, 0);
    }

    private async Task<string> ReceiveString(int length)
    {
        byte[] bytes = new byte[length];
        await _stream.ReadAsync(bytes, 0, length);
        return Encoding.UTF8.GetString(bytes);
    }

    private void Validate(string ip, int port)
    {
        if (!_ipRegex.IsMatch(ip))
            throw new ValidationException("IP of server has wrong format!");
        if (port > 65535)
            throw new ValidationException("Server port has limit 65535!");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            ProgressChanged = null;
            _stream?.Dispose();
            _client?.Dispose();
            _disposed = true;
        }
    }
}