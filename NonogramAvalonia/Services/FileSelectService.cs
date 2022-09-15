using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using JetBrains.Annotations;

namespace NonogramAvalonia.Services;

internal interface IFileSelectService
{
    Task<Uri?> GetBoardFileNameByUserAsync();
}

internal class FileSelectService : IFileSelectService
{
    public async Task<Uri?> GetBoardFileNameByUserAsync()
    {
        var options = new FilePickerOpenOptions()
        {
            FileTypeFilter = new List<FilePickerFileType>()
            {
                new FilePickerFileType("Board File")
                {
                    Patterns = new[] { "*.json" },
                    AppleUniformTypeIdentifiers = new[] { "public.json" },
                    MimeTypes = new[] { "application/json" }
                }
            },
            Title = "Select Board File"
        };

        var window = GetWindow();

        if (window is null)
            return null;

        var pickerResult = await window.StorageProvider.OpenFilePickerAsync(options);
        //if (pickerResult?[0] is IStorageFile storageFile)
        //{
        //    var uriResult = storageFile.TryGetUri(out var uri);

        //    if (uriResult is true)
        //        return uri;
        //}

        if (pickerResult?[0].TryGetUri(out var uri) is true)
        {
            return uri;
        }

        return null;
    }

    private static Window? GetWindow()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return lifetime?.MainWindow;
    }
}
