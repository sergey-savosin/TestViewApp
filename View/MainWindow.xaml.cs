using System.Windows;
using TestViewApp.ViewModel;

namespace TestViewApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel m_ViewModel = null!;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMainGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = (MainWindowViewModel)this.DataContext;
        }
    }
}