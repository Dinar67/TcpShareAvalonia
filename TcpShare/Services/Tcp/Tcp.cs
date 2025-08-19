using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace TcpShare.Services.Tcp;

public class Tcp : IDisposable, INotifyPropertyChanged
{
    public Action<float> ProgressChanged;
    
    private float _progress;
    public float Progress
    {
        get => _progress;
        protected set
        {
            _progress = Math.Clamp(value, 0, 1);
            ProgressChanged?.Invoke(_progress);
            OnPropertyChanged();
        } 
    }
    private string _currentFile;
    public string CurrentFile
    {
        get => _currentFile;
        protected set
        {
            _currentFile = value;
            OnPropertyChanged();
        } 
    }
    
    protected bool _disposed = false;
    protected NetworkStream _stream;
    
    public virtual void Dispose()
    {
        if (_disposed) return;
        ProgressChanged = null;
        _stream?.Dispose();
        _disposed = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}