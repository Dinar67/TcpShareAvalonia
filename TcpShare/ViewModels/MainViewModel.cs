using System;
using System.Collections.Generic;

namespace TcpShare.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public List<int> BufferSizes { get; private set; } = new List<int>() { 4096, 8192, 16384, 32768 };
    private int _currentBufferSize;
    public int CurrentBufferSize
    {
        get => _currentBufferSize;
        set
        {
            _currentBufferSize = value;
            OnPropertyChanged();
        }
    }

    private int _port;

    public int Port
    {
        get => _port;
        set
        {
            _port = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        CurrentBufferSize = BufferSizes[1];
        Port = 5080;
    }
}