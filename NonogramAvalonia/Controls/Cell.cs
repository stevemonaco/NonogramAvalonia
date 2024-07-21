using Avalonia;
using Avalonia.Controls.Primitives;
using NonogramAvalonia.ViewModels;
using System;

namespace NonogramAvalonia.Controls;

public class Cell : TemplatedControl
{
    public static readonly StyledProperty<CellState> StateProperty =
        AvaloniaProperty.Register<Cell, CellState>(nameof(State));

    public CellState? State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    static Cell()
    {
        StateProperty.Changed.Subscribe(OnStateChanged);
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
