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
/// Star tool for drawing 5-pointed stars.
/// </summary>
public class StarTool : IDrawingTool
{
    private Polygon? _currentStar;
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

        _currentStar = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Round
        };

        canvas.Children.Add(_currentStar);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentStar == null)
            return;

        double centerX = (_startPoint.X + position.X) / 2;
        double centerY = (_startPoint.Y + position.Y) / 2;
        double outerRadius = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y)) / 2;
        double innerRadius = outerRadius * 0.4;

        _currentStar.Points.Clear();

        for (int i = 0; i < 10; i++)
        {
            double radius = (i % 2 == 0) ? outerRadius : innerRadius;
            double angle = Math.PI / 2 + i * Math.PI / 5;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY - radius * Math.Sin(angle);
            _currentStar.Points.Add(new Point(x, y));
        }
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentStar == null)
            return;

        _isDrawing = false;
        var completedStar = _currentStar;
        _currentStar = null;

        double size = Math.Max(Math.Abs(position.X - _startPoint.X), Math.Abs(position.Y - _startPoint.Y));

        if (size > 10)
        {
            canvas.Children.Remove(completedStar);
            var command = new AddShapeCommand(canvas, completedStar);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedStar);
        }
    }
}
