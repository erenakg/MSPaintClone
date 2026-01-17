using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using TextBox = System.Windows.Controls.TextBox;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Text tool for adding text to the canvas.
/// </summary>
public class TextTool : IDrawingTool
{
    private TextBox? _activeTextBox;
    private double _currentFontSize = 14;
    private bool _isFinalizingTextBox = false;

    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        // Finalize any existing text box first
        FinalizeTextBox(canvas);

        // Use StrokeThickness as font size for text tool
        _currentFontSize = Math.Max(10, StrokeThickness * 2);

        // Create a new TextBox at the clicked location
        _activeTextBox = new TextBox
        {
            Background = Brushes.White,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Foreground = CurrentBrush,
            FontSize = _currentFontSize,
            MinWidth = 100,
            MinHeight = Math.Max(25, (int)(_currentFontSize * 1.5)),
            Padding = new Thickness(2),
            AcceptsReturn = false
        };

        Canvas.SetLeft(_activeTextBox, position.X);
        Canvas.SetTop(_activeTextBox, position.Y);
        canvas.Children.Add(_activeTextBox);

        // Focus the TextBox for immediate typing
        _activeTextBox.Focus();

        // Store reference for event handlers
        var textBox = _activeTextBox;

        // Handle when user clicks away or presses Enter
        textBox.LostFocus += (s, e) => 
        {
            if (textBox == _activeTextBox)
                FinalizeTextBox(canvas);
        };
        textBox.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter)
            {
                FinalizeTextBox(canvas);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                CancelTextBox(canvas);
                e.Handled = true;
            }
        };
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        // Text tool doesn't use mouse move
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        // Text tool handles completion via TextBox events
    }

    private void FinalizeTextBox(Canvas canvas)
    {
        // Prevent re-entry
        if (_isFinalizingTextBox || _activeTextBox == null)
            return;

        _isFinalizingTextBox = true;

        try
        {
            if (string.IsNullOrWhiteSpace(_activeTextBox.Text))
            {
                CancelTextBox(canvas);
                return;
            }

            // Get the text and position
            var text = _activeTextBox.Text;
            var left = Canvas.GetLeft(_activeTextBox);
            var top = Canvas.GetTop(_activeTextBox);
            var fontSize = _currentFontSize;

            // Remove the TextBox
            canvas.Children.Remove(_activeTextBox);
            _activeTextBox = null;

            // Create a TextBlock to display the finalized text
            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = CurrentBrush,
                FontSize = fontSize,
                Background = Brushes.Transparent
            };

            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, top);

            // Push command to undo stack
            var command = new AddShapeCommand(canvas, textBlock);
            CommandManager?.ExecuteCommand(command);
        }
        finally
        {
            _isFinalizingTextBox = false;
        }
    }

    private void CancelTextBox(Canvas canvas)
    {
        if (_activeTextBox != null)
        {
            canvas.Children.Remove(_activeTextBox);
            _activeTextBox = null;
        }
        _isFinalizingTextBox = false;
    }
}
