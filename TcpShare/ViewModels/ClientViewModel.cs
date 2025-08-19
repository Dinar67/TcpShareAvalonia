using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TcpShare.Services;
using TcpShare.Services.Tcp;
using TcpShare.Views;

namespace TcpShare.ViewModels;

public class ClientViewModel : ClientServerViewModelBase
{
    public ObservableCollection<UdpDevice> Devices { get; private set; }
    
    private UdpDevice _selectedDevice;
    public UdpDevice SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            _selectedDevice = value;
            NotifyCanExecute();
        }
    }
    
    public ICommand RefreshCommand { get; private set; }
    private bool _isConnecting = false;

    public ClientViewModel() : base()
    {
        Devices = new ObservableCollection<UdpDevice>();
        Devices.CollectionChanged += DevicesOnCollectionChanged;
        _locator.DeviceLocated += (device) => { Devices.Add(device); };

        RefreshCommand = new AsyncRelayCommand(RefreshAsync, canExecute: () => !_isConnecting);

        ConnectClick = new RelayCommand(async () =>
        {
            _isConnecting = true;
            NotifyCanExecute();
            await OnConnectClick();
            _isConnecting = false;
        }, () =>
        {
            return SelectedDevice != null && !_isConnecting;
        });
        
    }
    private void DevicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => OnPropertyChanged();
    
    private async Task RefreshAsync()
    {
        _tcp = null;
        Devices.Clear();
        await _locator.LocateDevices();
    }

    private async Task OnConnectClick()
    {
        try
        {
            if ( _tcp == null) await ConnectAsync();
            if(_tcp is not Client client) return;
            
            _tcp.PropertyChanged += TcpOnPropertyChanged;
            
            await client.ReceiveFiles();
            RefreshAsync();
            Logger.Show("Получено!");
        }
        catch (Exception ex)
        {
            Logger.Show(ex.Message);
            _tcp?.Dispose();
        }
    }
    
    private async Task ConnectAsync()
    {
        _tcp = new Client(SelectedDevice.IpAdrress, MainView.Port);
        await (_tcp as Client)!.ConnectAsync();
    }

    protected override void NotifyCanExecute()
    {
        base.NotifyCanExecute();
        (RefreshCommand as AsyncRelayCommand)!.NotifyCanExecuteChanged();
    }
}