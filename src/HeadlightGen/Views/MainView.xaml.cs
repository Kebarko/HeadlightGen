using System.Windows;

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
    }
}
