using CommunityToolkit.Mvvm.ComponentModel;

namespace NonogramAvalonia.Settings;
public sealed partial class AppSettings : ObservableObject
{
    [ObservableProperty] private BoardTheme _currentBoardTheme = BoardTheme.Default;
}
