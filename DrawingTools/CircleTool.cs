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
/// Circle tool for drawing ellipses/circles.
/// </summary>
public class CircleTool : IDrawingTool
{
    private Ellipse? _currentEllipse;
    private Point _startPoint;
    private bool _isDrawing;

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public FontFamily? FontFamily { get; set; }
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        _isDrawing = true;
        _startPoint = position;

        _currentEllipse = new Ellipse
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(_currentEllipse, position.X);
        Canvas.SetTop(_currentEllipse, position.Y);
        canvas.Children.Add(_currentEllipse);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentEllipse == null)
            return;

        var x = Math.Min(position.X, _startPoint.X);
        var y = Math.Min(position.Y, _startPoint.Y);
        var width = Math.Abs(position.X - _startPoint.X);
        var height = Math.Abs(position.Y - _startPoint.Y);

        Canvas.SetLeft(_currentEllipse, x);
        Canvas.SetTop(_currentEllipse, y);
        _currentEllipse.Width = width;
        _currentEllipse.Height = height;
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentEllipse == null)
            return;

        _isDrawing = false;
        var completedEllipse = _currentEllipse;
        _currentEllipse = null;

        // Only create command if ellipse has size
        if (completedEllipse.Width > 0 && completedEllipse.Height > 0)
        {
            // Remove from canvas first (CommandManager will re-add it)
            canvas.Children.Remove(completedEllipse);

            // Push command to undo stack
            var command = new AddShapeCommand(canvas, completedEllipse);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedEllipse);
        }
    }
}
