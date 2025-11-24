using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using KE.MSTS.HeadlightGen.Model;
using KE.MSTS.HeadlightGen.Services;
using KE.MSTS.HeadlightGen.ViewModels.Common;
using Microsoft.Win32;

namespace KE.MSTS.HeadlightGen.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private string? title;
        private float? centerX;
        private float? centerZ;
        private float? centerY;
        private int? circles;
        private float? maxRadius;
        private int? increment;
        private int? baseAngle;
        private int? totalSegments;
        private int? canvasWidth;
        private int? canvasHeight;

        public string? Title
        {
            get => title;
            set
            {
                if (value == title) return;
                title = value;
                OnPropertyChanged(nameof(Title));
                Redraw();
            }
        }

        public float? CenterX
        {
            get => centerX;
            set
            {
                if (Nullable.Equals(value, centerX)) return;
                centerX = value;
                OnPropertyChanged(nameof(CenterX));
                Redraw();
            }
        }

        public float? CenterY
        {
            get => centerY;
            set
            {
                if (Nullable.Equals(value, centerY)) return;
                centerY = value;
                OnPropertyChanged(nameof(CenterY));
                Redraw();
            }
        }

        public float? CenterZ
        {
            get => centerZ;
            set
            {
                if (Nullable.Equals(value, centerZ)) return;
                centerZ = value;
                OnPropertyChanged(nameof(CenterZ));
                Redraw();
            }
        }

        public int? Circles
        {
            get => circles;
            set
            {
                if (value == circles) return;
                circles = value;
                OnPropertyChanged(nameof(Circles));
                Redraw();
            }
        }

        public float? MaxRadius
        {
            get => maxRadius;
            set
            {
                if (Nullable.Equals(value, maxRadius)) return;
                maxRadius = value;
                OnPropertyChanged(nameof(MaxRadius));
                Redraw();
            }
        }

        public int? Increment
        {
            get => increment;
            set
            {
                if (value == increment) return;
                increment = value;
                OnPropertyChanged(nameof(Increment));
                Redraw();
            }
        }

        public int? BaseAngle
        {
            get => baseAngle;
            set
            {
                if (Nullable.Equals(value, baseAngle)) return;
                baseAngle = value;
                OnPropertyChanged(nameof(BaseAngle));
                Redraw();
            }
        }

        public int? TotalSegments
        {
            get => totalSegments;
            set
            {
                if (value == totalSegments) return;
                totalSegments = value;
                OnPropertyChanged(nameof(TotalSegments));
                Redraw();
            }
        }

        public int? CanvasHeight
        {
            get => canvasHeight;
            set
            {
                if (value == canvasHeight) return;
                canvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
                Redraw();
            }
        }

        public int? CanvasWidth
        {
            get => canvasWidth;
            set
            {
                if (value == canvasWidth) return;
                canvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
                Redraw();
            }
        }

        public ObservableCollection<Shape> Shapes { get; } = new ObservableCollection<Shape>();

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
                BaseAngle = appSettings.BaseAngle;
            }
        }

        private bool CanGenerate(object? obj)
        {
            return CenterX.HasValue && CenterY.HasValue && CenterZ.HasValue && Circles.HasValue && MaxRadius.HasValue && Increment.HasValue && BaseAngle.HasValue;
        }

        private void Generate(object? obj)
        {
            IList<PointF> points = SegmentCalculator.Calculate(new PointF(CenterX!.Value, CenterY!.Value), Circles!.Value, MaxRadius!.Value / 100, Increment!.Value, BaseAngle!.Value, out _);
            
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

            Exporter.Export(Title, points, CenterZ!.Value, openFileDialog.FileName, saveFileDialog.FileName);

            MessageBox.Show("Light file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Redraw()
        {
            Shapes.Clear();

            if (CanvasWidth == null || CanvasHeight == null || CenterX == null || CenterY == null || Circles == null || MaxRadius == null || Increment == null || BaseAngle == null)
                return;

            IList<PointF> points = SegmentCalculator.Calculate(new PointF(CenterX.Value, CenterY.Value), Circles.Value, MaxRadius.Value / 100, Increment.Value, BaseAngle.Value, out RectangleF boudingBox);
            
            TotalSegments = points.Count;
            
            if (points.Count == 0)
                return;

            foreach (var shape in Renderer.Render(CanvasWidth.Value, CanvasHeight.Value, boudingBox, points))
            {
                Shapes.Add(shape);
            }
        }
    }
}
