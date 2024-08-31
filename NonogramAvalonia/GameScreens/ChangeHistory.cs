using System;
using System.Collections.Generic;

namespace NonogramAvalonia.GameScreens;

/// <summary>
/// Manages a change history for reversible diffs
/// </summary>
/// <typeparam name="T"></typeparam>
public class ChangeHistory<T>
{
    private Stack<T> _undoHistory = [];
    private Stack<T> _redoHistory = [];

    public virtual bool CanUndo { get => _undoHistory.Count > 0; }
    public virtual bool CanRedo { get => _redoHistory.Count > 0; }

    public virtual T PopUndoState()
    {
        if (!CanUndo)
            throw new InvalidOperationException();

        var undoState = _undoHistory.Pop();
        _redoHistory.Push(undoState);

        return undoState;
    }

    public virtual T PopRedoState()
    {
        if (!CanRedo)
            throw new InvalidOperationException();

        var redoState = _redoHistory.Pop();
        _undoHistory.Push(redoState);

        return redoState;
    }

    public virtual void PushState(T state)
    {
        _redoHistory.Clear();
        _undoHistory.Push(state);
    }
}
