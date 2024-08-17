using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Controls;
using NonogramAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.Views;
public partial class BoardPlayView : UserControl, 
    IRecipient<GameStartedMessage>, IRecipient<GameWinMessage>, IRecipient<GameQuitMessage>
{
    public static readonly StyledProperty<BoardTheme> BoardThemeProperty =
        AvaloniaProperty.Register<BoardPlayView, BoardTheme>(nameof(BoardTheme), defaultValue: BoardTheme.Default);

    public BoardTheme BoardTheme
    {
        get => GetValue(BoardThemeProperty);
        set => SetValue(BoardThemeProperty, value);
    }

    private List<BoardTheme> _themeCycle = [BoardTheme.Default, BoardTheme.Plain];

    internal BoardViewModel ViewModel => (BoardViewModel)DataContext!;
    private readonly DispatcherTimer _timer;
    private DateTime _timeStarted;

    public BoardPlayView()
    {
        InitializeComponent();

        _timer = new(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Timer_Tick);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S)
        {
            ViewModel.SolveBoard();
        }
        else if (e.Key == Key.T)
        {
            var index = (_themeCycle.IndexOf(BoardTheme) + 1) % _themeCycle.Count;
            BoardTheme = _themeCycle[index];
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && sender is Control { DataContext: CellViewModel cell })
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
        if (e.Pointer.Type == PointerType.Mouse && sender is Control { DataContext: CellViewModel cell })
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
        _timer.Stop();
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;

        foreach (var cellControl in board.GetLogicalChildren().Select(x => x.LogicalChildren.OfType<Cell>().First()))
        {
            cellControl.Classes.Add("win");
        }
    }

    public void Receive(GameQuitMessage message)
    {
        _timer.Stop();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}