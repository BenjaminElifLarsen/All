using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Pen = System.Drawing.Pen;
using Rectangle = System.Drawing.Rectangle;

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
        float[] trapezoidArray = new float[3];
        float[] coneArray = new float[2];
        float polygonLength;
        uint polygonSideAmount; 

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
            circle = new Circle(resultBox, visual);
            polygon = new Polygon(resultBox, visual);
            trapezoid = new Trapezoid(resultBox, visual);
            cone = new Cone(resultBox, visual);
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
                    numberOld.Text = oldNumber.ToString();
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

        private void Circle_Click(object sender, RoutedEventArgs e)
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
                if (areaState == 0)
                {
                    AreaTextBox.Text = "Radius";
                    areaState++;
                }
                else if (areaState == 1)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        circle.Area((float)num);
                        circle.VisualRepresentation();
                        areaSelected.Text = "Correct shape?";
                        AreaBoolChanger(false, false, false, false);
                        areaState = 0;
                    }
                }
            }
            else if (trapezoidBool)
            {
                if (areaState == 0)
                {
                    AreaTextBox.Text = "Height";
                    areaState++;
                }
                else if (areaState == 1)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        trapezoidArray[0] = (float)num;
                        areaSelected.Text = "Small length";
                        areaState++;
                    }
                }
                else if (areaState == 2)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    { //need to ensure that long length is bigger than small length or does that matter?
                        //maybe ask for length 1 and length 2 and then in code just check which one is longest
                        //in case one of them is going outside the image size
                        trapezoidArray[1] = (float)num;
                        areaSelected.Text = "Long length";
                        areaState++;
                    }
                }
                else if (areaState == 3)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        trapezoidArray[2] = (float)num;
                        trapezoid.Area(trapezoidArray[0], trapezoidArray[1], trapezoidArray[2]);
                        trapezoid.VisualArrayCalculation();
                        trapezoid.VisualRepresentation();
                        areaSelected.Text = "Correct shape?";
                        AreaBoolChanger(false, false, false, false);
                        areaState = 0;
                    }
                }
            }
            else if (coneBool)
            {
                if (areaState == 0)
                {
                    AreaTextBox.Text = "Radius";
                    areaState++;
                }
                else if (areaState == 1)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        coneArray[0] = (float)num;
                        areaSelected.Text = "Height";
                        areaState++;
                    }
                }
                else if (areaState == 2)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        coneArray[1] = (float)num;
                        cone.Area(coneArray[0], coneArray[1]);
                        cone.VisualArrayCalculation();
                        cone.VisualRepresentation();
                        areaSelected.Text = "Correct shape?";
                        AreaBoolChanger(false, false, false, false);
                        areaState = 0;
                    }
                }
            }
            else if (polygonBool)
            {
                if (areaState == 0)
                {
                    AreaTextBox.Text = "Length";
                    areaState++;
                }
                else if (areaState == 1)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        polygonLength = (float)num;
                        areaSelected.Text = "Amount of sides";
                        areaState++;
                    }
                }
                else if (areaState == 2)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    {
                        polygonSideAmount = (uint)num;
                        polygon.Area(polygonSideAmount, polygonLength);
                        polygon.VisualRepresentation();
                        areaSelected.Text = "Correct shape?";
                        AreaBoolChanger(false, false, false, false);
                        areaState = 0;
                    }
                }

            }


            }

    }

    public class Shape
    {
        public PointF[] visualArray;
        TextBox resultBox;
        System.Windows.Controls.Image imageBox; 

        public virtual TextBox SetTextBox
        {
            set => resultBox = value;
        }
        public virtual System.Windows.Controls.Image SetImageBox
        {
            set => imageBox = value;
        }

        /// <summary>
        /// Calculates the area of a sqaure. 
        /// </summary>
        /// <param name="height">The height of the square.</param>
        /// <param name="length">The length of the square.</param>
        /// <returns></returns>
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
        { //takes an 2d array and draws the shape using polygons. Circle might wants it own version. 
            BitmapImage shapeImage = new BitmapImage();
            Bitmap shapeBitmap = new Bitmap(400, 300);
            Graphics g = Graphics.FromImage(shapeBitmap);
            g.Clear(System.Drawing.Color.FromArgb(15, 93, 16));
            Pen pen = new Pen(Color.FromArgb(255, 255, 255), 2);
            g.DrawPolygon(pen, visualArray);

            imageBox.Source = BitmapTobitmapImage(shapeBitmap);
        }

        public BitmapImage BitmapTobitmapImage(Bitmap bmp)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream()) //opens a memory stream, used to save an image in the stream and then load it into another type
            {
                bmp.Save(memory, ImageFormat.Bmp); //saves the bitmap to the stream
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory; //loads the bitmap into a bitmapimage
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

    }

    public class Circle : Shape
    {
        PointF[] visualArray;
        float diameter;
        Image imageBox;
        public Circle(TextBox textBox, Image imageBox)
        {
            SetTextBox(textBox);
            SetImageBox(imageBox);
            this.imageBox = imageBox;
        }

        /// <summary>
        /// Calculates the area of a circle.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns></returns>
        public double Area(float radius)
        {
            diameter = 2 * radius;
            float result = (float)(Math.PI*Math.Pow(radius,2));
            ToResultBox(result);
            return result;
        }

        public override void VisualRepresentation()
        {
            int[] center = new int[2] { 400 / 2, 300 / 2 };
            if (diameter > 300)
                diameter = 300;
            Bitmap shapeBitmap = new Bitmap(400, 300);
            Graphics g = Graphics.FromImage(shapeBitmap);
            g.Clear(System.Drawing.Color.FromArgb(15, 93, 16));
            Pen pen = new Pen(Color.FromArgb(255, 255, 255),2);
            Rectangle rect = new Rectangle(center[0] - (int)Math.Round(diameter / 2), center[1] - (int)Math.Round(diameter / 2), (int)Math.Round(diameter), (int)Math.Round(diameter));
            g.DrawEllipse(pen, rect);
            imageBox.Source = base.BitmapTobitmapImage(shapeBitmap);
        }

        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    public class Trapezoid : Shape
    {
        float height;
        float lengthSmall;
        float lengthLong;
        PointF[] visualArray;
        public Trapezoid(TextBox textBox, Image imageBox)
        {
            SetTextBox(textBox);
            SetImageBox(imageBox);
        }

        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        /// <summary>
        /// Calculates the area of a trapezoid.
        /// </summary>
        /// <param name="height">The height of the trapezoid.</param>
        /// <param name="length_small">The smaller length of the trapezoid.</param>
        /// <param name="length_long">The longer length of the trapezoid.</param>
        /// <returns></returns>
        public double Area(float height, float length_small, float length_long)
        {
            this.height = height;
            lengthSmall = length_small;
            lengthLong = length_long;
            float result = height / 2f * (length_small + length_long);
            ToResultBox(result);
            return result;
        }

        public void VisualArrayCalculation()
        {

            float biggestValue = lengthLong > height ? lengthLong: height;
            bool heightBiggest = height > lengthLong ? true: false; 
            biggestValue = biggestValue > lengthSmall ? biggestValue: lengthSmall;
            if ((biggestValue > 400 && !heightBiggest) || (biggestValue > 300 && heightBiggest))
            { //should scale the height and lengths as a %. Also, do the same for the cone.
                float scaleback;
                if (heightBiggest)
                    scaleback = 300 / biggestValue;
                else
                    scaleback = 400 / biggestValue;
                lengthLong *= scaleback;
                lengthSmall *= scaleback;
                height *= scaleback;
            }
            PointF topRight = new PointF(200f + lengthSmall / 2, 150 - height / 2);
            PointF topLeft = new PointF(200f - lengthSmall / 2, 150 - height / 2);
            PointF bottomRight = new PointF(200f + lengthLong / 2, 150 + height / 2);
            PointF bottomLeft = new PointF(200f - lengthLong / 2, 150 + height / 2);

            visualArray = new PointF[]
            {
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            };
            base.visualArray = this.visualArray;

        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    public class Polygon : Shape
    {
        PointF[] visualArray;
        uint sideAmount;

        public Polygon(TextBox textBox, Image imageBox)
        {
            SetTextBox(textBox);
            SetImageBox(imageBox);
        }

        /// <summary>
        /// Calculates the area of a polygon.
        /// </summary>
        /// <param name="amountOFSides">The amount of sides of the polygon.</param>
        /// <param name="length">The length of the sides.</param>
        /// <returns></returns>
        public double Area(uint amountOFSides, float length)
        {
            sideAmount = amountOFSides;
            float result = amountOFSides * (float)Math.Pow(length, 2) * Cot(Math.PI / amountOFSides) / 4f;
            ToResultBox(result);
            return result;
        }

        public uint[,] VisualArrayCalculation()
        {
            float angleBetweenSides = (sideAmount - 2) * 180 / sideAmount;


            return null;
        }

        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
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
        PointF[] visualArray;
        float diameter;
        float height; 

        public Cone(TextBox textBox, Image imageBox)
        {
            SetTextBox(textBox);
            SetImageBox(imageBox);
        }

        /// <summary>
        /// Calculates the area of a cone.
        /// </summary>
        /// <param name="radius">The radius of the cone.</param>
        /// <param name="height">The height of the cone.</param>
        /// <returns></returns>
        public override double Area(float radius, float height)
        {
            this.height = height;
            diameter = 2 * radius;
            float slantHeight = (float)Math.Sqrt(Math.Pow(radius,2) + Math.Pow(height, 2));
            float result = (float)Math.PI * radius * slantHeight + (float)Math.PI * (float)Math.Pow(radius,2); 
            ToResultBox(result);
            return result;
        }

        public void VisualArrayCalculation()
        {
            PointF top = new PointF(200f, 150 - height / 2);
            PointF bottomLeft = new PointF(200 - diameter / 2, 150 + height / 2);
            PointF bottomRight = new PointF(200 + diameter / 2, 150 + height / 2);
            visualArray = new PointF[3]
            {
                top,
                bottomLeft,
                bottomRight
            };
            base.visualArray = this.visualArray;
        }

        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }
    }
}
