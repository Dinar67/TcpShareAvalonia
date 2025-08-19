using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using TcpShare.Services;
using TcpShare.Services.Tcp;
using TcpShare.Views;

namespace TcpShare.ViewModels;

public class ServerViewModel : ClientServerViewModelBase
{
    private bool _isSending = false;

    private string _ip;

    public string IP
    {
        get => _ip;
        private set
        {
            _ip = value;
            OnPropertyChanged();
        }
    }
    
    public ServerViewModel() : base()
    {
        ConnectClick = new RelayCommand(
            async () =>
            {
                _isSending = true;
                NotifyCanExecute();
                await Start();
                _isSending = false;
                NotifyCanExecute();
            },
        () => { return !_isSending; }
        );
    }

    private async Task Start()
    {
        try
        {
            Validate();
            var files = await SelectFiles();

            using (var server = await Connect())
                await server.SendFiles(files.ToList());
            
            Logger.Show("Отправлено!");
        }
        catch (Exception ex)
        {
            Logger.Show(ex.Message);
        }
    }

    private async Task<List<IStorageFile>> SelectFiles()
    {
        var files = await Storage.OpenFiles();
        if (files.Count == 0) throw new Exception("You did not select files!");
        return files;
    }
    private async Task<Server> Connect()
    {
        var server = new Server(MainView.Port);
        _tcp = server;
        IP = server.Ip;
        _tcp.PropertyChanged += TcpOnPropertyChanged;
        Task.Run(async () =>
        {
            while (!server.Connected)
            {
                await Task.Delay(50);
                await _locator.SendBroadcastMessage();
            }
        });
        await server.AcceptClientAsync();
        return server;
    }


    private void Validate()
    {
        if (MainView.Port >= 65535) 
            throw new Exception("Server port is wrong!");
    }
}