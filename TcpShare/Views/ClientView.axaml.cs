using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TcpShare.Services;
using TcpShare.Services.Tcp;
using Exception = System.Exception;

namespace TcpShare.Views;

public partial class ClientView : UserControl
{
    private Client _client;
    private int _port;
    public ClientView()
    {
        InitializeComponent();
    }
    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _client?.Dispose();
        MainView.Navigate(new ChoiceView());
    }

    private async void ConnectBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            ConnectBtn.IsEnabled = false;
            Validate();
            if (_client == null || !_client.Connected) await Connect();
            await _client.ReceiveFiles();
            Logger.Show("Получено!");
            ConnectBtn.IsEnabled = true;
        }
        catch (Exception ex)
        {
            ConnectBtn.IsEnabled = true;
            Logger.Show(ex.Message);
            _client?.Dispose();
        }
    }

    private async Task Connect()
    {
        _client = new Client(IpTb.Text!, _port);
        _client.ProgressChanged += ProgressChanged;
        await _client.ConnectAsync();
        DebugText.IsVisible = true;
    }

    private void ProgressChanged(float value)
    {
        FileReceivePb.Value = value;
        FileNameTb.Text = _client.CurrentFile;
    }

    private void Validate()
    {
        if(string.IsNullOrWhiteSpace(IpTb.Text))
            throw new ValidationException("Fill the IP textBox!");
        if (!int.TryParse(PortTb.Text, out _port))
            throw new ValidationException("Server port should be numbers!");
    }
}