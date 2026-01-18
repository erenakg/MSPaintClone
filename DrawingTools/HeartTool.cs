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
/// Heart tool for drawing heart shapes.
/// </summary>
public class HeartTool : IDrawingTool
{
    private Path? _currentHeart;
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

        _currentHeart = new Path
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent
        };

        canvas.Children.Add(_currentHeart);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentHeart == null)
            return;

        double left = Math.Min(_startPoint.X, position.X);
        double top = Math.Min(_startPoint.Y, position.Y);
        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width < 1) width = 1;
        if (height < 1) height = 1;

        _currentHeart.Data = CreateHeartGeometry(left, top, width, height);
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentHeart == null)
            return;

        _isDrawing = false;
        var completedHeart = _currentHeart;
        _currentHeart = null;

        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width > 10 && height > 10)
        {
            canvas.Children.Remove(completedHeart);
            var command = new AddShapeCommand(canvas, completedHeart);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedHeart);
        }
    }

    private Geometry CreateHeartGeometry(double x, double y, double width, double height)
    {
        double w = width;
        double h = height;

        var pathGeometry = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(x + w / 2, y + h),
            IsClosed = true
        };

        // Left side bezier
        figure.Segments.Add(new BezierSegment(
            new Point(x, y + h * 0.7),
            new Point(x, y + h * 0.3),
            new Point(x + w / 2, y + h * 0.3),
            true));

        // Right side bezier
        figure.Segments.Add(new BezierSegment(
            new Point(x + w, y + h * 0.3),
            new Point(x + w, y + h * 0.7),
            new Point(x + w / 2, y + h),
            true));

        pathGeometry.Figures.Add(figure);
        return pathGeometry;
    }
}
