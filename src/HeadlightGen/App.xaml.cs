using System.Windows;
using KE.MSTS.HeadlightGen.ViewModels;
using KE.MSTS.HeadlightGen.Views;

namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        new MainView().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        MainViewModel? mainViewModel = (MainViewModel?)MainWindow?.DataContext;
        if (mainViewModel != null)
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
                BaseAngle = mainViewModel.BaseAngle
            };
            appSettings.Save();
        }
        base.OnExit(e);
    }
}
