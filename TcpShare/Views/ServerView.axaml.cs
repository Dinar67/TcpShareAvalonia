using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using TcpShare.Services;
using TcpShare.Services.Tcp;

namespace TcpShare.Views;

public partial class ServerView : UserControl
{
    private Server _server;
    public ServerView()
    {
        InitializeComponent();
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _server?.Dispose();
        MainView.Navigate(new ChoiceView());
    }

    private async void StartBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            StartBtn.IsEnabled = false;
            Validate();
            var files = await Storage.OpenFiles();
            if (files.Count == 0) throw new Exception("You did not select files!");

            if (_server == null || !_server.Connected) await Connect();
            await _server.SendFiles(files.ToList());
            Logger.Show("Отправлено!");
            StartBtn.IsEnabled = true;
        }
        catch (Exception ex)
        {
            StartBtn.IsEnabled = true;
            Logger.Show(ex.Message);
            _server?.Dispose();
        }
    }

    private async Task Connect()
    {
        _server = new Server(MainView.Port);
        _server.ProgressChanged += ProgressChanged;
        ShowIp();
        Task.Run(async () =>
        {
            while (!_server.Connected)
            {
                await Task.Delay(50);
                await UdpLocator.SendBroadcastMessage();
            }
        });
        await _server.AcceptClientAsync();
        DebugText.IsVisible = true;
    }

    private void ProgressChanged(float value)
    {
        FileSendPb.Value = value;
        FileNameTb.Text = _server.CurrentFile;
    }


    private void ShowIp()
    {
        IpPanel.IsVisible = true;
        IpTb.Text = _server.Ip;
    }
    private void Validate()
    {
        if (MainView.Port >= 65535) 
            throw new Exception("Server port is wrong!");
    }
}