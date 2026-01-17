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
/// Line tool for drawing straight lines.
/// </summary>
public class LineTool : IDrawingTool
{
    private Line? _currentLine;
    private bool _isDrawing;

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public FontFamily? FontFamily { get; set; }
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        _isDrawing = true;

        _currentLine = new Line
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            X1 = position.X,
            Y1 = position.Y,
            X2 = position.X,
            Y2 = position.Y
        };

        canvas.Children.Add(_currentLine);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null)
            return;

        _currentLine.X2 = position.X;
        _currentLine.Y2 = position.Y;
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null)
            return;

        _isDrawing = false;
        var completedLine = _currentLine;
        _currentLine = null;

        double dx = completedLine.X2 - completedLine.X1;
        double dy = completedLine.Y2 - completedLine.Y1;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length > 5)
        {
            canvas.Children.Remove(completedLine);
            var command = new AddShapeCommand(canvas, completedLine);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedLine);
        }
    }
}
