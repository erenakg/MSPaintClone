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
/// Arrow tool for drawing lines with arrowheads.
/// </summary>
public class ArrowTool : IDrawingTool
{
    private Line? _currentLine;
    private Line? _arrowWing1;
    private Line? _arrowWing2;
    private Point _startPoint;
    private bool _isDrawing;
    private const double ArrowHeadLength = 15;
    private const double ArrowHeadAngle = Math.PI / 6; // 30 degrees

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public FontFamily? FontFamily { get; set; }
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        _isDrawing = true;
        _startPoint = position;

        // Create the main line
        _currentLine = new Line
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            X1 = position.X,
            Y1 = position.Y,
            X2 = position.X,
            Y2 = position.Y
        };

        // Create arrow wings
        _arrowWing1 = new Line
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness
        };

        _arrowWing2 = new Line
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness
        };

        canvas.Children.Add(_currentLine);
        canvas.Children.Add(_arrowWing1);
        canvas.Children.Add(_arrowWing2);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null || _arrowWing1 == null || _arrowWing2 == null)
            return;

        // Update main line end point
        _currentLine.X2 = position.X;
        _currentLine.Y2 = position.Y;

        // Calculate arrowhead
        UpdateArrowHead(position);
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (!_isDrawing || _currentLine == null || _arrowWing1 == null || _arrowWing2 == null)
            return;

        _isDrawing = false;

        // Store references before clearing
        var completedLine = _currentLine;
        var completedWing1 = _arrowWing1;
        var completedWing2 = _arrowWing2;

        _currentLine = null;
        _arrowWing1 = null;
        _arrowWing2 = null;

        // Check if arrow has meaningful size
        double dx = completedLine.X2 - completedLine.X1;
        double dy = completedLine.Y2 - completedLine.Y1;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length > 5)
        {
            // Remove from canvas (CommandManager will re-add)
            canvas.Children.Remove(completedLine);
            canvas.Children.Remove(completedWing1);
            canvas.Children.Remove(completedWing2);

            // Group all parts into a single canvas for undo/redo
            var arrowGroup = new Canvas();
            arrowGroup.Children.Add(completedLine);
            arrowGroup.Children.Add(completedWing1);
            arrowGroup.Children.Add(completedWing2);

            var command = new AddShapeCommand(canvas, arrowGroup);
            CommandManager?.ExecuteCommand(command);
        }
        else
        {
            // Remove small/accidental arrows
            canvas.Children.Remove(completedLine);
            canvas.Children.Remove(completedWing1);
            canvas.Children.Remove(completedWing2);
        }
    }

    private void UpdateArrowHead(Point endPoint)
    {
        if (_arrowWing1 == null || _arrowWing2 == null)
            return;

        // Calculate angle of the line
        double dx = endPoint.X - _startPoint.X;
        double dy = endPoint.Y - _startPoint.Y;
        double angle = Math.Atan2(dy, dx);

        // Calculate arrowhead wing points
        double wing1Angle = angle + Math.PI - ArrowHeadAngle;
        double wing2Angle = angle + Math.PI + ArrowHeadAngle;

        // Wing 1
        _arrowWing1.X1 = endPoint.X;
        _arrowWing1.Y1 = endPoint.Y;
        _arrowWing1.X2 = endPoint.X + ArrowHeadLength * Math.Cos(wing1Angle);
        _arrowWing1.Y2 = endPoint.Y + ArrowHeadLength * Math.Sin(wing1Angle);

        // Wing 2
        _arrowWing2.X1 = endPoint.X;
        _arrowWing2.Y1 = endPoint.Y;
        _arrowWing2.X2 = endPoint.X + ArrowHeadLength * Math.Cos(wing2Angle);
        _arrowWing2.Y2 = endPoint.Y + ArrowHeadLength * Math.Sin(wing2Angle);
    }
}
