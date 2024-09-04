using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace NonogramAvalonia.Services;

public interface IFileSelectService
{
    Task<string?> RequestOpenBoardFileNameAsync();
    Task<string?> RequestSaveBoardFileNameAsync();
}

public sealed class FileSelectService : IFileSelectService
{
    public async Task<string?> RequestOpenBoardFileNameAsync()
    {
        if (GetWindow() is not Window window)
            return null;

        var options = new FilePickerOpenOptions()
        {
            FileTypeFilter = new List<FilePickerFileType>()
            {
                new FilePickerFileType("Board File")
                {
                    Patterns = ["*.json"],
                    AppleUniformTypeIdentifiers = ["public.json"],
                    MimeTypes = ["application/json"]
                }
            },
            Title = "Select Board File"
        };

        var pickerResult = await window.StorageProvider.OpenFilePickerAsync(options);
        return pickerResult?.FirstOrDefault()?.TryGetLocalPath();
    }

    public async Task<string?> RequestSaveBoardFileNameAsync()
    {
        if (GetWindow() is not Window window)
            return null;

        var folder = await window.StorageProvider.TryGetFolderFromPathAsync("_boards");

        if (folder is null)
        {
            return null;
        }

        var options = new FilePickerSaveOptions()
        {
            DefaultExtension = "json",
            SuggestedStartLocation = folder,
            Title = "Select Board File"
        };

        var pickerResult = await window.StorageProvider.SaveFilePickerAsync(options);
        return pickerResult?.TryGetLocalPath();
    }

    private static Window? GetWindow()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return lifetime?.MainWindow;
    }
}
