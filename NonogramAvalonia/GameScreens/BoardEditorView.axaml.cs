using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using NonogramAvalonia.ViewModels;
using System;
using System.Linq;

namespace NonogramAvalonia.Views;
public partial class BoardEditorView : UserControl
{
    internal BoardViewModel ViewModel => (BoardViewModel)DataContext!;

    public BoardEditorView ()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Focus();

        int spacing = 5;

        foreach (var outerBorder in board.GetLogicalChildren().Select(x => x.LogicalChildren.OfType<Border>().First()))
        {
            var cell = (CellViewModel)outerBorder.DataContext!;

            bool hasTopEdge = cell.Row % spacing == 0 && cell.Row != ViewModel.GridRows && cell.Row != 0;
            bool hasBottomEdge = (cell.Row + 1) % spacing == 0 && (cell.Row + 1) != ViewModel.GridRows && cell.Row != 0;
            bool hasLeftEdge = (cell.Column + 1) % spacing == 0 && (cell.Column + 1) != ViewModel.GridColumns && cell.Column != 0;
            bool hasRightEdge = cell.Column % spacing == 0 && cell.Column != ViewModel.GridColumns && cell.Column != 0;

            var className = (hasTopEdge, hasBottomEdge, hasLeftEdge, hasRightEdge) switch
            {
                (true, false, false, false) => "top-edge",
                (false, true, false, false) => "bottom-edge",
                (false, false, true, false) => "left-edge",
                (false, false, false, true) => "right-edge",

                (true, false, true, false) => "top-left-edge",
                (true, false, false, true) => "top-right-edge",
                (false, true, true, false) => "bottom-left-edge",
                (false, true, false, true) => "bottom-right-edge",
                (false, false, false, false) => "none",
                _ => throw new InvalidOperationException()
            };

            if (className != "none")
                outerBorder.Classes.Add(className);
        }
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
}