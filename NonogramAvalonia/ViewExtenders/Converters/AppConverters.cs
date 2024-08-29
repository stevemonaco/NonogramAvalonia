using Avalonia.Data.Converters;
using NonogramAvalonia.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.Converters;
public static class AppConverters
{
    public static EnumToBooleanConverter EnumToBoolean { get; } = new();
    public static TimeSpanToTimerStringConverter TimeSpanToTimerString { get; } = new();

    public static IValueConverter RowConstraintToStrings { get; } =
        new FuncValueConverter<IList<LineConstraints>, IEnumerable<string>>(x => GenerateConstraintStrings(x, " "));

    public static IValueConverter ColumnConstraintToStrings { get; } =
        new FuncValueConverter<IList<LineConstraints>, IEnumerable<string>>(x => GenerateConstraintStrings(x, "\n"));

    private static IEnumerable<string> GenerateConstraintStrings(IEnumerable<LineConstraints> lineConstraints, string separator)
    {
        foreach (var constraints in lineConstraints)
            yield return string.Join(separator, constraints.Select(x => x.ToString("d")));
    }
}
