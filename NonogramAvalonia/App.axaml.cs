using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Views;
using System.Linq;

namespace NonogramAvalonia;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Remove Avalonia data validation so that Mvvm Toolkit's data validation works
        var annotationPlugin = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().FirstOrDefault();
        if (annotationPlugin is not null)
        {
            BindingPlugins.DataValidators.Remove(annotationPlugin);
        }

        var services = new ServiceCollection();
        var bootstrapper = new Bootstrapper();
        bootstrapper.ConfigureIoc(services);
        bootstrapper.ConfigureServices(services);
        bootstrapper.ConfigureViews(services);
        bootstrapper.ConfigureViewModels(services);

        var provider = services.BuildServiceProvider();
        await bootstrapper.LoadConfigurations(provider);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var shellView = provider.GetService<ShellView>();
            shellView!.DataContext = provider.GetService<ShellViewModel>();
            desktop.MainWindow = shellView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
