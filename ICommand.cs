namespace MSPaintClone;

/// <summary>
/// Interface for the Command Pattern, enabling undo/redo functionality.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute();

    /// <summary>
    /// Reverses the command (undo).
    /// </summary>
    void UnExecute();
}
