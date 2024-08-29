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
    public NonogramViewModel Nonogram { get; }
    public BoardMode Mode { get; }

    [ObservableProperty] private bool _isSolved;
    [ObservableProperty] private TimeSpan? _timeElapsed;
    [ObservableProperty] private bool _showConstraints;

    protected CellTransition _transition;
    private readonly BoardService _boardService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    public BoardViewModel(BoardMode mode, NonogramViewModel nonogramViewModel, BoardService boardService, IFileSelectService fileSelectService, SolverService solverService)
    {
        Mode = mode;
        Nonogram = nonogramViewModel;
        _boardService = boardService;
        _fileSelectService = fileSelectService;
        _solverService = solverService;
        ShowConstraints = true;
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

        Nonogram.RebuildPlayerConstraints();

        if (Mode == BoardMode.Play && Nonogram.CheckWinState())
            IsSolved = true;

        if (Mode == BoardMode.Editor)
            Nonogram.RebuildPlayerConstraints();

        return true;
    }

    public virtual bool TryStartCellTransition(CellViewModel cell, bool secondary)
    {
        if (cell.Locked || IsSolved)
            return false;

        Nonogram.RebuildPlayerConstraints();

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
            Nonogram.RebuildPlayerConstraints();
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

        Nonogram.Name = Path.GetFileNameWithoutExtension(fileName);
        Nonogram.RebuildPlayerConstraints();
        var json = _boardService.SerializeBoard(Nonogram);
        await File.WriteAllTextAsync(fileName, json);
    }

    [RelayCommand]
    public void SolveBoard()
    {
        _solverService.SolveBoard(Nonogram, true);
    }

    partial void OnIsSolvedChanged(bool value)
    {
        if (value is true)
        {
            Messenger.Send(new GameWinMessage());
        }
    }
}
