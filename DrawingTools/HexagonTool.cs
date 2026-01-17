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
/// Hexagon tool for drawing regular hexagons.
/// </summary>
public class HexagonTool : IDrawingTool
{
    private Polygon? _currentHexagon;
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

        _currentHexagon = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Round
        };

        canvas.Children.Add(_currentHexagon);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentHexagon == null)
            return;

        double centerX = (_startPoint.X + position.X) / 2;
        double centerY = (_startPoint.Y + position.Y) / 2;
        double radius = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y)) / 2;

        _currentHexagon.Points.Clear();

        for (int i = 0; i < 6; i++)
        {
            double angle = i * Math.PI / 3;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);
            _currentHexagon.Points.Add(new Point(x, y));
        }
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentHexagon == null)
            return;

        _isDrawing = false;
        var completedHexagon = _currentHexagon;
        _currentHexagon = null;

        double size = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y));

        if (size > 10)
        {
            canvas.Children.Remove(completedHexagon);
            var command = new AddShapeCommand(canvas, completedHexagon);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedHexagon);
        }
    }
}
