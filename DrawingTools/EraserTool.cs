using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Eraser tool - behaves like PencilTool but always draws with White color.
/// </summary>
public class EraserTool : IDrawingTool
{
    private Polyline? _currentLine;
    private bool _isDrawing;

    // CurrentBrush is ignored - eraser always uses White
    public Brush CurrentBrush { get; set; } = Brushes.White;
    public double StrokeThickness { get; set; } = 10; // Default larger for eraser
    public double FontSize { get; set; } = 14;
    public FontFamily? FontFamily { get; set; }
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        _isDrawing = true;
        _currentLine = new Polyline
        {
            Stroke = Brushes.White, // Always white regardless of CurrentBrush
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
