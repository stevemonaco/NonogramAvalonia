using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;
public partial class ShellView : Window, IRecipient<GameStartedMessage>, IRecipient<GameWinMessage>
{
    internal ShellViewModel ViewModel => (ShellViewModel)DataContext!;
    private readonly DispatcherTimer _timer;
    private DateTime _timeStarted;

    public ShellView()
    {
        InitializeComponent();

        _timer = new(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Timer_Tick);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        await ViewModel.LoadBoardAsync(@"_boards\PicrossDS 1-B.json");
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;
    }

    public void ExitApplication(object? sender, RoutedEventArgs e)
    {
        var canExit = ViewModel.RequestApplicationExit();
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
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;
        _timer.Stop();
    }
}
