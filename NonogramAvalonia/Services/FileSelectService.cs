using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace NonogramAvalonia.Services;

internal interface IFileSelectService
{
    Task<string?> GetBoardFileNameByUserAsync();
}

internal class FileSelectService : IFileSelectService
{
    public async Task<string?> GetBoardFileNameByUserAsync()
    {
        var filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Extensions = { "json" }, Name = "Nonogram Board Files" }
        };

        var dialog = new OpenFileDialog
        {
            Title = "Select Board File",
            AllowMultiple = false,
            Filters = filters
        };

        var window = GetWindow();

        if (window is not null)
        {
            var result = await dialog.ShowAsync(window);
            return result?.FirstOrDefault();
        }

        return null;
    }

    private static Window? GetWindow()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return lifetime?.MainWindow;
    }
}
