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
/// Pentagon tool for drawing regular pentagons.
/// </summary>
public class PentagonTool : IDrawingTool
{
    private Polygon? _currentPentagon;
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

        _currentPentagon = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Round
        };

        canvas.Children.Add(_currentPentagon);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentPentagon == null)
            return;

        double centerX = (_startPoint.X + position.X) / 2;
        double centerY = (_startPoint.Y + position.Y) / 2;
        double radius = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y)) / 2;

        _currentPentagon.Points.Clear();

        for (int i = 0; i < 5; i++)
        {
            double angle = Math.PI / 2 + i * 2 * Math.PI / 5;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY - radius * Math.Sin(angle);
            _currentPentagon.Points.Add(new Point(x, y));
        }
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentPentagon == null)
            return;

        _isDrawing = false;
        var completedPentagon = _currentPentagon;
        _currentPentagon = null;

        double size = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y));

        if (size > 10)
        {
            canvas.Children.Remove(completedPentagon);
            var command = new AddShapeCommand(canvas, completedPentagon);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedPentagon);
        }
    }
}
