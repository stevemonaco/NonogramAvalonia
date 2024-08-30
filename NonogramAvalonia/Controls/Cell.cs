using Avalonia;
using Avalonia.Controls.Primitives;
using NonogramAvalonia.ViewModels;
using System;

namespace NonogramAvalonia.Controls;

public partial class Cell : TemplatedControl
{
    static Cell()
    {
        StateProperty.Changed.Subscribe(OnStateChanged);

        var items = new AvaloniaProperty[]
        {
            StateProperty, FillProperty
        };

        AffectsRender<Cell>(items);
    }

    private static void OnStateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is Cell cell)
        {
            cell.Classes.Set(":undetermined", cell.State == CellState.Undetermined);
            cell.Classes.Set(":empty", cell.State == CellState.Empty);
            cell.Classes.Set(":filled", cell.State == CellState.Filled);
        }
    }
}
