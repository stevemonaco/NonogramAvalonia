using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.Factory;
public sealed class BoardViewModelFactory
{
    private readonly NonogramService _nonogramService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    public BoardViewModelFactory(NonogramService nonogramService, IFileSelectService fileSelectService, SolverService solverService)
    {
        _nonogramService = nonogramService;
        _fileSelectService = fileSelectService;
        _solverService = solverService;
    }

    public BoardViewModel CreatePlay(NonogramViewModel board)
    {
        return new BoardViewModel(BoardMode.Play, board, _nonogramService, _fileSelectService, _solverService);
    }

    public BoardViewModel CreateEditor(NonogramViewModel board)
    {
        return new BoardViewModel(BoardMode.Editor, board, _nonogramService, _fileSelectService, _solverService);
    }
}
