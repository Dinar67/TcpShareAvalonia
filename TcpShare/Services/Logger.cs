using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using TcpShare.Views;

namespace TcpShare.Services;

public class Logger
{
    private static ContentControl _messageContentControl;
    private static Rectangle _messageRect;
    public Logger(ContentControl messageContentControl, Rectangle messageRect)
    {
        _messageContentControl = messageContentControl;
        _messageRect = messageRect;
    }

    public static void Show(string text)
    {
        _messageContentControl.IsVisible = true;
        _messageRect.IsVisible = true;
        _messageContentControl.Content = new MessageView().Initialize(text, Close);
    }

    private static void Close()
    {
        _messageContentControl.IsVisible = false;
        _messageRect.IsVisible = false;
    }
}