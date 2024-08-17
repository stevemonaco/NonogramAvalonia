using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Controls;
public partial class Cell
{
    public static readonly StyledProperty<CellState> StateProperty =
        AvaloniaProperty.Register<Cell, CellState>(nameof(State));

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(Fill));

    public CellState? State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    public IBrush? Fill
    {
        get { return GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }
}
