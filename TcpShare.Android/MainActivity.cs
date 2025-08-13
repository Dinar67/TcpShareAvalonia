using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
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
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Запрашиваем разрешения после инициализации Activity
        RequestStoragePermissions();
    }

    private void RequestStoragePermissions()
    {
        var permissionsToRequest = new List<string>();

        // Для Android 10+ (API 29+) WRITE_EXTERNAL_STORAGE не нужен
        // Используем Scoped Storage или MANAGE_EXTERNAL_STORAGE для полного доступа

        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
            permissionsToRequest.Add(Manifest.Permission.ReadExternalStorage);

        // WRITE_EXTERNAL_STORAGE — только для API < 29
        if ((int)Build.VERSION.SdkInt < 29 &&
            ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            permissionsToRequest.Add(Manifest.Permission.WriteExternalStorage);

        // Дополнительные разрешения
        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessWifiState) != Permission.Granted)
            permissionsToRequest.Add(Manifest.Permission.AccessWifiState);

        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != Permission.Granted)
            permissionsToRequest.Add(Manifest.Permission.AccessNetworkState);

        // MANAGE_EXTERNAL_STORAGE — особое разрешение для доступа ко всем файлам (Android 11+)
        if ((int)Build.VERSION.SdkInt >= 30)
        {
            if (!Environment.IsExternalStorageManager)
            {
                permissionsToRequest.Add(Manifest.Permission.ManageExternalStorage);
            }
        }

        if (permissionsToRequest.Count > 0)
        {
            ActivityCompat.RequestPermissions(this, permissionsToRequest.ToArray(), 1000);
        }
    }
}