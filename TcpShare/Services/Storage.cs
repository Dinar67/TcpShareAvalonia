using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using TcpShare.Views;

namespace TcpShare.Services;

public static class Storage
{
    #region OpenFiles
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
    #endregion

    
    #region SaveFiles
    public static async Task<IStorageFile?> SaveFile(string fileName)
    {
        return await MainView.Storage!.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            SuggestedFileName = fileName,
            DefaultExtension = Path.GetExtension(fileName)
        });
    }
    #endregion


    #region OpenFolders
    public static async Task<IStorageFolder?> OpenFolder()
    {
        var files = await OpenFolders(false);
        if(files.Count == 0) return null;
        return files[0];
    }
    public static async Task<List<IStorageFolder>> OpenFolders()
        => await OpenFolders(true);
    private static async Task<List<IStorageFolder>> OpenFolders(bool allowMultiple)
    {
        if (!Validate()) return new List<IStorageFolder>();
        return (await MainView.Storage!.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {AllowMultiple = allowMultiple})).ToList();
    }
    #endregion
    
    
    private static bool Validate()
    {
        if (MainView.Storage == null) return false;
        return true;
    }
}