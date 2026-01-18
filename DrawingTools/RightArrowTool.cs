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
/// Right Arrow tool for drawing right-pointing arrow shapes.
/// </summary>
public class RightArrowTool : IDrawingTool
{
    private Polygon? _currentArrow;
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

        _currentArrow = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Miter
        };

        canvas.Children.Add(_currentArrow);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentArrow == null)
            return;

        double left = Math.Min(_startPoint.X, position.X);
        double top = Math.Min(_startPoint.Y, position.Y);
        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width < 1) width = 1;
        if (height < 1) height = 1;

        double arrowHeadWidth = width * 0.4;
        double bodyHeight = height * 0.4;
        double centerY = top + height / 2;

        _currentArrow.Points.Clear();
        // Arrow body (left part)
        _currentArrow.Points.Add(new Point(left, centerY - bodyHeight / 2));
        _currentArrow.Points.Add(new Point(left + width - arrowHeadWidth, centerY - bodyHeight / 2));
        // Arrow head top
        _currentArrow.Points.Add(new Point(left + width - arrowHeadWidth, top));
        // Arrow tip
        _currentArrow.Points.Add(new Point(left + width, centerY));
        // Arrow head bottom
        _currentArrow.Points.Add(new Point(left + width - arrowHeadWidth, top + height));
        _currentArrow.Points.Add(new Point(left + width - arrowHeadWidth, centerY + bodyHeight / 2));
        // Arrow body (back to start)
        _currentArrow.Points.Add(new Point(left, centerY + bodyHeight / 2));
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentArrow == null)
            return;

        _isDrawing = false;
        var completedArrow = _currentArrow;
        _currentArrow = null;

        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width > 10 && height > 10)
        {
            canvas.Children.Remove(completedArrow);
            var command = new AddShapeCommand(canvas, completedArrow);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedArrow);
        }
    }
}
