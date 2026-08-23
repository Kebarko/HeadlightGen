using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KE.MSTS.HeadlightGen.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void CanvasLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.CanvasWidth = (int)MainCanvas.ActualWidth;
            vm.CanvasHeight = (int)MainCanvas.ActualHeight;
        }
    }

    private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.CanvasWidth = (int)MainCanvas.ActualWidth;
            vm.CanvasHeight = (int)MainCanvas.ActualHeight;
        }
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (e.Key != Key.Up && e.Key != Key.Down)
            return;

        e.Handled = true;

        if (!int.TryParse(textBox.Text, out int value))
            return;

        int increment = e.Key == Key.Up ? 1 : -1;
        textBox.Text = (value + increment).ToString();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel mainViewModel)
        {
            var appSettings = new Services.AppSettings
            {
                Title = mainViewModel.Title,
                CenterX = mainViewModel.CenterX,
                CenterY = mainViewModel.CenterY,
                CenterZ = mainViewModel.CenterZ,
                Circles = mainViewModel.Circles,
                MaxRadius = mainViewModel.MaxRadius,
                Increment = mainViewModel.Increment,
                Rotation = mainViewModel.Rotation,
                Elevation = mainViewModel.Elevation
            };
            appSettings.Save();
        }
    }
}