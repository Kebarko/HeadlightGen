using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using KE.MSTS.HeadlightGen.Model;
using KE.MSTS.HeadlightGen.Services;
using KE.MSTS.HeadlightGen.ViewModels.Common;
using Microsoft.Win32;

namespace KE.MSTS.HeadlightGen.ViewModels;

internal class MainViewModel : ViewModelBase
{
    public string? Title
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(Title));
            Redraw();
        }
    }

    public string? CenterXStr
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(CenterXStr));
            Redraw();
        }
    }

    public float? CenterX
    {
        get => float.TryParse(CenterXStr, NumberFormatInfo.CurrentInfo, out float result) ? result : null;
        set => CenterXStr = value?.ToString();
    }

    public string? CenterYStr
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(CenterYStr));
            Redraw();
        }
    }

    public float? CenterY
    {
        get => float.TryParse(CenterYStr, NumberFormatInfo.CurrentInfo, out float result) ? result : null;
        set => CenterYStr = value?.ToString();
    }

    public string? CenterZStr
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(CenterZStr));
            Redraw();
        }
    }

    public float? CenterZ
    {
        get => float.TryParse(CenterZStr, NumberFormatInfo.CurrentInfo, out float result) ? result : null;
        set => CenterZStr = value?.ToString();
    }

    public int? Circles
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(Circles));
            Redraw();
        }
    }

    public string? MaxRadiusStr
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(MaxRadiusStr));
            Redraw();
        }
    }

    public float? MaxRadius
    {
        get => float.TryParse(MaxRadiusStr, NumberFormatInfo.CurrentInfo, out float result) ? result : null;
        set => MaxRadiusStr = value?.ToString();
    }

    public int? Increment
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(Increment));
            Redraw();
        }
    }

    public int? Rotation
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(Rotation));
            Redraw();
        }
    }

    public int? Elevation
    {
        get;
        set
        {
            if (Nullable.Equals(value, field)) return;
            field = value;
            OnPropertyChanged(nameof(Elevation));
            Redraw();
        }
    }

    public int? TotalSegments
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(TotalSegments));
            Redraw();
        }
    }

    public IReadOnlyList<RenderView> Views { get; } = Enum.GetValues<RenderView>();

    public RenderView SelectedView
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(SelectedView));
            Redraw();
        }
    } = RenderView.Front;

    public int? CanvasHeight
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(CanvasHeight));
            Redraw();
        }
    }

    public int? CanvasWidth
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged(nameof(CanvasWidth));
            Redraw();
        }
    }

    public ObservableCollection<Shape> Shapes { get; } = new ObservableCollection<Shape>();

    public ObservableCollection<UIElement> AxisTripod { get; } = new ObservableCollection<UIElement>();

    public ICommand GenerateCommand { get; }

    public MainViewModel()
    {
        GenerateCommand = new RelayCommand(Generate, CanGenerate);

        AppSettings? appSettings = AppSettings.Load();
        if (appSettings != null)
        {
            Title = appSettings.Title;
            CenterX = appSettings.CenterX;
            CenterY = appSettings.CenterY;
            CenterZ = appSettings.CenterZ;
            Circles = appSettings.Circles;
            MaxRadius = appSettings.MaxRadius;
            Increment = appSettings.Increment;
            Rotation = appSettings.Rotation;
            Elevation = appSettings.Elevation;
        }
    }

    private bool CanGenerate(object? obj)
    {
        return CenterX.HasValue && CenterY.HasValue && CenterZ.HasValue && Circles.HasValue && MaxRadius.HasValue && Increment.HasValue && Rotation.HasValue && Elevation.HasValue;
    }

    private void Generate(object? obj)
    {
        IList<Point3D> points = SegmentCalculator.Calculate(new Point3D(CenterX!.Value, CenterY!.Value, CenterZ!.Value), Circles!.Value, MaxRadius!.Value / 100, Increment!.Value, Rotation!.Value, Elevation!.Value, out _);

        var openFileDialog = new OpenFileDialog()
        {
            Title = "Select Template File",
            Filter = "Text Files (*.inc)|*.inc",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var saveFileDialog = new SaveFileDialog()
        {
            Title = "Select Output File",
            Filter = "Text Files (*.inc)|*.inc"
        };

        if (saveFileDialog.ShowDialog() != true)
            return;

        Exporter.Export(Title, points, Elevation!.Value, openFileDialog.FileName, saveFileDialog.FileName);

        MessageBox.Show("Light file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Redraw()
    {
        RedrawAxisTripod();

        Shapes.Clear();

        if (CanvasWidth == null || CanvasHeight == null || CenterX == null || CenterY == null || CenterZ == null || Circles == null || MaxRadius == null || Increment == null || Rotation == null || Elevation == null)
            return;

        IList<Point3D> points = SegmentCalculator.Calculate(new Point3D(CenterX.Value, CenterY.Value, CenterZ.Value), Circles.Value, MaxRadius.Value / 100, Increment.Value, Rotation.Value, Elevation.Value, out _);

        TotalSegments = points.Count;

        if (points.Count == 0)
            return;

        foreach (var shape in Renderer.Render(CanvasWidth.Value, CanvasHeight.Value, points, SelectedView))
        {
            Shapes.Add(shape);
        }
    }

    /// <summary>
    /// Redraws the coloured axis tripod shown in the bottom-left corner of the canvas for the currently selected view.
    /// </summary>
    private void RedrawAxisTripod()
    {
        AxisTripod.Clear();

        if (CanvasHeight == null)
            return;

        const double margin = 8;

        foreach (UIElement element in Renderer.RenderAxisTripod(SelectedView, margin, CanvasHeight.Value - margin))
        {
            AxisTripod.Add(element);
        }
    }
}