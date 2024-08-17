using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia;

public record NavigateToPlayMessage(NonogramViewModel Board);
public record NavigateToCreateMessage(int Rows, int Columns);
public record NavigateToMenuMessage();

public record CycleThemeForwardMessage();

public record GameOpenedMessage();
public record GameStartedMessage();
public record GameQuitMessage();
public record GameWinMessage();
