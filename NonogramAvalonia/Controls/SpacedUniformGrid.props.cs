using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramAvalonia.Controls;
public partial class SpacedUniformGrid
{
    /// <summary>
    /// Defines the <see cref="Rows"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<SpacedUniformGrid, int>(nameof(Rows));

    /// <summary>
    /// Defines the <see cref="Columns"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<SpacedUniformGrid, int>(nameof(Columns));

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> RowSpacingProperty =
        AvaloniaProperty.Register<SpacedUniformGrid, int>(nameof(RowSpacing));

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnSpacingProperty =
        AvaloniaProperty.Register<SpacedUniformGrid, int>(nameof(ColumnSpacing));

    /// <summary>
    /// Defines the <see cref="Padding"/> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<Decorator, Thickness>(nameof(Padding));

    /// <summary>
    /// Defines the <see cref="DoubleSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<int> DoubleSpacingProperty =
        AvaloniaProperty.Register<SpacedUniformGrid, int>(nameof(DoubleSpacing));

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
    /// Applies double the spacing of rows/columns every n rows/columns
    /// </summary>
    public int DoubleSpacing
    {
        get => GetValue(DoubleSpacingProperty);
        set => SetValue(DoubleSpacingProperty, value);
    }
}
