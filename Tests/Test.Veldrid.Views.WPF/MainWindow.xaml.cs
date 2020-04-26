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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeRendering();
        }

        const int Size = 1024;

        private void InitializeRendering()
        {
            d3D11Image.SetPixelSize(Size, Size);
            d3D11Image.WindowOwner = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
        }

        private void BnTest_Click(object sender, RoutedEventArgs e)
        {
            d3D11Image.RequestRender();
        }
    }
}
