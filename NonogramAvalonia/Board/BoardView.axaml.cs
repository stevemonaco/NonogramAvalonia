using System;
using Avalonia.Controls;
using Avalonia.Input;
using Nonogram.Domain;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;
public partial class BoardView : UserControl
{
    private BoardViewModel _viewModel = null!;

    public BoardView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not null)
        {
            _viewModel = (BoardViewModel)DataContext;
        }

        base.OnDataContextChanged(e);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && _viewModel is not null && sender is Control { DataContext: NonogramCell cell })
        {
            var props = e.GetCurrentPoint(null).Properties;
            var secondary = props.IsRightButtonPressed;

            _viewModel.TryStartCellTransition(cell, secondary);
            _viewModel.TryApplyCellTransition(cell);

            e.Pointer.Capture(null); // Release capture so that the mouse can color multiple cells while pressed
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && _viewModel is not null)
        {
            var props = e.GetCurrentPoint(null).Properties;
            var anyPressed = props.IsLeftButtonPressed || props.IsMiddleButtonPressed || props.IsRightButtonPressed;

            if (!anyPressed)
                _viewModel.EndCellTransition();
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse && _viewModel is not null && sender is Control { DataContext: NonogramCell cell })
        {
            var props = e.GetCurrentPoint(null).Properties;
            var anyPressed = props.IsLeftButtonPressed || props.IsMiddleButtonPressed || props.IsRightButtonPressed;

            if (!anyPressed)
            {
                _viewModel.EndCellTransition();
                return;
            }

            _viewModel.TryApplyCellTransition(cell);
            e.Handled = true;
        }
    }
}
