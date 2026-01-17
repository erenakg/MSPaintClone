using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MSPaintClone.DrawingTools;

namespace MSPaintClone;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CommandManager _commandManager = new();
    private IDrawingTool? _currentTool;
    private Brush _currentBrush = Brushes.Black;
    private double _strokeThickness = 3;
    private double _fontSize = 14;
    private Ellipse? _cursorPreview;

    // Commands for keyboard shortcuts
    public System.Windows.Input.ICommand UndoCommand { get; }
    public System.Windows.Input.ICommand RedoCommand { get; }
    public System.Windows.Input.ICommand SaveCommand { get; }

    public MainWindow()
    {
        // Initialize commands
        UndoCommand = new RelayCommand(_ => ExecuteUndo(), _ => _commandManager.CanUndo);
        RedoCommand = new RelayCommand(_ => ExecuteRedo(), _ => _commandManager.CanRedo);
        SaveCommand = new RelayCommand(_ => ExecuteSave());
        
        DataContext = this;
        
        InitializeComponent();
        
        // Create cursor preview ellipse for eraser
        _cursorPreview = new Ellipse
        {
            Stroke = Brushes.Gray,
            StrokeThickness = 1,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false,
            Visibility = Visibility.Collapsed
        };
        
        // Set default tool to Pencil
        _currentTool = new PencilTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        UpdateToolButtonStyles();
        UpdateUndoRedoButtons();
        
        // Add cursor preview to canvas after it's loaded
        Loaded += (s, e) => 
        {
            if (_cursorPreview != null && !DrawingCanvas.Children.Contains(_cursorPreview))
            {
                DrawingCanvas.Children.Add(_cursorPreview);
            }
        };
    }

    // Tool button click handlers
    private void PencilButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new PencilTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        HideCursorPreview();
        UpdateToolButtonStyles();
    }

    private void EraserButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new EraserTool { StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        ShowCursorPreview();
        UpdateToolButtonStyles();
    }

    private void BucketButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new BucketTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        HideCursorPreview();
        UpdateToolButtonStyles();
    }

    private void RectangleButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new RectangleTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        HideCursorPreview();
        UpdateToolButtonStyles();
    }

    private void CircleButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new CircleTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        HideCursorPreview();
        UpdateToolButtonStyles();
    }

    private void TextButton_Click(object sender, RoutedEventArgs e)
    {
        _currentTool = new TextTool { CurrentBrush = _currentBrush, StrokeThickness = _strokeThickness, FontSize = _fontSize, CommandManager = _commandManager };
        HideCursorPreview();
        UpdateToolButtonStyles();
    }

    // Color picker handler
    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string colorName)
        {
            _currentBrush = colorName switch
            {
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Blue" => Brushes.Blue,
                "Green" => Brushes.Green,
                "Yellow" => Brushes.Yellow,
                "Orange" => Brushes.Orange,
                _ => Brushes.Black
            };

            if (_currentTool != null)
            {
                _currentTool.CurrentBrush = _currentBrush;
            }

            UpdateColorButtonStyles();
        }
    }

    // Thickness slider handler
    private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _strokeThickness = e.NewValue;
        
        if (_currentTool != null)
        {
            _currentTool.StrokeThickness = _strokeThickness;
        }

        // Update the label to show current thickness
        if (ThicknessLabel != null)
        {
            ThicknessLabel.Content = ((int)_strokeThickness).ToString();
        }
    }

    // Command execution methods for keyboard shortcuts
    private void ExecuteUndo()
    {
        _commandManager.Undo();
        UpdateUndoRedoButtons();
    }

    private void ExecuteRedo()
    {
        _commandManager.Redo();
        UpdateUndoRedoButtons();
    }

    private void ExecuteSave()
    {
        SaveButton_Click(this, new RoutedEventArgs());
    }

    // Undo/Redo handlers
    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        ExecuteUndo();
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        ExecuteRedo();
    }

    // Canvas mouse event handlers
    private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_currentTool == null)
            return;

        var position = e.GetPosition(DrawingCanvas);
        _currentTool.OnMouseDown(DrawingCanvas, position);
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(DrawingCanvas);
        
        // Update status bar with coordinates
        StatusText.Text = $"X: {(int)position.X}, Y: {(int)position.Y}";

        // Update cursor preview position for eraser
        UpdateCursorPreviewPosition(position);

        if (_currentTool == null || e.LeftButton != MouseButtonState.Pressed)
            return;

        _currentTool.OnMouseMove(DrawingCanvas, position);
    }

    private void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
    {
        // Hide cursor preview when mouse leaves canvas
        if (_cursorPreview != null)
        {
            _cursorPreview.Visibility = Visibility.Collapsed;
        }
    }

    private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_currentTool == null)
            return;

        var position = e.GetPosition(DrawingCanvas);
        _currentTool.OnMouseUp(DrawingCanvas, position);
        UpdateUndoRedoButtons();
    }

    private void UpdateToolButtonStyles()
    {
        // Reset all tool buttons
        PencilButton.FontWeight = FontWeights.Normal;
        EraserButton.FontWeight = FontWeights.Normal;
        BucketButton.FontWeight = FontWeights.Normal;
        RectangleButton.FontWeight = FontWeights.Normal;
        CircleButton.FontWeight = FontWeights.Normal;
        TextButton.FontWeight = FontWeights.Normal;

        // Highlight current tool
        if (_currentTool is PencilTool)
            PencilButton.FontWeight = FontWeights.Bold;
        else if (_currentTool is EraserTool)
            EraserButton.FontWeight = FontWeights.Bold;
        else if (_currentTool is BucketTool)
            BucketButton.FontWeight = FontWeights.Bold;
        else if (_currentTool is RectangleTool)
            RectangleButton.FontWeight = FontWeights.Bold;
        else if (_currentTool is CircleTool)
            CircleButton.FontWeight = FontWeights.Bold;
        else if (_currentTool is TextTool)
            TextButton.FontWeight = FontWeights.Bold;
    }

    private void UpdateColorButtonStyles()
    {
        // Reset all color button borders
        ColorBlack.BorderThickness = new Thickness(1);
        ColorRed.BorderThickness = new Thickness(1);
        ColorBlue.BorderThickness = new Thickness(1);
        ColorGreen.BorderThickness = new Thickness(1);
        ColorYellow.BorderThickness = new Thickness(1);
        ColorOrange.BorderThickness = new Thickness(1);

        // Highlight selected color with thicker border
        var selectedButton = _currentBrush == Brushes.Black ? ColorBlack :
                            _currentBrush == Brushes.Red ? ColorRed :
                            _currentBrush == Brushes.Blue ? ColorBlue :
                            _currentBrush == Brushes.Green ? ColorGreen :
                            _currentBrush == Brushes.Yellow ? ColorYellow :
                            _currentBrush == Brushes.Orange ? ColorOrange : null;

        if (selectedButton != null)
        {
            selectedButton.BorderThickness = new Thickness(3);
            selectedButton.BorderBrush = Brushes.White;
        }
    }

    private void UpdateUndoRedoButtons()
    {
        UndoButton.IsEnabled = _commandManager.CanUndo;
        RedoButton.IsEnabled = _commandManager.CanRedo;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Bitmap Image (*.bmp)|*.bmp",
            DefaultExt = ".bmp",
            FileName = "drawing"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            SaveCanvasToBmp(saveFileDialog.FileName);
        }
    }

    private void SaveCanvasToBmp(string filePath)
    {
        // Get the actual size of the canvas
        var bounds = VisualTreeHelper.GetDescendantBounds(DrawingCanvas);
        var width = (int)DrawingCanvas.ActualWidth;
        var height = (int)DrawingCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        // Create RenderTargetBitmap
        var renderBitmap = new RenderTargetBitmap(
            width,
            height,
            96d,  // DPI X
            96d,  // DPI Y
            PixelFormats.Pbgra32);

        // Render the canvas to the bitmap
        renderBitmap.Render(DrawingCanvas);

        // Encode as BMP and save
        var encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

        using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        {
            encoder.Save(fileStream);
        }

        MessageBox.Show($"Image saved to:\n{filePath}", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image Files (*.bmp;*.png;*.jpg;*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
            DefaultExt = ".bmp",
            Title = "Open Image"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LoadImageToCanvas(openFileDialog.FileName);
        }
    }

    private void LoadImageToCanvas(string filePath)
    {
        try
        {
            // Load the image
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            // Create an Image control to display it
            var image = new System.Windows.Controls.Image
            {
                Source = bitmap,
                Stretch = Stretch.None
            };

            // Clear the canvas and add the image
            DrawingCanvas.Children.Clear();
            _commandManager.Clear();
            
            // Re-add cursor preview after clearing
            if (_cursorPreview != null)
            {
                DrawingCanvas.Children.Add(_cursorPreview);
            }
            
            DrawingCanvas.Children.Add(image);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);

            UpdateUndoRedoButtons();

            MessageBox.Show($"Image loaded from:\n{filePath}", "Open Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load image:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Cursor preview methods for eraser
    private void ShowCursorPreview()
    {
        if (_cursorPreview != null)
        {
            _cursorPreview.Width = _strokeThickness;
            _cursorPreview.Height = _strokeThickness;
            _cursorPreview.Visibility = Visibility.Visible;
        }
    }

    private void HideCursorPreview()
    {
        if (_cursorPreview != null)
        {
            _cursorPreview.Visibility = Visibility.Collapsed;
        }
    }

    private void UpdateCursorPreviewPosition(Point position)
    {
        if (_cursorPreview == null || _cursorPreview.Visibility != Visibility.Visible)
            return;

        // Update size based on current thickness
        _cursorPreview.Width = _strokeThickness;
        _cursorPreview.Height = _strokeThickness;

        // Center the ellipse on the cursor
        Canvas.SetLeft(_cursorPreview, position.X - _strokeThickness / 2);
        Canvas.SetTop(_cursorPreview, position.Y - _strokeThickness / 2);

        // Bring to front
        Canvas.SetZIndex(_cursorPreview, int.MaxValue);
    }
}