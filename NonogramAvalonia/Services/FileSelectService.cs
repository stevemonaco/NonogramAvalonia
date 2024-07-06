using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace NonogramAvalonia.Services;

public interface IFileSelectService
{
    Task<string?> RequestBoardFileNameAsync();
}

public class FileSelectService : IFileSelectService
{
    public async Task<string?> RequestBoardFileNameAsync()
    {
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

        var window = GetWindow();

        if (window is null)
            return null;

        var pickerResult = await window.StorageProvider.OpenFilePickerAsync(options);
        return pickerResult?.FirstOrDefault()?.TryGetLocalPath();
    }

    private static Window? GetWindow()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return lifetime?.MainWindow;
    }
}
