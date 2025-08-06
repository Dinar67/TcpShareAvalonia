using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using TcpShare.Views;

namespace TcpShare.Services;

public static class Storage
{
    public static async Task<IStorageFile?> OpenFile()
    {
        var files = await OpenFiles(false);
        if(files.Count == 0) return null;
        return files[0];
    }
    public static async Task<List<IStorageFile>> OpenFiles()
        => await OpenFiles(true);
    private static async Task<List<IStorageFile>> OpenFiles(bool allowMultiple)
    {
        if (!Validate()) return new List<IStorageFile>();
        return (await MainView.Storage!.OpenFilePickerAsync(new FilePickerOpenOptions()
            {AllowMultiple = allowMultiple})).ToList();
    }
    public static async Task<IStorageFile?> SaveFile(string fileName)
    {
        return await MainView.Storage!.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            SuggestedFileName = fileName,
            DefaultExtension = Path.GetExtension(fileName)
        });
    }
    private static bool Validate()
    {
        if (MainView.Storage == null) return false;
        return true;
    }
}