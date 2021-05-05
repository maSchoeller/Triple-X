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

using TripleX.Core.Prototype;

namespace TripleX.Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parser = new PhonenumberParser(txtBox_Input.Text);
            try
            {
                var result = parser.GetPhonenumber();
                txtBox_country.Text = result.Country?.ToString() ?? txt_customeCountryCode.Text;
                txtBox_area.Text = result.Area.ToString();
                txtBox_main.Text = result.Main.ToString();
                txtBox_forwarding.Text = result.Forwarding!.ToString();

            }
            catch (Exception ex)
            {
                lbl_error.Content = ex.Message;
                return;
            }
        }
    }
}
