using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using NonogramAvalonia.ViewModels;
using System;

namespace NonogramAvalonia.Controls;

public partial class Cell : Control
{
    static Cell()
    {
        StateProperty.Changed.Subscribe(OnStateChanged);

        var items = new AvaloniaProperty[]
        {
            StateProperty, FillProperty, BorderThicknessProperty, CornerRadiusProperty,
            LeftBrushProperty, RightBrushProperty, TopBrushProperty, BottomBrushProperty
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

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var clipRect = new RoundedRect(Bounds, CornerRadius);
        var clipRectState = context.PushClip(clipRect);

        var bounds = Bounds.Deflate(new Thickness(BorderThickness.Left, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom));

        if (Fill is not null)
            context.FillRectangle(Fill, bounds);


        var leftPen = new ImmutablePen(GetColor(LeftBrush), BorderThickness.Left);
        context.DrawLine(leftPen, Bounds.TopLeft, Bounds.BottomLeft);

        var rightPen = new ImmutablePen(GetColor(RightBrush), BorderThickness.Right);
        context.DrawLine(rightPen, Bounds.TopRight, Bounds.BottomRight);

        var topPen = new ImmutablePen(GetColor(TopBrush), BorderThickness.Top);
        context.DrawLine(topPen, Bounds.TopLeft, Bounds.TopRight);

        var bottomPen = new ImmutablePen(GetColor(BottomBrush), BorderThickness.Bottom);
        context.DrawLine(bottomPen, Bounds.BottomLeft, Bounds.BottomRight);

        clipRectState.Dispose();
    }

    private static uint GetColor(IBrush? brush) => ((ISolidColorBrush)brush!).Color.ToUInt32();
}
