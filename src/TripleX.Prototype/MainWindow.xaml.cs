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

namespace TripleX.Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PhonenumberParser _parser;
        public MainWindow()
        {
            _parser = new PhonenumberParser();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = _parser.Parser(txtBox_Input.Text);

            txtBox_country.Text = result.Country;
            txtBox_area.Text = result.Area;
            txtBox_main.Text = result.Main;
            txtBox_forwarding.Text = result.Forwarding;
            lbl_error.Content = result.ErrorMessage;
        }
    }
}
