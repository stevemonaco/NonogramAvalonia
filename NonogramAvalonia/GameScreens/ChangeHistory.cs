using System.Collections.Generic;

namespace NonogramAvalonia.GameScreens;

/// <summary>
/// Manages a change history collection for reversible diffs
/// Caller is responsible for restoring state
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
        var undoState = _undoHistory.Pop();
        _redoHistory.Push(undoState);

        return undoState;
    }

    public virtual T PopRedoState()
    {
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
