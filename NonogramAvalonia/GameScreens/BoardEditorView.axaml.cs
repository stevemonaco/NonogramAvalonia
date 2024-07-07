using Avalonia.Controls;
using Avalonia.Input;
using Nonogram.Domain;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;

public partial class BoardEditorView : UserControl
{
    internal BoardViewModel ViewModel => (BoardViewModel)DataContext!;

    public BoardEditorView ()
    {
        InitializeComponent();
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
}