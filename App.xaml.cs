using System.Windows;
using TestViewApp.View;
using TestViewApp.ViewModel;

namespace TestViewApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            var viewModel = new MainWindowViewModel();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Произошла ошибка."
                + Environment.NewLine + e.Exception.Message + "\r\n" + e.Exception.StackTrace, "Ошибка в приложении");
            e.Handled = true;
        }
    }

}
