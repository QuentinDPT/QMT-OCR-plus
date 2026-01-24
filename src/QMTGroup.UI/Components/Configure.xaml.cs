using QMTGroup.IO.Camera;
using QMTGroup.Models.Camera;
using System.Windows;
using System.Windows.Controls;

namespace QMTGroup.UI.Components
{
    /// <summary>
    /// Logique d'interaction pour Configure.xaml
    /// </summary>
    public partial class Configure : Page
    {
        public Configure()
        {
            InitializeComponent();
        }


        private void BtnStartCamera_Click(object sender, RoutedEventArgs e)
        {
            BtnStartCamera.IsEnabled = false;
            ((MainWindow)System.Windows.Application.Current.MainWindow).StartCamera(() => new USBCamera(0));
        }

        private void BtnAddFilter_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).ToggleFilters();
        }

        private void BtnFillMode_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).ChangeFillMode();
        }
    }
}
