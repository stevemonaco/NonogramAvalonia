using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Services;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Factory;
public class BoardViewModelFactory
{
    private readonly BoardService _boardService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    public BoardViewModelFactory(BoardService boardService, IFileSelectService fileSelectService, SolverService solverService)
    {
        _boardService = boardService;
        _fileSelectService = fileSelectService;
        _solverService = solverService;
    }

    public BoardViewModel CreatePlay(NonogramViewModel board)
    {
        return new BoardViewModel(BoardMode.Play, board, _boardService, _fileSelectService, _solverService);
    }

    public BoardViewModel CreateEditor(NonogramViewModel board)
    {
        return new BoardViewModel(BoardMode.Editor, board, _boardService, _fileSelectService, _solverService);
    }
}
