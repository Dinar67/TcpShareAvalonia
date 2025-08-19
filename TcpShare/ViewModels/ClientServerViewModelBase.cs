using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TcpShare.Services;
using TcpShare.Services.Tcp;
using TcpShare.Views;

namespace TcpShare.ViewModels;

public partial class ClientServerViewModelBase : ViewModelBase
{
    public float Progress => _tcp?.Progress ?? 0;
    public string CurrentFile => _tcp?.CurrentFile ?? string.Empty;
    
    protected UdpLocator _locator;
    protected Tcp _tcp;

    public RelayCommand ConnectClick { get; protected set; }
    protected ClientServerViewModelBase()
    {
        _locator = new UdpLocator();
    }
    protected void TcpOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Пробрасываем уведомления об изменении
        if (e.PropertyName == nameof(Tcp.Progress))
        {
            OnPropertyChanged(nameof(Progress));
        }
        else if (e.PropertyName == nameof(Tcp.CurrentFile))
        {
            OnPropertyChanged(nameof(CurrentFile));
        }
    }
    [RelayCommand]
    public void Back()
    {
        _tcp?.Dispose();
        MainView.Navigate(new ChoiceView());
    }
    protected virtual void NotifyCanExecute()
    {
        (ConnectClick as RelayCommand)!.NotifyCanExecuteChanged();
    }
}