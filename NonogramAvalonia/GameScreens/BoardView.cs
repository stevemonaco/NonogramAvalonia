using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using NonogramAvalonia.Settings;
using NonogramAvalonia.ViewModels;
using System.Collections.Generic;

namespace NonogramAvalonia.Views;

/// <summary>
/// Base functionality for the Editor and Play boards
/// </summary>
public abstract class BoardView : UserControl
{
    public static readonly StyledProperty<BoardTheme> BoardThemeProperty =
        AvaloniaProperty.Register<BoardPlayView, BoardTheme>(nameof(BoardTheme), defaultValue: BoardTheme.Default);

    public BoardTheme BoardTheme
    {
        get => GetValue(BoardThemeProperty);
        set => SetValue(BoardThemeProperty, value);
    }

    public List<BoardTheme> AvailableThemes { get; } = [BoardTheme.Default, BoardTheme.Plain];

    protected BoardViewModel ViewModel => (BoardViewModel)DataContext!;
    private AppSettings _settings;

    protected BoardView()
    {
        _settings = Ioc.Default.GetRequiredService<AppSettings>();
        BoardTheme = _settings.CurrentBoardTheme;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S)
        {
            if (ViewModel.Mode == BoardMode.Play)
                ViewModel.SolveBoard();
            else if (ViewModel.Mode == BoardMode.Editor)
                ViewModel.TestSolveBoard();
        }
        else if (e.Key == Key.Z)
        {
            ViewModel.TryUndo();
        }
        else if (e.Key == Key.Y || e.Key == Key.R)
        {
            ViewModel.TryRedo();
        }
        else if (e.Key == Key.T)
        {
            var index = (AvailableThemes.IndexOf(BoardTheme) + 1) % AvailableThemes.Count;
            BoardTheme = AvailableThemes[index];
            _settings.CurrentBoardTheme = BoardTheme;
        }
        else
        {
            base.OnKeyDown(e);
        }
    }

    protected void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && sender is Control { DataContext: CellViewModel cell })
        {
            var props = e.GetCurrentPoint(null).Properties;

            TransitionAction action;

            if (props.IsLeftButtonPressed)
                action = TransitionAction.Primary;
            else if (props.IsRightButtonPressed)
                action = TransitionAction.Secondary;
            else
                return;

            ViewModel.TryStartCellTransition(cell, action);
            ViewModel.TryApplyCellTransition(cell);

            e.Pointer.Capture(null); // Release capture so that the mouse can color multiple cells while held and dragged
            e.Handled = true;
        }
    }

    protected void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
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

    protected void OnPointerMoved(object? sender, PointerEventArgs e)
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
}
