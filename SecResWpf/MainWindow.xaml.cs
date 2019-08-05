using SecResWpf.Cef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecResWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += new EventHandler(MainWindow_Closed);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            CefTools.Down();
        }

        private async void YStockPricesBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadYahooStockPrices.LoadAsync();
        }
    }
}
