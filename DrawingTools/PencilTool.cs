using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Pencil tool for freehand drawing using Polyline.
/// </summary>
public class PencilTool : IDrawingTool
{
    private Polyline? _currentLine;
    private bool _isDrawing;

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        _isDrawing = true;
        _currentLine = new Polyline
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };
        _currentLine.Points.Add(position);
        canvas.Children.Add(_currentLine);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null)
            return;

        _currentLine.Points.Add(position);
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null)
            return;

        _isDrawing = false;
        var completedLine = _currentLine;
        _currentLine = null;

        // Remove from canvas first (CommandManager will re-add it)
        canvas.Children.Remove(completedLine);

        // Push command to undo stack
        var command = new AddShapeCommand(canvas, completedLine);
        CommandManager?.ExecuteCommand(command);
    }
}
