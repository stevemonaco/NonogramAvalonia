using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia;

public record NavigateToPlayMessage(NonogramViewModel Board);
public record NavigateToEditorMessage(int Rows, int Columns);
public record NavigateToMenuMessage();

public record GameOpenedMessage();
public record GameStartedMessage();
public record GameQuitMessage();
public record GameWinMessage();
