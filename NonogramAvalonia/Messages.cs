using Nonogram.Domain;

namespace NonogramAvalonia;

public record NavigateToPlayMessage(NonogramBoard Board);
public record NavigateToCreateMessage();
public record NavigateToMenuMessage();

public record GameOpenedMessage();
public record GameStartedMessage();
public record GameQuitMessage();
public record GameWinMessage();
