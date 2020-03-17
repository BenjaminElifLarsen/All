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
        bool coneBool = false;
        bool circleBool = false;
        bool polygonBool = false;
        bool trapezoidBool = false;
        string numberText = "";
        float? oldNumber = null;
        float resultNumber = 0;
        Circle circle;
        Polygon polygon;
        Trapezoid trapezoid;
        Cone cone;
        byte areaState = 0;

        private void MathBoolChanger(bool plus_, bool minus_, bool devide_, bool multiply_)
        {
            plus = plus_;
            minus = minus_;
            devide = devide_;
            multiply = multiply_;
        }

        private void AreaBoolChanger(bool cone_, bool circle_, bool polygon_, bool trapezoid_)
        {
            coneBool = cone_;
            circleBool = circle_;
            polygonBool = polygon_;
            trapezoidBool = trapezoid_;
        }

        public MainWindow()
        {
            InitializeComponent();
            numberOld.Text = "NaN";
            circle = new Circle(resultBox);
            polygon = new Polygon(resultBox);
            trapezoid = new Trapezoid(resultBox);
            cone = new Cone(resultBox);
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
            MathBoolChanger(true, false, false, false);
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

        private void AreaPreparation()
        {
            if (coneBool)
                areaSelected.Text = "Cone";
            else if (circleBool)
                areaSelected.Text = "Circle";
            else if (trapezoidBool)
                areaSelected.Text = "Trapezoid";
            else if (polygonBool)
                areaSelected.Text = "Polygon";
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, true, false, false);
            Math();
        }

        private void Multiply(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, false, false, true);
            Math();
        }

        private void Devide(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, false, true, false);
            Math();
        }

        private void Cicle_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, true, false, false);
            AreaPreparation();
        }

        private void Trapezoid_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, false, true);
            AreaPreparation();
        }

        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, true, false);
            AreaPreparation();
        }

        private void Cone_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(true, false, false, false);
            AreaPreparation();
        }

        private void AreaVariableConfirm_Click(object sender, RoutedEventArgs e)
        {
            //get the variables, do the calculations.
            if (circleBool)
            {
                if(areaState == 0)
                {
                    AreaTextBox.Text = "Radius";
                    areaState++;
                }
                else if(areaState == 1)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                        circle.Area((float)num);

                    areaSelected.Text = "";
                    AreaBoolChanger(false, false, false, false);
                }

            }


        }
    }

    public class Shape
    {
        TextBox resultBox;

        public virtual TextBox SetTextBox
        {
            set => resultBox = value;
        }

        public virtual double Area(float height, float length)
        {
            float result = height * length;
            ToResultBox(result);
            return result;
        }

        public void ToResultBox(float display)
        {
            resultBox.Text = display.ToString();
        }

        public virtual void VisualRepresentation()
        {

        }

    }

    public class Circle : Shape
    {

        public Circle(TextBox textBox)
        {
            SetTextBox(textBox);
        }

        public double Area(float radius)
        {
            float result = (float)(Math.PI*Math.Pow(radius,2));
            ToResultBox(result);
            return result;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    public class Trapezoid : Shape
    {

        public Trapezoid(TextBox textBox)
        {
            SetTextBox(textBox);
        }

        public double Area(float height, float length_small, float length_long)
        {
            float result = height / 2f * (length_small + length_long);
            ToResultBox(result);
            return result;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    public class Polygon : Shape
    {

        public Polygon(TextBox textBox)
        {
            SetTextBox(textBox);
        }

        public double Area(uint amountOFSides, float length)
        {
            float result = amountOFSides * (float)Math.Pow(length, 2) * Cot(Math.PI / amountOFSides) / 4f;
            ToResultBox(result);
            return result;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

        /// <summary>
        /// Calculates and returns the Cot of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value given to Cot.</param>
        /// <returns>Returns the Cot of <paramref name="value"/>.</returns>
        public static float Cot(double value) //consider moving this into its own class or liberay
        {
            return 1f / (float)Math.Tan(value);
        }

    }

    public class Cone : Shape
    {

        public Cone(TextBox textBox)
        {
            SetTextBox(textBox);
        }

        public override double Area(float radius, float height)
        {
            float slantHeight = (float)Math.Sqrt(Math.Pow(radius,2) + Math.Pow(height, 2));
            float result = (float)Math.PI * radius * slantHeight + (float)Math.PI * (float)Math.Pow(radius,2); 
            ToResultBox(result);
            return result;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }
    }
}
