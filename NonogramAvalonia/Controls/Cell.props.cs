using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using NonogramAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramAvalonia.Controls;
public partial class Cell
{
    public static readonly StyledProperty<CellState> StateProperty =
        AvaloniaProperty.Register<Cell, CellState>(nameof(State));

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(Fill));

    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.Register<Border, Thickness>(nameof(BorderThickness));

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<Border, CornerRadius>(nameof(CornerRadius));

    public static readonly StyledProperty<IBrush?> LeftBrushProperty =
    AvaloniaProperty.Register<Shape, IBrush?>(nameof(LeftBrush));

    public static readonly StyledProperty<IBrush?> RightBrushProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(RightBrush));

    public static readonly StyledProperty<IBrush?> TopBrushProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(TopBrush));

    public static readonly StyledProperty<IBrush?> BottomBrushProperty =
        AvaloniaProperty.Register<Shape, IBrush?>(nameof(BottomBrush));

    public CellState? State
    {
        get { return GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    /// <summary>
    /// Gets or sets the <see cref="IBrush"/> that fills the Cell's bounds.
    /// </summary>
    public IBrush? Fill
    {
        get { return GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public IBrush? LeftBrush
    {
        get { return GetValue(LeftBrushProperty); }
        set { SetValue(LeftBrushProperty, value); }
    }

    public IBrush? RightBrush
    {
        get { return GetValue(RightBrushProperty); }
        set { SetValue(RightBrushProperty, value); }
    }

    public IBrush? TopBrush
    {
        get { return GetValue(TopBrushProperty); }
        set { SetValue(TopBrushProperty, value); }
    }

    public IBrush? BottomBrush
    {
        get { return GetValue(BottomBrushProperty); }
        set { SetValue(BottomBrushProperty, value); }
    }
}
