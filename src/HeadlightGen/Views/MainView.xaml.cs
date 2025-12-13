using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KE.MSTS.HeadlightGen.Views
{
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

            if (!double.TryParse(textBox.Text, NumberFormatInfo.InvariantInfo, out double value))
                return;

            double increment = e.Key == Key.Up ? 1 : -1;
            textBox.Text = (value + increment).ToString(CultureInfo.InvariantCulture);
        }
    }
}
