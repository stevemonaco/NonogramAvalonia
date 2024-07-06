using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;
public partial class ShellView : Window
{
    internal ShellViewModel ViewModel => (ShellViewModel)DataContext!;

    public ShellView()
    {
        InitializeComponent();
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        await ViewModel.InitializeAsync();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape && ViewModel.ActiveScreen is PlayBoardViewModel)
        {
            WeakReferenceMessenger.Default.Send(new GameQuitMessage());
            WeakReferenceMessenger.Default.Send(new NavigateToMenuMessage());
        }
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
}
