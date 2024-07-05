using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace NonogramAvalonia.Controls;

public partial class OutlinedText : TextBlock
{
    internal Geometry? _geometry;

    static OutlinedText()
    {
        ClipToBoundsProperty.OverrideDefaultValue<OutlinedText>(true);

        var items = new AvaloniaProperty[]
        {
            BackgroundProperty, ForegroundProperty, StrokeProperty, StrokeDashArrayProperty, StrokeDashOffsetProperty,
            StrokeJoinProperty, StrokeLineCapProperty, StrokeThicknessProperty
        };

        AffectsRender<OutlinedText>(items);
    }

    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        _geometry = _geometry ?? CreateTextGeometry(Text);

        ImmutablePen? pen = null;

        if (Stroke is not null)
        {
            var strokeDashArray = StrokeDashArray;

            ImmutableDashStyle? dashStyle = null;

            if (strokeDashArray is not null && strokeDashArray.Count > 0)
            {
                dashStyle = new ImmutableDashStyle(strokeDashArray, StrokeDashOffset);
            }

            pen = new ImmutablePen(Stroke.ToImmutable(), StrokeThickness, dashStyle, StrokeLineCap, StrokeJoin);
        }

        context.DrawGeometry(Foreground, pen, _geometry!);
    }

    protected virtual Geometry? CreateTextGeometry(string? text)
    {
        var formattedText = new FormattedText(text ?? "", CultureInfo.GetCultureInfo("en-us"), FlowDirection,
            new Typeface(FontFamily, FontStyle, FontWeight), FontSize, Foreground);

        return formattedText.BuildGeometry(new Point(0, 0));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        switch (change.Property.Name)
        {
            case nameof(Stroke):
            case nameof(StrokeJoin):
            case nameof(StrokeDashArray):
            case nameof(StrokeDashOffset):
            case nameof(StrokeThickness):
            case nameof(StrokeLineCap):
            {
                InvalidateTextLayout();
                break;
            }
        }
    }
}
