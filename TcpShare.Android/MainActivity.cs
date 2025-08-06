﻿using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace TcpShare.Android;

[Activity(
    Label = "TcpShare",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/logo",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}