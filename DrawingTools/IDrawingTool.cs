using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Interface for the Strategy Pattern, defining drawing tool behavior.
/// </summary>
public interface IDrawingTool
{
    /// <summary>
    /// Gets or sets the current drawing color.
    /// </summary>
    Brush CurrentBrush { get; set; }

    /// <summary>
    /// Gets or sets the stroke thickness.
    /// </summary>
    double StrokeThickness { get; set; }

    /// <summary>
    /// Gets or sets the font size for text tools.
    /// </summary>
    double FontSize { get; set; }

    /// <summary>
    /// Gets or sets the CommandManager for undo/redo support.
    /// </summary>
    CommandManager? CommandManager { get; set; }

    /// <summary>
    /// Called when the mouse button is pressed on the canvas.
    /// </summary>
    /// <param name="canvas">The drawing canvas.</param>
    /// <param name="position">The mouse position.</param>
    void OnMouseDown(Canvas canvas, Point position);

    /// <summary>
    /// Called when the mouse moves on the canvas.
    /// </summary>
    /// <param name="canvas">The drawing canvas.</param>
    /// <param name="position">The mouse position.</param>
    void OnMouseMove(Canvas canvas, Point position);

    /// <summary>
    /// Called when the mouse button is released on the canvas.
    /// </summary>
    /// <param name="canvas">The drawing canvas.</param>
    /// <param name="position">The mouse position.</param>
    void OnMouseUp(Canvas canvas, Point position);
}
