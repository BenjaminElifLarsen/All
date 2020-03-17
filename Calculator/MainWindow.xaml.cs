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

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool plus = false;
        bool minus = false;
        bool devide = false;
        bool multiply = false;
        string numberText = "";
        float oldNumber = 0;
        float newNumber = 0;


        private void BoolChanger(bool plus_, bool minus_, bool devide_, bool multiply_)
        {
            plus = plus_;
            minus = minus_;
            devide = devide_;
            multiply = multiply_;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool GetNumber(out float? num)
        {
            string numText = number.Text;
            number.Text = "";
            num = 0;
            float num2;
            bool isText = Single.TryParse(numberText, out num2) ? true : false;
            if (isText)
                num = num2;
            return isText;

        }

        private void Plus(object sender, RoutedEventArgs e)
        {
            BoolChanger(true, false, false, false);
            
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, true, false, false);
        }

        private void Multiply(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, true, false);
        }

        private void Devide(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, false, true);
        }

        private void Number(object sender, TextChangedEventArgs e)
        {

        }
    }
}
