using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Views;
using System.Linq;

namespace NonogramAvalonia;
public sealed partial class App : Application
{
    private ServiceProvider? _serviceProvider;

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
        bootstrapper.ConfigureViewModels(services);
        bootstrapper.ConfigureViews(services);

        _serviceProvider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(_serviceProvider);

        await bootstrapper.LoadConfigurations(_serviceProvider);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += Desktop_ShutdownRequested;

            var shellView = _serviceProvider.GetService<ShellView>();
            shellView!.DataContext = _serviceProvider.GetService<ShellViewModel>();
            desktop.MainWindow = shellView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Desktop_ShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        // Ensures logs are flushed
        _serviceProvider?.Dispose();
    }
}
