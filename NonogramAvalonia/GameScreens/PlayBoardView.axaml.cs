using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Nonogram.Domain;
using NonogramAvalonia.ViewModels;
using System;

namespace NonogramAvalonia.Views;

public partial class PlayBoardView : UserControl, 
    IRecipient<GameStartedMessage>, IRecipient<GameWinMessage>, IRecipient<GameQuitMessage>
{
    internal PlayBoardViewModel ViewModel => (PlayBoardViewModel)DataContext!;
    private readonly DispatcherTimer _timer;
    private DateTime _timeStarted;

    public PlayBoardView()
    {
        InitializeComponent();

        _timer = new(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Timer_Tick);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && sender is Control { DataContext: NonogramCell cell })
        {
            var props = e.GetCurrentPoint(null).Properties;
            var secondary = props.IsRightButtonPressed;

            ViewModel.TryStartCellTransition(cell, secondary);
            ViewModel.TryApplyCellTransition(cell);

            e.Pointer.Capture(null); // Release capture so that the mouse can color multiple cells while pressed
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse)
        {
            var props = e.GetCurrentPoint(null).Properties;
            var anyPressed = props.IsLeftButtonPressed || props.IsMiddleButtonPressed || props.IsRightButtonPressed;

            if (!anyPressed)
                ViewModel.EndCellTransition();
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && sender is Control { DataContext: NonogramCell cell })
        {
            var props = e.GetCurrentPoint(null).Properties;
            var anyPressed = props.IsLeftButtonPressed || props.IsMiddleButtonPressed || props.IsRightButtonPressed;

            if (!anyPressed)
            {
                ViewModel.EndCellTransition();
                return;
            }

            ViewModel.TryApplyCellTransition(cell);
            e.Handled = true;
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

    public void Receive(GameQuitMessage message)
    {
        _timer.Stop();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}