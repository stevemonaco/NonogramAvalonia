using Nonogram.Domain;

namespace NonogramAvalonia;

public record NavigateToPlayMessage(NonogramBoard Board);
public record NavigateToCreateMessage(int Rows, int Columns);
public record NavigateToMenuMessage();

public record GameOpenedMessage();
public record GameStartedMessage();
public record GameQuitMessage();
public record GameWinMessage();
