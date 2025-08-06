using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using TcpShare.Services;

namespace TcpShare.Views;

public partial class MainView : UserControl
{
    public static IStorageProvider? Storage { get; private set; }
    private static MainView _instance;
    public MainView()
    {
        InitializeComponent();
        _instance = this;
        InitServices();
        Navigate(new ChoiceView());
        this.Loaded += OnLoaded;
    }

    private void InitServices()
    {
        new Logger(MessageContentControl, MessageRect);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        var top = TopLevel.GetTopLevel(this);
        if(top != null) Storage = top!.StorageProvider;
    }


    public static void Navigate(UserControl page) {
        _instance.MainContentControl.Content = page;
    }
}