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

        /// <summary>
        /// Sets the states of the differnet bools.
        /// </summary>
        /// <param name="plus_"></param>
        /// <param name="minus_"></param>
        /// <param name="devide_"></param>
        /// <param name="multiply_"></param>
        private void MathBoolChanger(bool plus_, bool minus_, bool devide_, bool multiply_)
        {
            plus = plus_;
            minus = minus_;
            devide = devide_;
            multiply = multiply_;
        }

        /// <summary>
        /// Sets the states of the differnet bools.
        /// </summary>
        /// <param name="cone_"></param>
        /// <param name="circle_"></param>
        /// <param name="polygon_"></param>
        /// <param name="trapezoid_"></param>
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

        /// <summary>
        /// Tries to parse string <paramref name="value"/> and gives it as <paramref name="num"/>.
        /// It will also return a bool depending if the string could be parsed. 
        /// If <paramref name="value"/> could not be parsed, <paramref name="num"/> will be null. 
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="num">The numerical value of <paramref name="value"/></param>
        /// <returns>Returns a bool, true if <paramref name="value"/> could be parsed, else false.</returns>
        private bool GetNumber(string value, out float? num)
        {
            num = null;
            float num2;
            bool isText = Single.TryParse(value, out num2) ? true : false;
            if (isText)
                num = num2;
            return isText;
        }

        /// <summary>
        /// Gets the string from the textbox number. 
        /// </summary>
        /// <param name="numberTxt">The string from textbox number.</param>
        private void GetText(out string numberTxt)
        {
            numberTxt = number.Text;
        }

        /// <summary>
        /// Calculates the result of the new number and the old number and writes it out. 
        /// If there is no old number, new number is written as the result. 
        /// Old number becomes the result. 
        /// </summary>
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

        /// <summary>
        /// Set the text of the areaSelected textbox, text depending on the selected shape.  
        /// </summary>
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


        private void Plus(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(true, false, false, false);
            Math();
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

        /// <summary>
        /// Enables further progress for circle calculations. 
        /// </summary>
        /// <param name="sender">What to write...</param>
        /// <param name="e">What to write...</param>
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, true, false, false);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for trapezoid calculations. 
        /// </summary>
        /// <param name="sender">What to write...</param>
        /// <param name="e">What to write...</param>
        private void Trapezoid_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, false, true);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for polygon calculations. 
        /// </summary>
        /// <param name="sender">What to write...</param>
        /// <param name="e">What to write...</param>
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, true, false);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for cone calculations. 
        /// </summary>
        /// <param name="sender">What to write...</param>
        /// <param name="e">What to write...</param>
        private void Cone_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(true, false, false, false);
            AreaPreparation();
        }

        /// <summary>
        /// If a shape have been selected, ask for information for it and gather it. Make it sound better
        /// </summary>
        /// <param name="sender">What to write...</param>
        /// <param name="e">What to write...</param>
        private void AreaVariableConfirm_Click(object sender, RoutedEventArgs e)
        {
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
                        AreaTextBox.Text = "Correct shape?";
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
                        AreaTextBox.Text = "First length";
                        areaState++;
                    }
                }
                else if (areaState == 2)
                {
                    string valueTxt = AreaTextBox.Text;
                    bool gotNumber = GetNumber(valueTxt, out float? num);
                    if (gotNumber)
                    { 
                        trapezoidArray[1] = (float)num;
                        AreaTextBox.Text = "Second length";
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
                        AreaTextBox.Text = "Correct shape?";
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
                        AreaTextBox.Text = "Height";
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
                        AreaTextBox.Text = "Correct shape?";
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
                        AreaTextBox.Text = "Amount of sides";
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
                        polygon.VisualArrayCalculation();
                        polygon.VisualRepresentation();
                        AreaTextBox.Text = "Correct shape?";
                        AreaBoolChanger(false, false, false, false);
                        areaState = 0;
                    }
                }

            }


            }

    }

    /// <summary>
    /// Class related to shapes. Should be used to inherient from.  
    /// </summary>
    public class Shape
    {
        public PointF[] visualArray;
        TextBox resultBox;
        Image imageBox;

        /// <summary>
        /// Sets the textbox the results of the calculations should be written too. 
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        public virtual TextBox SetTextBox
        {
            set => resultBox = value;
        }

        /// <summary>
        /// Sets the imagebox the graphic should be written too. 
        /// </summary>
        /// <param name="textBox">The imagebox.</param>
        public virtual Image SetImageBox
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

        /// <summary>
        /// Writes the result to the textbox <paramref name="display"/>.
        /// </summary>
        /// <param name="display">The value to display</param>
        public void ToResultBox(float display)
        {
            resultBox.Text = display.ToString();
        }


        /// <summary>
        /// Draws the visual representation to a bitmapImage and displays it.
        /// </summary>
        public virtual void VisualRepresentation()
        { //takes an 2d array and draws the shape using polygons. Circle might wants it own version. 
            BitmapImage shapeImage = new BitmapImage();
            Bitmap shapeBitmap = new Bitmap(400, 300);
            Graphics g = Graphics.FromImage(shapeBitmap);
            g.Clear(Color.FromArgb(0, 0, 0));
            Pen pen = new Pen(Color.FromArgb(255, 255, 255), 2);
            GridDraw(g);
            g.DrawPolygon(pen, visualArray);

            imageBox.Source = BitmapTobitmapImage(shapeBitmap);
        }

        /// <summary>
        /// Draws a grid. A line is draw at each 10s pixel.
        /// </summary>
        /// <param name="g">The graphic to draw on.</param>
        public void GridDraw(Graphics g)
        {
            Pen pen = new Pen(Color.FromArgb(122, 122, 122), 1);
            for (int i = 0; i < 400; i++)
            {
                bool draw = i % 10 == 0 ? true : false ;
                if (draw)
                    g.DrawLine(pen, i, 0, i, 299);
            }
            for (int i = 0; i < 300; i++)
            {
                bool draw = i % 10 == 0 ? true : false;
                if (draw)
                    g.DrawLine(pen, 0, i, 399, i);
            }
        }

        /// <summary>
        /// Converts a bitmap to a bitmapIamge. 
        /// </summary>
        /// <param name="bmp">Bitmap to convert.</param>
        /// <returns>Returns the bitmapImage.</returns>
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

    /// <summary>
    /// Class related to circles 
    /// </summary>
    public class Circle : Shape
    {
        float diameter;
        Image imageBox;

        /// <summary>
        /// Constructor for the circle <c>class</c>. Takes <paramref name="imageBox"/> and <paramref name="textBox"/> and sets them. 
        /// </summary>
        /// <param name="textBox">The textbox the class should write results too.</param>
        /// <param name="imageBox">The imagebox the class should display images too.</param>
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

        /// <summary>
        /// Draws the visual representation to a bitmapImage and displays it.
        /// </summary>
        public override void VisualRepresentation()
        {
            int[] center = new int[2] { 400 / 2, 300 / 2 };
            if (diameter > 300)
                diameter = 300;
            Bitmap shapeBitmap = new Bitmap(400, 300);
            Graphics g = Graphics.FromImage(shapeBitmap);
            g.Clear(System.Drawing.Color.FromArgb(0, 0, 0));
            Pen pen = new Pen(Color.FromArgb(255, 255, 255),2);
            Rectangle rect = new Rectangle(center[0] - (int)Math.Round(diameter / 2), center[1] - (int)Math.Round(diameter / 2), (int)Math.Round(diameter), (int)Math.Round(diameter));
            GridDraw(g);
            g.DrawEllipse(pen, rect);
            imageBox.Source = BitmapTobitmapImage(shapeBitmap);
        }

        /// <summary>
        /// Draws a grid. A line is draw at each 10s pixel.
        /// </summary>
        /// <param name="g">The graphic to draw on.</param>
        public new void GridDraw(Graphics g)
        {
            Pen pen = new Pen(Color.FromArgb(122, 122, 122), 1);
            for (int i = 0; i < 400; i++)
            {
                bool draw = i % 10 == 0 ? true : false;
                if (draw)
                    g.DrawLine(pen, i, 0, i, 299);
            }
            for (int i = 0; i < 300; i++)
            {
                bool draw = i % 10 == 0 ? true : false;
                if (draw)
                    g.DrawLine(pen, 0, i, 399, i);
            }
        }

        /// <summary>
        /// Sets the imagebox the graphic should be written too. 
        /// </summary>
        /// <param name="textBox">The imagebox.</param>
        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        /// <summary>
        /// Sets the textbox the results of the calculations should be written too. 
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    /// <summary>
    /// Class related to trapezoids 
    /// </summary>
    public class Trapezoid : Shape
    {
        float height;
        float lengthSecond;
        float lengthFirst;
        //PointF[] visualArray;

        /// <summary>
        /// Constructor for the trapezoid <c>class</c>. Takes <paramref name="imageBox"/> and <paramref name="textBox"/> and sets them. 
        /// </summary>
        /// <param name="textBox">The textbox the class should write results too.</param>
        /// <param name="imageBox">The imagebox the class should display images too.</param>
        public Trapezoid(TextBox textBox, Image imageBox)
        {
            SetTextBox(textBox);
            SetImageBox(imageBox);
        }

        /// <summary>
        /// Sets the imagebox the graphic should be written too. 
        /// </summary>
        /// <param name="textBox">The imagebox.</param>
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
            lengthSecond = length_small;
            lengthFirst = length_long;
            float result = height / 2f * (length_small + length_long);
            ToResultBox(result);
            return result;
        }

        /// <summary>
        /// Calculates the visual design of the object.
        /// </summary>
        public void VisualArrayCalculation()
        {

            float biggestValue = lengthFirst > height ? lengthFirst: height;
            bool heightBiggest = height > lengthFirst ? true: false; 
            biggestValue = biggestValue > lengthSecond ? biggestValue: lengthSecond;
            if ((biggestValue > 400 && !heightBiggest) || (biggestValue > 300 && heightBiggest))
            {
                float scaleback;
                if (heightBiggest)
                    scaleback = 300 / biggestValue;
                else
                    scaleback = 400 / biggestValue;
                lengthFirst *= scaleback;
                lengthSecond *= scaleback;
                height *= scaleback;
            }
            PointF topRight = new PointF(200f + lengthSecond / 2, 150 - height / 2);
            PointF topLeft = new PointF(200f - lengthSecond / 2, 150 - height / 2);
            PointF bottomRight = new PointF(200f + lengthFirst / 2, 150 + height / 2);
            PointF bottomLeft = new PointF(200f - lengthFirst / 2, 150 + height / 2);

            visualArray = new PointF[]
            {
                topLeft,
                topRight,
                bottomRight,
                bottomLeft
            };
            base.visualArray = this.visualArray;

        }

        /// <summary>
        /// Sets the textbox the results of the calculations should be written too. 
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }

    }

    /// <summary>
    /// Class related to polygons.
    /// </summary>
    public class Polygon : Shape
    {
        //PointF[] visualArray;
        uint sideAmount;
        float length;

        /// <summary>
        /// Constructor for the polygon <c>class</c>. Takes <paramref name="imageBox"/> and <paramref name="textBox"/> and sets them. 
        /// </summary>
        /// <param name="textBox">The textbox the class should write results too.</param>
        /// <param name="imageBox">The imagebox the class should display images too.</param>
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
        /// <returns>Returns the area of the polygon.</returns>
        public double Area(uint amountOFSides, float length)
        {
            this.length = length;
            sideAmount = amountOFSides;
            float result = amountOFSides * (float)Math.Pow(length, 2) * Cot(Math.PI / amountOFSides) / 4f;
            ToResultBox(result);
            return result;
        }

        /// <summary>
        /// Calculates the visual design of the object.
        /// </summary>
        public void VisualArrayCalculation()
        {
            float angleBetweenSides = (sideAmount - 2) * 180 / sideAmount;
            float angleFromCenterPoint = (180f - angleBetweenSides) * (float)Math.PI/180f; //radians. Remove (float)Math.PI/180f) for degrees
            uint CurrentSide = 0;
            visualArray = new PointF[sideAmount];
            float lastX = 200 - length /2;
            float lastY = 150 - length /2;
            float lengthEdgeMiddleToCenter = length / (2 * (float)Math.Tan(180*Math.PI/180 / sideAmount));
            float lengthVertexToCenter = (float)Math.Sqrt(Math.Pow(lengthEdgeMiddleToCenter, 2) + Math.Pow(length/2f, 2));
            if(lengthVertexToCenter > 300/2)
            {
                float scale = 300/2 / lengthVertexToCenter;
                lengthEdgeMiddleToCenter *= scale;
                lengthVertexToCenter *= scale;
            }
            float startX = lengthVertexToCenter; 
            float startY = 0;
            for (int i = 0; i < visualArray.Length; i++)
            {  
                float xRotated = 200+((float)Math.Cos(angleFromCenterPoint * (i)) * startX - (float)Math.Sin(angleFromCenterPoint * (i )) * startY);
                float yRotated = 150+((float)Math.Sin(angleFromCenterPoint * (i)) * startX + (float)Math.Cos(angleFromCenterPoint * (i )) * startY);
                visualArray[i] = new PointF(xRotated, yRotated);
            }
            base.visualArray = this.visualArray;
        }

        /// <summary>
        /// Sets the imagebox the graphic should be written too. 
        /// </summary>
        /// <param name="textBox">The imagebox.</param>
        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        /// <summary>
        /// Sets the textbox the results of the calculations should be written too. 
        /// </summary>
        /// <param name="textBox">The textbox.</param>
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

    /// <summary>
    /// Class related to cones
    /// </summary>
    public class Cone : Shape
    {
        //PointF[] visualArray;
        float diameter;
        float height;

        /// <summary>
        /// Constructor for the cone <c>class</c>. Takes <paramref name="imageBox"/> and <paramref name="textBox"/> and sets them. 
        /// </summary>
        /// <param name="textBox">The textbox the class should write results too.</param>
        /// <param name="imageBox">The imagebox the class should display images too.</param>
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

        /// <summary>
        /// Calculates the visual design of the object.
        /// </summary>
        public void VisualArrayCalculation()
        { //all of these should catch if Area has not been called yet or just have parameters
            bool biggestValueHeight = height > diameter? true: false;

            if (biggestValueHeight)
            {
                if (height > 300)
                {
                    float scaleBack = 300 / height;
                    height *= scaleBack;
                    diameter *= scaleBack;
                }
            }
            else
            {
                if (diameter > 400)
                {
                    float scaleBack = 400 / diameter;
                    height *= scaleBack;
                    diameter *= scaleBack;
                }
            }

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

        /// <summary>
        /// Sets the imagebox the graphic should be written too. 
        /// </summary>
        /// <param name="textBox">The imagebox.</param>
        public new void SetImageBox(Image imageBox)
        {
            base.SetImageBox = imageBox;
        }

        /// <summary>
        /// Sets the textbox the results of the calculations should be written too. 
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        public new void SetTextBox(TextBox textBox)
        {
            base.SetTextBox = textBox;
        }
    }
}
