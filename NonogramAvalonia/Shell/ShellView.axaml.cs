using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Messages;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;
public partial class ShellView : Window, IRecipient<GameStartedMessage>, IRecipient<GameWinMessage>
{
    private ShellViewModel _viewModel = null!;
    private readonly DispatcherTimer _timer;
    private DateTime _timeStarted;

    public ShellView()
    {
        InitializeComponent();

        _timer = new(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Timer_Tick);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _viewModel.TimeElapsed = DateTime.Now - _timeStarted;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not null)
            _viewModel = (ShellViewModel)DataContext;
    }

    public void ExitApplication(object? sender, RoutedEventArgs e)
    {
        var canExit = _viewModel.RequestApplicationExit();
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        if (canExit)
        {
            if (lifetime is not null)
                lifetime.Shutdown();
            else
                Environment.Exit(0);
        }
    }

    public void Receive(GameStartedMessage message)
    {
        _timeStarted = DateTime.Now;
        _timer.Start();
    }

    public void Receive(GameWinMessage message)
    {
        _viewModel.TimeElapsed = DateTime.Now - _timeStarted;
        _timer.Stop();
    }
}
