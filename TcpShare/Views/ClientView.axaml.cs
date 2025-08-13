using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    private string _ip;

    private List<UdpDevice> _devices = new List<UdpDevice>();
    public ClientView()
    {
        InitializeComponent();
        UdpLocator.DeviceLocated += (device) =>
        {
            _devices.Add(device);
            RefreshDevices();
        };
        RefreshDevices();
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
            if ( _client == null || !_client.Connected) await Connect();

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
        _client = new Client(_ip, _port);
        _client.ProgressChanged += ProgressChanged;
        await _client.ConnectAsync();
        DebugText.IsVisible = true;
    }

    private void ProgressChanged(float value)
    {
        FileReceivePb.Value = value;
        FileNameTb.Text = _client.CurrentFile;
    }
    
    private async void RefreshBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _devices.Clear();
        RefreshBtn.IsEnabled = false;
        await UdpLocator.LocateDevices();
        RefreshBtn.IsEnabled = true;
    }

    private void RefreshDevices()
    {
        DevicesLb.ItemsSource = null;
        DevicesLb.ItemsSource = _devices;
    }

    private void DevicesLb_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _ip = (DevicesLb.SelectedItem as UdpDevice)!.IpAdrress;
        _port = MainView.Port;
    }
}