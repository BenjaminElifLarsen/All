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

        private bool GetNumber(string value, out float? num)
        {
            num = 0;
            float num2;
            bool isText = Single.TryParse(value, out num2) ? true : false;
            if (isText)
                num = num2;
            return isText;

        }

        private void GetText(out string number1Txt, out string number2Txt)
        {
            number1Txt = number.Text;
            number2Txt = number2.Text;
        }

        private void Plus(object sender, RoutedEventArgs e)
        {
            BoolChanger(true, false, false, false);
            GetText(out string value1str, out string value2str);
            bool value1bool = GetNumber(value1str, out float? num1);
            bool value2bool = GetNumber(value2str, out float? num2);
            if (value1bool && value2bool)
            {
                float result = (float)(num1 + num2);

                resultBox.Text = result.ToString();
            }
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, true, false, false);
            BoolChanger(true, false, false, false);
            GetText(out string value1str, out string value2str);
            bool value1bool = GetNumber(value1str, out float? num1);
            bool value2bool = GetNumber(value2str, out float? num2);
            if (value1bool && value2bool)
            {
                float result = (float)(num1 - num2);

                resultBox.Text = result.ToString();
            }
        }

        private void Multiply(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, true, false);
            BoolChanger(true, false, false, false);
            GetText(out string value1str, out string value2str);
            bool value1bool = GetNumber(value1str, out float? num1);
            bool value2bool = GetNumber(value2str, out float? num2);
            if (value1bool && value2bool)
            {
                float result = (float)(num1 * num2);

                resultBox.Text = result.ToString();
            }
        }

        private void Devide(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, false, true);
            BoolChanger(true, false, false, false);
            GetText(out string value1str, out string value2str);
            bool value1bool = GetNumber(value1str, out float? num1);
            bool value2bool = GetNumber(value2str, out float? num2);
            if (value1bool && value2bool)
            {
                float result = (float)(num1 / num2);

                resultBox.Text = result.ToString();
            }
        }

        private void Number(object sender, TextChangedEventArgs e)
        {

        }
    }
}
