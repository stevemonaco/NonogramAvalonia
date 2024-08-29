using Avalonia.Controls;
using Avalonia;
using System;
using Avalonia.Media;

namespace NonogramAvalonia.Controls;

/// <summary>
/// UniformGrid with better support for spacing and ensuring each element has a whole integer size instead of fractional
/// </summary>
/// <remarks>Implementation based upon UniformGrid source</remarks>
public partial class SpacedUniformGrid : Panel
{
    private int _rows;
    private int _columns;

    static SpacedUniformGrid()
    {
        AffectsMeasure<SpacedUniformGrid>(RowsProperty, ColumnsProperty, RowSpacingProperty, ColumnSpacingProperty, PaddingProperty, DoubleSpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateRowsAndColumns();

        var maxWidth = 0d;
        var maxHeight = 0d;

        var (rowSpace, columnSpace) = CalculateTotalSpacing();

        var availableWidth = Math.Floor((availableSize.Width - columnSpace - Padding.Left - Padding.Right) / _columns);
        var availableHeight = Math.Floor((availableSize.Height - rowSpace - Padding.Top - Padding.Bottom) / _rows);
        var childAvailableSize = new Size(availableWidth, availableHeight);

        foreach (var child in Children)
        {
            child.Measure(childAvailableSize);

            if (child.DesiredSize.Width > maxWidth)
            {
                maxWidth = child.DesiredSize.Width;
            }

            if (child.DesiredSize.Height > maxHeight)
            {
                maxHeight = child.DesiredSize.Height;
            }
        }

        return new Size(maxWidth * _columns + columnSpace + Padding.Left + Padding.Right, maxHeight * _rows + rowSpace + Padding.Top + Padding.Bottom);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var x = 0;
        var y = 0;

        var (rowSpace, columnSpace) = CalculateTotalSpacing();

        var childWidth = Math.Floor((finalSize.Width - columnSpace - Padding.Left - Padding.Right) / _columns);
        var childHeight = Math.Floor((finalSize.Height - rowSpace - Padding.Top - Padding.Bottom) / _rows);

        var fullWidth = columnSpace + childWidth * _columns + Padding.Left + Padding.Right;
        var fullHeight = rowSpace + childHeight * _rows + Padding.Top + Padding.Bottom;

        Clip = new RectangleGeometry(new Rect(0, 0, fullWidth, fullHeight));

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }

            var xPos = x * childWidth + ColumnSpacing * x + Padding.Left;
            var yPos = y * childHeight + RowSpacing * y + Padding.Top;

            if (DoubleSpacing != 0)
            {
                xPos += ColumnSpacing * (x / DoubleSpacing);
                yPos += RowSpacing * (y / DoubleSpacing);
            }

            child.Arrange(new Rect(xPos, yPos, childWidth, childHeight));

            x++;

            if (x >= _columns)
            {
                x = 0;
                y++;
            }
        }

        return finalSize;
    }

    /// <summary>
    /// Calculates required extra space to accomodate the spacing across the entire grid. Does not account for padding.
    /// </summary>
    private (int RowSpace, int ColumnSpace) CalculateTotalSpacing()
    {
        var rowSpace = RowSpacing * (_rows - 1);
        var columnSpace = ColumnSpacing * (_columns - 1);

        if (DoubleSpacing > 0)
        {
            rowSpace += RowSpacing * ((_rows - 1) / DoubleSpacing);
            columnSpace += ColumnSpacing * ((_columns - 1) / DoubleSpacing);
        }

        return (rowSpace, columnSpace);
    }

    private void UpdateRowsAndColumns()
    {
        _rows = Rows;
        _columns = Columns;

        var itemCount = 0;

        foreach (var child in Children)
        {
            if (child.IsVisible)
            {
                itemCount++;
            }
        }

        if (_rows == 0)
        {
            if (_columns == 0)
            {
                _rows = _columns = (int)Math.Ceiling(Math.Sqrt(itemCount));
            }
            else
            {
                _rows = Math.DivRem(itemCount, _columns, out int rem);

                if (rem != 0)
                {
                    _rows++;
                }
            }
        }
        else if (_columns == 0)
        {
            _columns = Math.DivRem(itemCount, _rows, out int rem);

            if (rem != 0)
            {
                _columns++;
            }
        }
    }
}