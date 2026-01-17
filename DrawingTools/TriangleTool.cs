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
/// Triangle tool for drawing triangles using Polygon.
/// </summary>
public class TriangleTool : IDrawingTool
{
    private Polygon? _currentTriangle;
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

        _currentTriangle = new Polygon
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Fill = Brushes.Transparent,
            StrokeLineJoin = PenLineJoin.Round
        };

        // Initialize with three identical points
        _currentTriangle.Points.Add(position);
        _currentTriangle.Points.Add(position);
        _currentTriangle.Points.Add(position);

        canvas.Children.Add(_currentTriangle);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentTriangle == null)
            return;

        // Calculate triangle points based on drag
        // Top-center: midpoint of start and end X, at start Y (or end Y if dragging up)
        // Bottom-left: start point
        // Bottom-right: end X, start Y

        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        double left = Math.Min(_startPoint.X, position.X);
        double top = Math.Min(_startPoint.Y, position.Y);
        double right = Math.Max(_startPoint.X, position.X);
        double bottom = Math.Max(_startPoint.Y, position.Y);

        // Calculate the three points of the triangle
        // Top-center
        Point topCenter = new Point(left + width / 2, top);
        // Bottom-left
        Point bottomLeft = new Point(left, bottom);
        // Bottom-right
        Point bottomRight = new Point(right, bottom);

        // Update polygon points
        _currentTriangle.Points[0] = topCenter;
        _currentTriangle.Points[1] = bottomLeft;
        _currentTriangle.Points[2] = bottomRight;
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentTriangle == null)
            return;

        _isDrawing = false;
        var completedTriangle = _currentTriangle;
        _currentTriangle = null;

        // Check if triangle has meaningful size
        double width = Math.Abs(position.X - _startPoint.X);
        double height = Math.Abs(position.Y - _startPoint.Y);

        if (width > 5 && height > 5)
        {
            // Remove from canvas (CommandManager will re-add)
            canvas.Children.Remove(completedTriangle);

            var command = new AddShapeCommand(canvas, completedTriangle);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            // Remove small/accidental triangles
            canvas.Children.Remove(completedTriangle);
        }
    }
}
