using Avalonia.Controls;
using Avalonia;
using System;
using Avalonia.Media;

namespace NonogramAvalonia.Controls;

/// <summary>
/// Based on UniformGrid with better support for spacing and ensuring each element has a whole integer size instead of fractional
/// </summary>
public class BoardGrid : Panel
{
    /// <summary>
    /// Defines the <see cref="Rows"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<BoardGrid, int>(nameof(Rows));

    /// <summary>
    /// Defines the <see cref="Columns"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<BoardGrid, int>(nameof(Columns));

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RowSpacingProperty =
        AvaloniaProperty.Register<BoardGrid, int>(nameof(RowSpacing));

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnSpacingProperty =
        AvaloniaProperty.Register<BoardGrid, int>(nameof(ColumnSpacing));

    /// <summary>
    /// Defines the <see cref="Padding"/> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<Decorator, Thickness>(nameof(Padding));

    /// <summary>
    /// Defines the <see cref="DoubleSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> DoubleSpacingProperty =
        AvaloniaProperty.Register<BoardGrid, int>(nameof(DoubleSpacing));

    private int _rows;
    private int _columns;

    static BoardGrid()
    {
        AffectsMeasure<BoardGrid>(RowsProperty, ColumnsProperty);
    }

    /// <summary>
    /// Specifies the row count. If set to 0, row count will be calculated automatically.
    /// </summary>
    public int Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    /// <summary>
    /// Specifies the column count. If set to 0, column count will be calculated automatically.
    /// </summary>
    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Specifies the row spacing.
    /// </summary>
    public int RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    /// <summary>
    /// Specifies the row spacing.
    /// </summary>
    public int ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    /// <summary>
    /// Specifies the spacing around the outside of the elements.
    /// </summary>
    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// Use double spacing of rows/columns every n rows/columns
    /// </summary>
    public int DoubleSpacing
    {
        get => GetValue(DoubleSpacingProperty);
        set => SetValue(DoubleSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateRowsAndColumns();

        var maxWidth = 0d;
        var maxHeight = 0d;

        var (rowSpace, columnSpace) = GetRowColumnSpace();

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

        var (rowSpace, columnSpace) = GetRowColumnSpace();

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

    private (int RowSpace, int ColumnSpace) GetRowColumnSpace()
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