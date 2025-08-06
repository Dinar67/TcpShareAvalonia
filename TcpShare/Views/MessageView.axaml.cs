using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TcpShare.Views;

public partial class MessageView : UserControl
{
    private Action _okClick;
    public MessageView()
    {
        InitializeComponent();
    }

    public MessageView Initialize(string text, Action okClick)
    {
        MessageTb.Text = text;
        _okClick = okClick;
        return this;
    }

    private void OkBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _okClick?.Invoke();
    }
}