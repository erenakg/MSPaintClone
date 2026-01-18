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
/// Diamond tool for drawing diamond/rhombus shapes.
/// </summary>
public class DiamondTool : IDrawingTool
{
    private Polygon? _currentDiamond;
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

        _currentDiamond = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Miter
        };

        canvas.Children.Add(_currentDiamond);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentDiamond == null)
            return;

        double left = Math.Min(_startPoint.X, position.X);
        double top = Math.Min(_startPoint.Y, position.Y);
        double right = Math.Max(_startPoint.X, position.X);
        double bottom = Math.Max(_startPoint.Y, position.Y);
        double centerX = (left + right) / 2;
        double centerY = (top + bottom) / 2;

        _currentDiamond.Points.Clear();
        _currentDiamond.Points.Add(new Point(centerX, top));      // Top
        _currentDiamond.Points.Add(new Point(right, centerY));    // Right
        _currentDiamond.Points.Add(new Point(centerX, bottom));   // Bottom
        _currentDiamond.Points.Add(new Point(left, centerY));     // Left
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentDiamond == null)
            return;

        _isDrawing = false;
        var completedDiamond = _currentDiamond;
        _currentDiamond = null;

        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width > 5 && height > 5)
        {
            canvas.Children.Remove(completedDiamond);
            var command = new AddShapeCommand(canvas, completedDiamond);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            canvas.Children.Remove(completedDiamond);
        }
    }
}
