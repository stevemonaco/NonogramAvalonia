namespace NonogramAvalonia.Converters;
public static class AppConverters
{
    public static EnumToBooleanConverter EnumToBoolean { get; } = new();
    public static TimeSpanToTimerStringConverter TimeSpanToTimerString { get; } = new();
}
