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
/// Curve tool for drawing Quadratic Bezier curves with a 2-step state machine.
/// Step 1: Drag to draw a straight line (Start to End)
/// Step 2: Move mouse to adjust control point, click to finalize
/// </summary>
public class CurveTool : IDrawingTool
{
    private enum CurveState
    {
        Idle,
        DrawingLine,
        AdjustingControlPoint
    }

    private CurveState _state = CurveState.Idle;
    private Point _startPoint;
    private Point _endPoint;
    private Point _controlPoint;
    private Path? _currentPath;
    private PathGeometry? _pathGeometry;
    private PathFigure? _pathFigure;
    private QuadraticBezierSegment? _bezierSegment;

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public FontFamily? FontFamily { get; set; }
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        switch (_state)
        {
            case CurveState.Idle:
                // Start drawing the line
                _state = CurveState.DrawingLine;
                _startPoint = position;
                _endPoint = position;
                _controlPoint = position;

                // Create the path with bezier geometry
                CreateBezierPath(canvas);
                break;

            case CurveState.AdjustingControlPoint:
                // Finalize the curve
                FinalizeCurve(canvas);
                break;
        }
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        switch (_state)
        {
            case CurveState.DrawingLine:
                // Update end point while dragging
                _endPoint = position;
                // Control point is at midpoint during line drawing
                _controlPoint = new Point(
                    (_startPoint.X + _endPoint.X) / 2,
                    (_startPoint.Y + _endPoint.Y) / 2
                );
                UpdateBezierPath();
                break;

            case CurveState.AdjustingControlPoint:
                // Update control point based on mouse position
                _controlPoint = position;
                UpdateBezierPath();
                break;
        }
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        if (_state == CurveState.DrawingLine)
        {
            // Check if line has meaningful length
            double dx = _endPoint.X - _startPoint.X;
            double dy = _endPoint.Y - _startPoint.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);

            if (length > 5)
            {
                // Transition to control point adjustment state
                _state = CurveState.AdjustingControlPoint;
            }
            else
            {
                // Cancel if too small
                CancelCurve(canvas);
            }
        }
    }

    private void CreateBezierPath(Canvas canvas)
    {
        _bezierSegment = new QuadraticBezierSegment
        {
            Point1 = _controlPoint,
            Point2 = _endPoint
        };

        _pathFigure = new PathFigure
        {
            StartPoint = _startPoint,
            IsClosed = false
        };
        _pathFigure.Segments.Add(_bezierSegment);

        _pathGeometry = new PathGeometry();
        _pathGeometry.Figures.Add(_pathFigure);

        _currentPath = new Path
        {
            Stroke = CurrentBrush,
            StrokeThickness = StrokeThickness,
            Data = _pathGeometry,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };

        canvas.Children.Add(_currentPath);
    }

    private void UpdateBezierPath()
    {
        if (_pathFigure == null || _bezierSegment == null)
            return;

        _pathFigure.StartPoint = _startPoint;
        _bezierSegment.Point1 = _controlPoint;
        _bezierSegment.Point2 = _endPoint;
    }

    private void FinalizeCurve(Canvas canvas)
    {
        if (_currentPath == null)
        {
            _state = CurveState.Idle;
            return;
        }

        var completedPath = _currentPath;
        _currentPath = null;
        _pathGeometry = null;
        _pathFigure = null;
        _bezierSegment = null;
        _state = CurveState.Idle;

        // Remove from canvas (CommandManager will re-add)
        canvas.Children.Remove(completedPath);

        var command = new AddShapeCommand(canvas, completedPath);
        CommandManager?.ExecuteCommand(command);
    }

    private void CancelCurve(Canvas canvas)
    {
        if (_currentPath != null)
        {
            canvas.Children.Remove(_currentPath);
            _currentPath = null;
        }

        _pathGeometry = null;
        _pathFigure = null;
        _bezierSegment = null;
        _state = CurveState.Idle;
    }
}
