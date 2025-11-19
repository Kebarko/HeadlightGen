using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
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
    private const string SettingsPath = "appsettings.json";

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

        string? json = File.Exists(SettingsPath)
            ? File.ReadAllText(SettingsPath, Encoding.UTF8)
            : null;

        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    InputCenterX.Text = settings.CenterX ?? string.Empty;
                    InputCenterY.Text = settings.CenterY ?? string.Empty;
                    InputCircles.Text = settings.Circles ?? string.Empty;
                    InputMaxRadius.Text = settings.MaxRadius ?? string.Empty;
                    InputIncrement.Text = settings.Increment ?? string.Empty;
                }
            }
            catch
            {
                // Ignore errors
            }
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        string json = JsonSerializer.Serialize(new AppSettings
        {
            CenterX = InputCenterX.Text,
            CenterY = InputCenterY.Text,
            Circles = InputCircles.Text,
            MaxRadius = InputMaxRadius.Text,
            Increment = InputIncrement.Text
        }, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(SettingsPath, json, Encoding.UTF8);
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

        var generator = new LightGenerator();
        generator.GenerateLight(centerX, centerY, circles, maxRadius, increment, openFileDialog.FileName, saveFileDialog.FileName);

        MessageBox.Show("Light file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
