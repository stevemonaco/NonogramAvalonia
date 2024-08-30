using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Services;
using NonogramAvalonia.Mappers;


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
    private readonly SerializationService _boardService;
    private readonly IFileSelectService _fileSelectService;
    private readonly SolverService _solverService;

    public BoardViewModel(BoardMode mode, NonogramViewModel nonogramViewModel, SerializationService boardService, IFileSelectService fileSelectService, SolverService solverService)
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

        Nonogram.RebuildPlayerConstraints();
        if (string.IsNullOrEmpty(Nonogram.Name))
            Nonogram.Name = Path.GetFileNameWithoutExtension(fileName);

        var model = Nonogram.ToModel();
        var json = _boardService.SerializeNonogram(model);
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
