using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NonogramAvalonia.ViewModels;
using System.Collections.Generic;

namespace NonogramAvalonia.Views;
public class BoardView : UserControl
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

    protected void OnPointerPressed(object? sender, PointerPressedEventArgs e)
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
