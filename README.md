# 🎨 MS Paint Clone

A feature-rich drawing application built with **WPF (.NET 8)** that replicates the classic Microsoft Paint experience with modern enhancements.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D6?style=flat-square&logo=windows)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## ✨ Features

### 🖌️ Drawing Tools
| Tool | Description |
|------|-------------|
| **Pencil** | Freehand drawing with customizable thickness |
| **Eraser** | Erase parts of your drawing (white color) |
| **Bucket Fill** | Flood fill an area with the selected color |
| **Text** | Add text with customizable font family and size |

### 📐 Shape Tools
Access all shapes from a single dropdown menu:

| Shape | Description |
|-------|-------------|
| **Line** | Draw straight lines |
| **Rectangle** | Draw rectangles/squares |
| **Circle** | Draw circles/ellipses |
| **Triangle** | Draw equilateral triangles |
| **Diamond** | Draw diamond/rhombus shapes |
| **Pentagon** | Draw regular pentagons |
| **Hexagon** | Draw regular hexagons |
| **Star** | Draw 5-pointed stars |
| **Arrow** | Draw line arrows with arrowheads |
| **Right Arrow** | Draw block-style right arrows |
| **Heart** | Draw heart shapes |

### 🎨 Color Options
- **Quick Colors**: Black, Red, Blue, Green, Yellow, Orange
- **Custom Colors**: "More Colors..." button opens the Windows Color Dialog for unlimited color choices

### 🔧 Additional Features
- **Adjustable Brush Size**: Slider from 1 to 50 pixels
- **Font Selection**: Choose from 12 popular fonts for the Text tool
- **Undo/Redo**: Full undo/redo support with keyboard shortcuts
- **Save/Open**: Save your artwork as BMP files

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl + Z` | Undo |
| `Ctrl + Y` | Redo |
| `Ctrl + S` | Save |

---

## 🏗️ Architecture

This project follows clean software design patterns:

### Design Patterns Used

#### 1. **Strategy Pattern** (`IDrawingTool`)
Each drawing tool implements the `IDrawingTool` interface, allowing tools to be swapped at runtime without changing the main application logic.

```
IDrawingTool
├── PencilTool
├── EraserTool
├── BucketTool
├── TextTool
├── LineTool
├── RectangleTool
├── CircleTool
├── TriangleTool
├── DiamondTool
├── PentagonTool
├── HexagonTool
├── StarTool
├── ArrowTool
├── RightArrowTool
└── HeartTool
```

#### 2. **Command Pattern** (`ICommand`)
All drawing operations are wrapped in commands, enabling undo/redo functionality.

```
ICommand
├── AddShapeCommand (for shapes)
└── FillCommand (for bucket fill)
```

### Project Structure

```
MSPaintClone/
├── App.xaml                    # Application definition
├── App.xaml.cs                 # Application startup
├── MainWindow.xaml             # Main UI layout
├── MainWindow.xaml.cs          # Main window logic
├── CommandManager.cs           # Undo/Redo stack management
├── ICommand.cs                 # Command pattern interface
├── RelayCommand.cs             # ICommand implementation for MVVM
├── DrawingTools/
│   ├── IDrawingTool.cs         # Strategy pattern interface
│   ├── AddShapeCommand.cs      # Command for adding shapes
│   ├── PencilTool.cs           # Freehand drawing
│   ├── EraserTool.cs           # Erasing
│   ├── BucketTool.cs           # Flood fill
│   ├── TextTool.cs             # Text insertion
│   ├── LineTool.cs             # Line drawing
│   ├── RectangleTool.cs        # Rectangle drawing
│   ├── CircleTool.cs           # Circle/Ellipse drawing
│   ├── TriangleTool.cs         # Triangle drawing
│   ├── DiamondTool.cs          # Diamond drawing
│   ├── PentagonTool.cs         # Pentagon drawing
│   ├── HexagonTool.cs          # Hexagon drawing
│   ├── StarTool.cs             # Star drawing
│   ├── ArrowTool.cs            # Line arrow drawing
│   ├── RightArrowTool.cs       # Block arrow drawing
│   └── HeartTool.cs            # Heart shape drawing
└── MSPaintClone.csproj         # Project file
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 (WPF is Windows-only)

### Build & Run

```bash
# Clone the repository
git clone https://github.com/yourusername/MSPaintClone.git
cd MSPaintClone

# Build the project
dotnet build

# Run the application
dotnet run
```

### Or using Visual Studio
1. Open `MSPaintClone.sln`
2. Press `F5` to build and run

---

## 📸 Screenshots

*Coming soon...*

---

## 🛠️ Technical Details

- **Framework**: .NET 8.0 Windows
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# 12
- **Additional References**: 
  - Windows Forms (for ColorDialog)

### Key Technical Features
- **Unsafe code** for fast pixel manipulation in Bucket Fill tool
- **PathGeometry** for complex shapes (Heart)
- **Polygon** for regular shapes (Star, Pentagon, Hexagon)
- **WriteableBitmap** for flood fill algorithm

---

## 🤝 Contributing

Contributions are welcome! Feel free to:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Inspired by the classic Microsoft Paint
- Icons from Segoe MDL2 Assets font
- Built with ❤️ using WPF

---

## 📋 TODO / Future Enhancements

- [ ] Selection tool (move, resize, copy, paste)
- [ ] Layers support
- [ ] More file formats (PNG, JPG, GIF)
- [ ] Spray paint tool
- [ ] Zoom in/out
- [ ] Recent colors palette
- [ ] Eyedropper tool
- [ ] Crop functionality
- [ ] Resize canvas
