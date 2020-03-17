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
        float? oldNumber = null;
        float resultNumber = 0;


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
            numberOld.Text = "NaN";
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

        private void GetText(out string number1Txt)
        {
            number1Txt = number.Text;
        }

        private void Plus(object sender, RoutedEventArgs e)
        {
            BoolChanger(true, false, false, false);
            Math();
        }

        private void Math()
        {
            GetText(out string newNumText);
            bool isNumber = GetNumber(newNumText, out float? newNum);
            if (isNumber)
            {
                if (oldNumber != null)
                {
                    if (plus)
                        resultNumber = (float)oldNumber + (float)newNum;
                    else if (minus)
                        resultNumber = (float)oldNumber - (float)newNum;
                    else if (multiply)
                        resultNumber = (float)oldNumber * (float)newNum;
                    else if (devide)
                        resultNumber = (float)oldNumber / (float)newNum;
                    resultBox.Text = resultNumber.ToString();
                    oldNumber = resultNumber;
                    numberOld.Text = oldNumber.ToString();
                }
                else
                {
                    oldNumber = newNum;
                    numberOld.Text = oldNumber.ToString();
                }
            }
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, true, false, false);
            Math();
        }

        private void Multiply(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, false, true);
            Math();
        }

        private void Devide(object sender, RoutedEventArgs e)
        {
            BoolChanger(false, false, true, false);
            Math();
        }


    }
}
