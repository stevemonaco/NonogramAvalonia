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

        if (Fill is not null)
            context.FillRectangle(Fill, Bounds);

        var bounds = Bounds; // Bounds.Deflate(new Thickness(BorderThickness.Left / 2, BorderThickness.Top / 2, BorderThickness.Right / 2, BorderThickness.Bottom / 2));


        var leftPen = new ImmutablePen(GetColor(LeftBrush), BorderThickness.Left);
        context.DrawLine(leftPen, bounds.TopLeft, bounds.BottomLeft);

        var rightPen = new ImmutablePen(GetColor(RightBrush), BorderThickness.Right);
        context.DrawLine(rightPen, bounds.TopRight, bounds.BottomRight);

        var topPen = new ImmutablePen(GetColor(TopBrush), BorderThickness.Top);
        context.DrawLine(topPen, bounds.TopLeft, bounds.TopRight);

        var bottomPen = new ImmutablePen(GetColor(BottomBrush), BorderThickness.Bottom);
        context.DrawLine(bottomPen, bounds.BottomLeft, bounds.BottomRight);

        clipRectState.Dispose();
    }

    private static uint GetColor(IBrush? brush) => ((ISolidColorBrush)brush!).Color.ToUInt32();
}
