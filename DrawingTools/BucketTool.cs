using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace MSPaintClone.DrawingTools;

/// <summary>
/// Bucket tool for flood filling areas with a color.
/// </summary>
public class BucketTool : IDrawingTool
{
    public Brush CurrentBrush { get; set; } = Brushes.Black;
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 14;
    public CommandManager? CommandManager { get; set; }

    public void OnMouseDown(Canvas canvas, Point position)
    {
        // Get the fill color from CurrentBrush
        var fillColor = GetColorFromBrush(CurrentBrush);
        
        // Render canvas to a WriteableBitmap
        int width = (int)canvas.ActualWidth;
        int height = (int)canvas.ActualHeight;
        
        if (width <= 0 || height <= 0)
            return;

        // Create RenderTargetBitmap from canvas
        var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        renderBitmap.Render(canvas);

        // Convert to WriteableBitmap for pixel manipulation
        var writeableBitmap = new WriteableBitmap(renderBitmap);
        
        int x = (int)position.X;
        int y = (int)position.Y;
        
        // Bounds check
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        // Get target color at click position
        var targetColor = GetPixelColor(writeableBitmap, x, y);
        
        // Don't fill if clicking on same color
        if (ColorsMatch(targetColor, fillColor))
            return;

        // Perform flood fill
        FloodFill(writeableBitmap, x, y, targetColor, fillColor);

        // Create an Image from the filled bitmap
        var filledImage = new System.Windows.Controls.Image
        {
            Source = writeableBitmap,
            Stretch = Stretch.None
        };

        // Store old children for undo
        var oldChildren = canvas.Children.Cast<UIElement>().ToList();

        // Clear canvas and add filled image
        canvas.Children.Clear();
        canvas.Children.Add(filledImage);
        Canvas.SetLeft(filledImage, 0);
        Canvas.SetTop(filledImage, 0);

        // Create command for undo/redo
        var command = new FillCommand(canvas, oldChildren, filledImage);
        CommandManager?.ExecuteCommand(command);
    }

    public void OnMouseMove(Canvas canvas, Point position)
    {
        // Bucket tool doesn't use mouse move
    }

    public void OnMouseUp(Canvas canvas, Point position)
    {
        // Bucket tool completes on mouse down
    }

    private Color GetColorFromBrush(Brush brush)
    {
        if (brush is SolidColorBrush solidBrush)
            return solidBrush.Color;
        return Colors.Black;
    }

    private Color GetPixelColor(WriteableBitmap bitmap, int x, int y)
    {
        if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
            return Colors.Transparent;

        bitmap.Lock();
        try
        {
            unsafe
            {
                IntPtr buffer = bitmap.BackBuffer;
                int stride = bitmap.BackBufferStride;
                byte* pixels = (byte*)buffer.ToPointer();
                
                int index = y * stride + x * 4;
                byte b = pixels[index];
                byte g = pixels[index + 1];
                byte r = pixels[index + 2];
                byte a = pixels[index + 3];
                
                return Color.FromArgb(a, r, g, b);
            }
        }
        finally
        {
            bitmap.Unlock();
        }
    }

    private void SetPixelColor(WriteableBitmap bitmap, int x, int y, Color color)
    {
        unsafe
        {
            IntPtr buffer = bitmap.BackBuffer;
            int stride = bitmap.BackBufferStride;
            byte* pixels = (byte*)buffer.ToPointer();
            
            int index = y * stride + x * 4;
            pixels[index] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }
    }

    private bool ColorsMatch(Color c1, Color c2)
    {
        // Allow some tolerance for anti-aliasing
        int tolerance = 10;
        return Math.Abs(c1.R - c2.R) <= tolerance &&
               Math.Abs(c1.G - c2.G) <= tolerance &&
               Math.Abs(c1.B - c2.B) <= tolerance &&
               Math.Abs(c1.A - c2.A) <= tolerance;
    }

    private void FloodFill(WriteableBitmap bitmap, int startX, int startY, Color targetColor, Color fillColor)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        
        // Use stack-based approach to avoid stack overflow
        var stack = new Stack<(int x, int y)>();
        stack.Push((startX, startY));
        
        // Track visited pixels
        var visited = new bool[width, height];
        
        bitmap.Lock();
        try
        {
            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                
                // Bounds check
                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;
                
                // Already visited check
                if (visited[x, y])
                    continue;
                
                visited[x, y] = true;
                
                // Get current pixel color
                var currentColor = GetPixelColorUnsafe(bitmap, x, y);
                
                // Check if this pixel should be filled
                if (!ColorsMatch(currentColor, targetColor))
                    continue;
                
                // Fill this pixel
                SetPixelColor(bitmap, x, y, fillColor);
                
                // Add neighbors to stack
                stack.Push((x + 1, y));
                stack.Push((x - 1, y));
                stack.Push((x, y + 1));
                stack.Push((x, y - 1));
            }
            
            // Mark the entire bitmap as dirty
            bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
        }
        finally
        {
            bitmap.Unlock();
        }
    }

    private Color GetPixelColorUnsafe(WriteableBitmap bitmap, int x, int y)
    {
        unsafe
        {
            IntPtr buffer = bitmap.BackBuffer;
            int stride = bitmap.BackBufferStride;
            byte* pixels = (byte*)buffer.ToPointer();
            
            int index = y * stride + x * 4;
            byte b = pixels[index];
            byte g = pixels[index + 1];
            byte r = pixels[index + 2];
            byte a = pixels[index + 3];
            
            return Color.FromArgb(a, r, g, b);
        }
    }
}

/// <summary>
/// Command for fill operation supporting undo/redo.
/// </summary>
public class FillCommand : ICommand
{
    private readonly Canvas _canvas;
    private readonly List<UIElement> _oldChildren;
    private readonly System.Windows.Controls.Image _filledImage;

    public FillCommand(Canvas canvas, List<UIElement> oldChildren, System.Windows.Controls.Image filledImage)
    {
        _canvas = canvas;
        _oldChildren = oldChildren;
        _filledImage = filledImage;
    }

    public void Execute()
    {
        _canvas.Children.Clear();
        _canvas.Children.Add(_filledImage);
        Canvas.SetLeft(_filledImage, 0);
        Canvas.SetTop(_filledImage, 0);
    }

    public void UnExecute()
    {
        _canvas.Children.Clear();
        foreach (var child in _oldChildren)
        {
            _canvas.Children.Add(child);
        }
    }
}
