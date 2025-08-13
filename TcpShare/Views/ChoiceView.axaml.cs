using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TcpShare.Services;
using TcpShare.ViewModels;

namespace TcpShare.Views;

public partial class ChoiceView : UserControl
{
    public ChoiceView()
    {
        InitializeComponent();
    }

    private void SendBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainView.Navigate(new ServerView());
    }

    private void ReceiveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainView.Navigate(new ClientView());
    }

    private async void InfoBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Logger.Show("Send files with TCP connection! Firstly, connect your devices to own wi-fi.");
    }
    
    public class DeviceDiscoveryServer
    {
        private UdpClient _udpClient;
        private const int DiscoveryPort = 8888;
    
        public void Start()
        {
            _udpClient = new UdpClient(DiscoveryPort);
            _udpClient.BeginReceive(ReceiveCallback, null);
        }
    
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = _udpClient.EndReceive(ar, ref remoteEndPoint);
            string message = Encoding.ASCII.GetString(data);
        
            if (message == "DISCOVERY_REQUEST")
            {
                string response = "DISCOVERY_RESPONSE:" + Dns.GetHostName();
                byte[] responseData = Encoding.ASCII.GetBytes(response);
                _udpClient.Send(responseData, responseData.Length, remoteEndPoint);
            }
        
            _udpClient.BeginReceive(ReceiveCallback, null);
        }
    }
}