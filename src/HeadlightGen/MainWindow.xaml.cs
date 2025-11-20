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
/// Interaction logic for MainWindow.xaml.
/// Main UI window for the HeadlightGen application that allows users to configure
/// and generate headlight point patterns with real-time preview.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// The file path to the application settings JSON file.
    /// </summary>
    private readonly string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    /// <summary>
    /// Initializes a new instance of the MainWindow class and sets up event handlers.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        InputCircles.PreviewKeyDown += InputOnPreviewKeyDown;
        InputIncrement.PreviewKeyDown += InputOnPreviewKeyDown;
        InputBaseAngle.PreviewKeyDown += InputOnPreviewKeyDown;

        Loaded += (_, _) => Redraw();
        InputCenterX.TextChanged += (_, _) => Redraw();
        InputCenterY.TextChanged += (_, _) => Redraw();
        InputCircles.TextChanged += (_, _) => Redraw();
        InputMaxRadius.TextChanged += (_, _) => Redraw();
        InputIncrement.TextChanged += (_, _) => Redraw();
        InputBaseAngle.TextChanged += (_, _) => Redraw();
        TbCanvas.SizeChanged += (_, _) => Redraw();

        GenerateButton.Click += (_, _) => Generate();
    }

    /// <summary>
    /// Called when the window is initialized. Loads persisted settings from the JSON file.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        string? json = File.Exists(settingsPath)
            ? File.ReadAllText(settingsPath, Encoding.UTF8)
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
                    InputBaseAngle.Text = settings.BaseAngle ?? string.Empty;
                }
            }
            catch
            {
                // Ignore errors
            }
        }
    }

    /// <summary>
    /// Called when the window is closing. Saves the current settings to the JSON file.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        string json = JsonSerializer.Serialize(new AppSettings
        {
            CenterX = InputCenterX.Text,
            CenterY = InputCenterY.Text,
            Circles = InputCircles.Text,
            MaxRadius = InputMaxRadius.Text,
            Increment = InputIncrement.Text,
            BaseAngle = InputBaseAngle.Text
        }, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(settingsPath, json, Encoding.UTF8);
    }

    /// <summary>
    /// Redraws the canvas with the current input values, performing validation and scaling.
    /// Updates the preview whenever user input changes or the window resizes.
    /// </summary>
    private void Redraw()
    {
        TbCanvas.Children.Clear();

        if (!float.TryParse(InputCenterX.Text, NumberFormatInfo.InvariantInfo, out float centerX)) return;
        if (!float.TryParse(InputCenterY.Text, NumberFormatInfo.InvariantInfo, out float centerY)) return;
        if (!int.TryParse(InputCircles.Text, out int circles) && circles <= 0) return;
        if (!float.TryParse(InputMaxRadius.Text, NumberFormatInfo.InvariantInfo, out float maxRadius) && maxRadius <= 0) return;
        if (!int.TryParse(InputIncrement.Text, out int increment) && increment <= 0) return;
        if (!int.TryParse(InputBaseAngle.Text, out int baseAngle) && baseAngle <= 0) return;

        var renderer = new Renderer();
        renderer.Render(TbCanvas, centerX, centerY, circles, maxRadius, increment, baseAngle);
    }
    
    /// <summary>
    /// Handles keyboard input for numeric text boxes, allowing up/down arrow keys to increment/decrement values.
    /// </summary>
    /// <param name="sender">The text box control that triggered the event.</param>
    /// <param name="e">The keyboard event arguments.</param>
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

    /// <summary>
    /// Generates and exports the light point pattern to a file.
    /// Prompts the user to select a template file and output file location.
    /// </summary>
    private void Generate()
    {
        if (!float.TryParse(InputCenterX.Text, NumberFormatInfo.InvariantInfo, out float centerX)) return;
        if (!float.TryParse(InputCenterY.Text, NumberFormatInfo.InvariantInfo, out float centerY)) return;
        if (!int.TryParse(InputCircles.Text, out int circles) && circles <= 0) return;
        if (!float.TryParse(InputMaxRadius.Text, NumberFormatInfo.InvariantInfo, out float maxRadius) && maxRadius <= 0) return;
        if (!int.TryParse(InputIncrement.Text, out int increment) && increment <= 0) return;
        if (!int.TryParse(InputBaseAngle.Text, out int baseAngle) && baseAngle <= 0) return;

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

        var exporter = new Exporter();
        exporter.Export(centerX, centerY, circles, maxRadius, increment, baseAngle, openFileDialog.FileName, saveFileDialog.FileName);

        MessageBox.Show("Light file generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
