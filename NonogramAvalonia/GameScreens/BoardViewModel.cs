using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.ViewModels;

public enum BoardMode { Play, Editor }
public enum CellTransition { None, ToUndetermined, ToEmpty, ToFilled }

public partial class BoardViewModel : ObservableRecipient
{
    public NonogramViewModel Board { get; }
    public BoardMode Mode { get; }

    [ObservableProperty] private int _gridRows;
    [ObservableProperty] private int _gridColumns;

    [ObservableProperty] private ObservableCollection<CellViewModel> _boardCells;
    [ObservableProperty] private string? _puzzleName;
    [ObservableProperty] private bool _isSolved;
    [ObservableProperty] private TimeSpan? _timeElapsed;
    [ObservableProperty] private bool _showConstraints;

    public IEnumerable<string> SolutionRowConstraints => GenerateConstraintStrings(Board.SolutionRowConstraints, " ");
    public IEnumerable<string> SolutionColumnConstraints => GenerateConstraintStrings(Board.SolutionColumnConstraints, "\n");
    public IEnumerable<string> RowConstraints => GenerateConstraintStrings(Board.PlayerRowConstraints, " ");
    public IEnumerable<string> ColumnConstraints => GenerateConstraintStrings(Board.PlayerColumnConstraints, "\n");

    protected CellTransition _transition;
    private readonly BoardService _boardService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    public BoardViewModel(BoardMode mode, NonogramViewModel board, BoardService boardService, IFileSelectService fileSelectService, SolverService solverService)
    {
        Mode = mode;
        Board = board;
        _boardService = boardService;
        _fileSelectService = fileSelectService;
        _solverService = solverService;
        ShowConstraints = true;

        _boardCells = new(Board.Board);
        
        _puzzleName = board.Name;
        GridRows = Board.Rows;
        GridColumns = Board.Columns;
    }

    public virtual bool TryApplyCellTransition(CellViewModel cell)
    {
        if (cell.Locked || IsSolved || _transition == CellTransition.None)
            return false;

        if (_transition == CellTransition.None)
            return false;

        cell.CellState = _transition switch
        {
            CellTransition.ToUndetermined => CellState.Undetermined,
            CellTransition.ToEmpty => CellState.Empty,
            CellTransition.ToFilled => CellState.Filled,
            _ => throw new InvalidOperationException($"{nameof(TryApplyCellTransition)} attempted to apply invalid transition {_transition}")
        };

        Board.BuildConstraints();

        if (Mode == BoardMode.Play && Board.CheckWinState())
            IsSolved = true;

        if (Mode == BoardMode.Editor)
            UpdateConstraints();

        return true;
    }

    public virtual bool TryStartCellTransition(CellViewModel cell, bool secondary)
    {
        if (cell.Locked || IsSolved)
            return false;

        Board.BuildConstraints();

        _transition = (cell.CellState, secondary) switch
        {
            (CellState.Undetermined, false) => CellTransition.ToFilled,
            (CellState.Undetermined, true) => CellTransition.ToEmpty,
            (CellState.Empty, false) => CellTransition.ToFilled,
            (CellState.Empty, true) => CellTransition.ToUndetermined,
            (CellState.Filled, false) => CellTransition.ToUndetermined,
            (CellState.Filled, true) => CellTransition.ToEmpty,
            _ => CellTransition.None
        };

        if (Mode == BoardMode.Editor)
        {
            UpdateConstraints();
        }

        return true;
    }

    public void EndCellTransition()
    {
        _transition = CellTransition.None;
    }

    [RelayCommand]
    public async Task SaveBoard()
    {
        var fileName = await _fileSelectService.RequestSaveBoardFileNameAsync();

        if (fileName is null)
            return;

        Board.Name = Path.GetFileNameWithoutExtension(fileName);
        Board.BuildConstraints();
        var json = _boardService.SerializeBoard(Board);
        await File.WriteAllTextAsync(fileName, json);
    }

    [RelayCommand]
    public void SolveBoard()
    {
        _solverService.SolveBoard(Board, true);
    }

    public void OnIsSolvedChanged(bool value)
    {
        if (value is true)
        {
            Messenger.Send(new GameWinMessage());
        }
    }

    private void UpdateConstraints()
    {
        Board.BuildConstraints();
        OnPropertyChanged(nameof(SolutionRowConstraints));
        OnPropertyChanged(nameof(SolutionColumnConstraints));
        OnPropertyChanged(nameof(RowConstraints));
        OnPropertyChanged(nameof(ColumnConstraints));
    }

    private IEnumerable<string> GenerateConstraintStrings(IEnumerable<LineConstraints> lineConstraints, string separator)
    {
        foreach (var constraints in lineConstraints)
            yield return string.Join(separator, constraints.Select(x => x.ToString("d")));
    }
}
