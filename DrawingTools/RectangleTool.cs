using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Rectangle tool for drawing rectangles.
/// </summary>
public class RectangleTool : IDrawingTool
{
    private Rectangle? _currentRectangle;
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

        _currentRectangle = new Rectangle
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(_currentRectangle, position.X);
        Canvas.SetTop(_currentRectangle, position.Y);
        canvas.Children.Add(_currentRectangle);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentRectangle == null)
            return;

        var x = Math.Min(position.X, _startPoint.X);
        var y = Math.Min(position.Y, _startPoint.Y);
        var width = Math.Abs(position.X - _startPoint.X);
        var height = Math.Abs(position.Y - _startPoint.Y);

        Canvas.SetLeft(_currentRectangle, x);
        Canvas.SetTop(_currentRectangle, y);
        _currentRectangle.Width = width;
        _currentRectangle.Height = height;
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentRectangle == null)
            return;

        _isDrawing = false;
        var completedRectangle = _currentRectangle;
        _currentRectangle = null;

        // Only create command if rectangle has size
        if (completedRectangle.Width > 0 && completedRectangle.Height > 0)
        {
            // Remove from canvas first (CommandManager will re-add it)
            canvas.Children.Remove(completedRectangle);

            // Push command to undo stack
            var command = new AddShapeCommand(canvas, completedRectangle);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedRectangle);
        }
    }
}
