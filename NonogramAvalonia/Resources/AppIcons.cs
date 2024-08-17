using Avalonia.Media;

namespace NonogramAvalonia.Resources;
public static class AppIcons
{
    // PackIconBootstrapIcons.X
    public static StreamGeometry PlainEmptyIcon { get; set; } = Parse("M5.22675 -4.10175A0.5625 0.5625 0 0 0 6.02325 -4.10175L9 -7.079625L11.97675 -4.10175A0.5625 0.5625 0 0 0 12.77325 -4.89825L9.795375 -7.875L12.77325 -10.85175A0.5625 0.5625 0 0 0 11.97675 -11.64825L9 -8.670375L6.02325 -11.64825A0.5625 0.5625 0 0 0 5.22675 -10.85175L8.204625 -7.875L5.22675 -4.89825A0.5625 0.5625 0 0 0 5.22675 -4.10175z");
    
    private static StreamGeometry Parse(string s) => StreamGeometry.Parse(s);
}
