using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Services;
using NonogramAvalonia.Mappers;
using System.Collections.Generic;
using NonogramAvalonia.GameScreens;

namespace NonogramAvalonia.ViewModels;

public enum BoardMode { Play, Editor }
public enum SolverState { Undetermined, Failed, Success }
public enum TransitionAction { Primary, Secondary }

public record CellStateChange(int Row, int Column, CellState OldState, CellState NewState);

public sealed partial class BoardViewModel : ObservableRecipient
{
    public NonogramViewModel Nonogram { get; }
    public BoardMode Mode { get; }

    [ObservableProperty] private bool _isSolved;
    [ObservableProperty] private TimeSpan? _timeElapsed;
    [ObservableProperty] private bool _showConstraints;
    [ObservableProperty] private SolverState _solverState;

    private CellState? _transition;
    private readonly SerializationService _boardService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    private ChangeHistory<List<CellStateChange>> _changeHistory = new();
    private List<CellStateChange> _currentChanges = [];

    public BoardViewModel(BoardMode mode, NonogramViewModel nonogramViewModel, SerializationService boardService, IFileSelectService fileSelectService, SolverService solverService)
    {
        Mode = mode;
        Nonogram = nonogramViewModel;
        _boardService = boardService;
        _fileSelectService = fileSelectService;
        _solverService = solverService;

        ShowConstraints = Mode == BoardMode.Play;
    }

    public bool TryApplyCellTransition(CellViewModel cell)
    {
        if (cell.Locked || IsSolved || !_transition.HasValue)
            return false;

        if (cell.CellState == _transition)
        {
            return false;
        }

        _currentChanges.Add(new(cell.Row, cell.Column, cell.CellState, _transition.Value));

        cell.CellState = _transition.Value;
        SolverState = SolverState.Undetermined;

        if (Mode == BoardMode.Play)
            Nonogram.UpdatePlayerConstraints(cell.Row, cell.Column);
        else if (Mode == BoardMode.Editor)
            Nonogram.UpdateSolutionConstraints(cell.Row, cell.Column);

        if (Mode == BoardMode.Play && Nonogram.CheckWinState())
            IsSolved = true;

        return true;
    }

    public bool TryStartCellTransition(CellViewModel cell, TransitionAction action)
    {
        if (cell.Locked || IsSolved || _transition is not null)
            return false;

        _currentChanges = [];

        _transition = (cell.CellState, action) switch
        {
            (CellState.Undetermined, TransitionAction.Primary) => CellState.Filled,
            (CellState.Undetermined, TransitionAction.Secondary) => CellState.Empty,
            (CellState.Empty, TransitionAction.Primary) => CellState.Filled,
            (CellState.Empty, TransitionAction.Secondary) => CellState.Undetermined,
            (CellState.Filled, TransitionAction.Primary) => CellState.Undetermined,
            (CellState.Filled, TransitionAction.Secondary) => CellState.Empty,
            _ => null
        };

        return _transition is CellState;
    }

    public void EndCellTransition()
    {
        _transition = null;

        if (_currentChanges.Count > 0)
        {
            _changeHistory.PushState(_currentChanges);
            _currentChanges = [];
        }
    }

    [RelayCommand]
    public async Task SaveBoard()
    {
        var fileName = await _fileSelectService.RequestSaveBoardFileNameAsync();

        if (fileName is null)
            return;

        Nonogram.RebuildSolutionConstraints();
        if (string.IsNullOrEmpty(Nonogram.Name))
            Nonogram.Name = Path.GetFileNameWithoutExtension(fileName);

        var model = Nonogram.ToModel();
        var json = _boardService.SerializeNonogram(model);
        await File.WriteAllTextAsync(fileName, json);
    }

    /// <summary>
    /// Solves the Board in play and fills the board state
    /// </summary>
    [RelayCommand]
    public void SolveBoard()
    {
        if (IsSolved)
            return;

        IsSolved = _solverService.SolveBoard(Nonogram, true);
    }

    /// <summary>
    /// Tests if the board can be solved with the solver. Does not change the board state.
    /// </summary>
    [RelayCommand]
    public void TestSolveBoard()
    {
        if (_solverService.SolveBoard(Nonogram, false))
            SolverState = SolverState.Success;
        else
            SolverState = SolverState.Failed;
    }

    /// <summary>
    /// Tries to restore the previous Nonogram state
    /// </summary>
    /// <returns></returns>
    public bool TryUndo()
    {
        if (IsSolved || !_changeHistory.CanUndo)
            return false;

        foreach (var change in _changeHistory.PopUndoState())
        {
            Nonogram.SetState(change.Row, change.Column, change.OldState);
        }

        return true;
    }

    /// <summary>
    /// Tries to redo a previously undone Nonogram state
    /// </summary>
    /// <returns></returns>
    public bool TryRedo()
    {
        if (IsSolved || !_changeHistory.CanRedo)
            return false;

        foreach (var change in _changeHistory.PopRedoState())
        {
            Nonogram.SetState(change.Row, change.Column, change.NewState);
        }

        return true;
    }

    partial void OnIsSolvedChanged(bool value)
    {
        if (value is true)
        {
            Messenger.Send(new GameWinMessage());
        }
    }
}
