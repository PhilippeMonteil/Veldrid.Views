using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Test.Veldrid.Views.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Grid0.SizeChanged += Grid0_SizeChanged;
        }

        private void Grid0_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => updateSize());
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // IMPORTANT
            d3D11Image.WindowOwner = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
            d3D11Image.RequestRender();
        }

        void updateSize()
        {
            double _w = Grid0.ColumnDefinitions[0].ActualWidth;
            double _h = Grid0.RowDefinitions[0].ActualHeight;
            Debug.WriteLine($" {nameof(updateSize)} _w={_w} _h={_h}");
            d3D11Image.SetPixelSize((int)_w, (int)_h); // RequestRender est inutile
        }

        int click;

        private void BnTest_Click(object sender, RoutedEventArgs e)
        {
            int _click = click++;
            Debug.WriteLine($" Image0.ActualWidth={Image0.ActualWidth} {Image0.ActualHeight}");
            d3D11Image.RequestRender();
        }
    }
}
