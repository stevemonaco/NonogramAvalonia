using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Views;
using RagnaRoute;

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
        ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);

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
            var shellViewModel = provider.GetService<ShellViewModel>();
            var shellView = provider.GetService<ShellView>();
            shellView!.DataContext = shellViewModel;
            desktop.MainWindow = shellView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
