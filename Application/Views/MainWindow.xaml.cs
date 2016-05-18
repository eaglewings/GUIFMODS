using MainApp.ViewModels;
using System.Windows;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void RadarChart_AxisClicked(object sender, Controls.AxisClickedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).GenerateData();
        }
    }
}
