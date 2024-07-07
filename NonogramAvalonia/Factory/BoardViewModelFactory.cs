using Nonogram.Domain;
using NonogramAvalonia.Services;
using NonogramAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramAvalonia.Factory;
public class BoardViewModelFactory
{
    private readonly BoardService _boardService;
    private readonly IFileSelectService _fileSelectService;

    public BoardViewModelFactory(BoardService boardService, IFileSelectService fileSelectService)
    {
        _boardService = boardService;
        _fileSelectService = fileSelectService;
    }

    public BoardViewModel CreatePlay(NonogramBoard board)
    {
        return new BoardViewModel(BoardMode.Play, board, _boardService, _fileSelectService);
    }

    public BoardViewModel CreateEditor(NonogramBoard board)
    {
        return new BoardViewModel(BoardMode.Editor, board, _boardService, _fileSelectService);
    }
}
