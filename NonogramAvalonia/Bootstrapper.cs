using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using NonogramAvalonia.Services;
using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Factory;
using System.IO;

namespace NonogramAvalonia;
public interface IAppBootstrapper<TViewModel> where TViewModel : class
{
    void ConfigureServices(IServiceCollection services);
    void ConfigureViews(IServiceCollection services);
    void ConfigureViewModels(IServiceCollection services);

    ValueTask<bool> LoadConfigurations(IServiceProvider provider);
}

public class Bootstrapper : IAppBootstrapper<ShellViewModel>
{
    private ILoggerFactory? _loggerFactory;
    private const string _logFileName = @"log.txt";

    public void ConfigureIoc(IServiceCollection services)
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFileName);
        _loggerFactory = CreateLoggerFactory(logPath);

        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<SerializationService>();
        services.AddTransient<SolverService>();
        services.AddTransient<IFileSelectService, FileSelectService>();
        services.AddTransient<BoardViewModelFactory>();
    }

    public void ConfigureViews(IServiceCollection services)
    {
        var viewTypes = GetType()
            .Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith("View"))
            .Where(x => !x.IsAbstract && !x.IsInterface);

        foreach (var viewType in viewTypes)
            services.AddTransient(viewType);
    }

    public void ConfigureViewModels(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();

        var vmTypes = GetType()
            .Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith("ViewModel"))
            .Where(x => !x.IsAbstract && !x.IsInterface);

        foreach (var vmType in vmTypes)
            services.TryAddTransient(vmType);
    }

    private ILoggerFactory CreateLoggerFactory(string logName)
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.File(logName, rollingInterval: RollingInterval.Month,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}");

        Log.Logger = config.CreateLogger();

        var factory = new LoggerFactory();
        return factory;
    }

    public ValueTask<bool> LoadConfigurations(IServiceProvider provider)
    {
        return ValueTask.FromResult(true);
    }
}
