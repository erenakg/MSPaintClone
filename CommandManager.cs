namespace MSPaintClone;

/// <summary>
/// Manages command execution with undo/redo support using the Command Pattern.
/// </summary>
public class CommandManager
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();

    /// <summary>
    /// Gets whether there are commands available to undo.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Gets whether there are commands available to redo.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Executes a command and adds it to the undo stack.
    /// Clears the redo stack since a new action invalidates the redo history.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear(); // New action invalidates redo history
    }

    /// <summary>
    /// Undoes the most recent command.
    /// </summary>
    public void Undo()
    {
        if (!CanUndo)
            return;

        var command = _undoStack.Pop();
        command.UnExecute();
        _redoStack.Push(command);
    }

    /// <summary>
    /// Redoes the most recently undone command.
    /// </summary>
    public void Redo()
    {
        if (!CanRedo)
            return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
    }

    /// <summary>
    /// Clears all undo and redo history.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
