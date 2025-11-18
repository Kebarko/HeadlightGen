using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        InputCircles.PreviewKeyDown += InputOnPreviewKeyDown;
        InputIncrement.PreviewKeyDown += InputOnPreviewKeyDown;

        Loaded += (_, _) => Redraw();
        InputCenterX.TextChanged += (_, _) => Redraw();
        InputCenterY.TextChanged += (_, _) => Redraw();
        InputCircles.TextChanged += (_, _) => Redraw();
        InputMaxRadius.TextChanged += (_, _) => Redraw();
        InputIncrement.TextChanged += (_, _) => Redraw();
        TbCanvas.SizeChanged += (_, _) => Redraw();

        GenerateButton.Click += (_, _) => Generate();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        InputCenterX.Text = "0";
        InputCenterY.Text = "0";
        InputCircles.Text = "5";
        InputMaxRadius.Text = "15";
        InputIncrement.Text = "6";
    }

    private void Redraw()
    {
        TbCanvas.Children.Clear();

        if (!float.TryParse(InputCenterX.Text, NumberFormatInfo.InvariantInfo, out float centerX)) return;
        if (!float.TryParse(InputCenterY.Text, NumberFormatInfo.InvariantInfo, out float centerY)) return;
        if (!int.TryParse(InputCircles.Text, out int circles) && circles <= 0) return;
        if (!float.TryParse(InputMaxRadius.Text, NumberFormatInfo.InvariantInfo, out float maxRadius) && maxRadius <= 0) return;
        if (!int.TryParse(InputIncrement.Text, out int increment) && increment <= 0) return;

        var renderer = new Renderer();
        renderer.Render(TbCanvas, centerX, centerY, circles, maxRadius, increment);
    }
    
    private void InputOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Up or Key.Down)
        {
            if (int.TryParse(((TextBox)sender).Text, out int value))
            {
                switch (e.Key)
                {
                    case Key.Up:
                        value++;
                        break;
                    case Key.Down:
                        value--;
                        break;
                }

                ((TextBox)sender).Text = value.ToString();
                ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length; // Keep cursor at end
            }

            e.Handled = true; // Prevent default behavior
        }
    }

    private void Generate()
    {
        if (!float.TryParse(InputCenterX.Text, NumberFormatInfo.InvariantInfo, out float centerX)) return;
        if (!float.TryParse(InputCenterY.Text, NumberFormatInfo.InvariantInfo, out float centerY)) return;
        if (!int.TryParse(InputCircles.Text, out int circles) && circles <= 0) return;
        if (!float.TryParse(InputMaxRadius.Text, NumberFormatInfo.InvariantInfo, out float maxRadius) && maxRadius <= 0) return;
        if (!int.TryParse(InputIncrement.Text, out int increment) && increment <= 0) return;

        var openFileDialog = new OpenFileDialog()
        {
            Title = "Select Template File",
            Filter = "Text Files (*.txt)|*.txt",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var saveFileDialog = new SaveFileDialog()
        {
            Title = "Select Output File",
            Filter = "Text Files (*.txt)|*.txt"
        };

        if (saveFileDialog.ShowDialog() != true)
            return;

        var generator = new LightGenerator();
        generator.GenerateLight(centerX, centerY, circles, maxRadius, increment, openFileDialog.FileName, saveFileDialog.FileName);

        MessageBox.Show("Light file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
