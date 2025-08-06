using Avalonia.Controls;
using Avalonia.Interactivity;
using TcpShare.Services;

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
}