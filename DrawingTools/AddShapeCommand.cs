using System.Windows;
using System.Windows.Controls;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Command for adding a shape to the canvas, supporting undo/redo.
/// </summary>
public class AddShapeCommand : ICommand
{
    private readonly Canvas _canvas;
    private readonly UIElement _shape;

    public AddShapeCommand(Canvas canvas, UIElement shape)
    {
        _canvas = canvas;
        _shape = shape;
    }

    /// <summary>
    /// Adds the shape to the canvas (or re-adds it for redo).
    /// </summary>
    public void Execute()
    {
        if (!_canvas.Children.Contains(_shape))
        {
            _canvas.Children.Add(_shape);
        }
    }

    /// <summary>
    /// Removes the shape from the canvas (undo).
    /// </summary>
    public void UnExecute()
    {
        _canvas.Children.Remove(_shape);
    }
}
