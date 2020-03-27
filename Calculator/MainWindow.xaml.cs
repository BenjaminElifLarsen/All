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
        bool divide = false;
        bool multiply = false;
        bool coneBool = false;
        bool circleBool = false;
        bool polygonBool = false;
        bool trapezoidBool = false;
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
        /// <param name="plus_">Sets the value of the bool plus.</param>
        /// <param name="minus_">Sets the value of the bool minus.</param>
        /// <param name="divide_"> Sets the value of the bool divide.</param>
        /// <param name="multiply_">Sets the value of the bool multiply.</param>
        private void MathBoolChanger(bool plus_, bool minus_, bool divide_, bool multiply_)
        {
            plus = plus_;
            minus = minus_;
            divide = divide_;
            multiply = multiply_;
        }

        /// <summary>
        /// Sets the states of the differnet bools.
        /// </summary>
        /// <param name="cone_">Sets the value of the bool coneBool.</param>
        /// <param name="circle_">Sets the value of the bool. circleBool</param>
        /// <param name="polygon_">Sets the value of the bool polygonBool.</param>
        /// <param name="trapezoid_">Sets the value of the bool trapezoidBool.</param>
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
        private void MathCalculationSimple()
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
                    else if (divide)
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

        /// <summary>
        /// Sets plus to true, rest to false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plus(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(true, false, false, false);
            MathCalculationSimple();
        }

        /// <summary>
        /// Sets Minus to true, rest to false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minus(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, true, false, false);
            MathCalculationSimple();
        }

        /// <summary>
        /// Set multiplier to true, rest to false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Multiply(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, false, false, true);
            MathCalculationSimple();
        }

        /// <summary>
        /// Set divide to true, rest to false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Divide(object sender, RoutedEventArgs e)
        {
            MathBoolChanger(false, false, true, false);
            MathCalculationSimple();
        }

        /// <summary>
        /// Enables further progress for circle calculations. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, true, false, false);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for trapezoid calculations. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trapezoid_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, false, true);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for polygon calculations. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(false, false, true, false);
            AreaPreparation();
        }

        /// <summary>
        /// Enables further progress for cone calculations. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cone_Click(object sender, RoutedEventArgs e)
        {
            AreaBoolChanger(true, false, false, false);
            AreaPreparation();
        }

        /// <summary>
        /// If a shape have been selected, ask for information for it and gather it. Make it sound better
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Calculates the result of a math equation, <paramref name="mathString"/> and returns it.
        /// Designed to be used with <c>AddSpaceToOperators</c> and <c>IsMathEquationProper</c>. The result of these can be used as the string.
        /// </summary>
        /// <param name="mathString">The string containing the math to calculate.</param>
        /// <returns>Returns the result of <paramref name="mathString"/>.</returns>
        private float MathComplex(string[] mathString)
        {
            float result = 0;
            string[] mathSplitted = mathString;
            List<string> startList = mathSplitted.ToList();
            string[] toDoFirstOperators = { "^" };
            List<string> goneThrough0 = new List<string>();
            for (int posistion = 0; posistion < mathSplitted.Length; posistion++)
            {
                string str = mathSplitted[posistion];
                if (str == toDoFirstOperators[0])
                {
                    float leftValue = Single.Parse(goneThrough0[(int)goneThrough0.Count - 1]);
                    float rightValue;
                    if (mathSplitted[posistion + 1] == "-")
                        rightValue = -Single.Parse(mathSplitted[posistion + 2]);
                    else if (mathSplitted[posistion + 1] == "+")
                        rightValue = Single.Parse(mathSplitted[posistion + 2]);
                    else
                        rightValue = Single.Parse(mathSplitted[posistion + 1]);
                    result = (float)Math.Pow(leftValue, rightValue);
                    goneThrough0.RemoveAt(goneThrough0.Count - 1);
                    goneThrough0.Add(result.ToString());
                    posistion++;
                }
                else
                    goneThrough0.Add(str);
            }
            string[] highestPriorotyOperators = { "*", "/" };
            List<string> goneThrough1 = new List<string>();
            for (int posistion = 0; posistion < goneThrough0.Count; posistion++)
            {
                string str = goneThrough0[posistion];
                if (str == highestPriorotyOperators[0])
                {
                    float leftValue = Single.Parse(goneThrough1[(int)goneThrough1.Count - 1]);
                    float rightValue = Single.Parse(goneThrough0[posistion + 1]);
                    result = leftValue * rightValue;
                    goneThrough1.RemoveAt(goneThrough1.Count - 1);
                    goneThrough1.Add(result.ToString());
                    posistion++;
                }
                else if (str == highestPriorotyOperators[1])
                {
                    float leftValue = Single.Parse(goneThrough1[(int)goneThrough1.Count - 1]);
                    float rightValue = Single.Parse(goneThrough0[posistion + 1]);
                    result = leftValue / rightValue;
                    goneThrough1.RemoveAt(goneThrough1.Count - 1);
                    goneThrough1.Add(result.ToString());
                    posistion++;
                }
                else
                    goneThrough1.Add(str);
            }
            string[] lowestPriorotyOperatorr = { "+", "-" };
            List<string> goneThrough2 = new List<string>();
            for (int posistion = 0; posistion < goneThrough1.Count; posistion++)
            {
                string str = goneThrough1[posistion];
                if (str == lowestPriorotyOperatorr[0])
                {
                    float leftValue;
                    if (posistion == 0)
                        leftValue = 0;
                    else
                        leftValue = Single.Parse(goneThrough2[(int)goneThrough2.Count - 1]);
                    float rightValue;
                    rightValue = Single.Parse(goneThrough1[posistion + 1]);
                    result = leftValue + rightValue;
                    if (posistion != 0)
                        goneThrough2.RemoveAt(goneThrough2.Count - 1);
                    goneThrough2.Add(result.ToString());
                    posistion++;
                }
                else if (str == lowestPriorotyOperatorr[1])
                {
                    float leftValue;
                    if (posistion == 0)
                        leftValue = 0;
                    else
                        leftValue = Single.Parse(goneThrough2[(int)goneThrough2.Count - 1]);
                    float rightValue;
                    rightValue = Single.Parse(goneThrough1[posistion + 1]);
                    result = leftValue - rightValue;
                    if (posistion != 0)
                        goneThrough2.RemoveAt(goneThrough2.Count - 1);
                    goneThrough2.Add(result.ToString());
                    posistion++;
                }
                else
                    goneThrough2.Add(str);
            }
            result = Single.Parse(goneThrough2[0]);
            return result;
        }


        /// <summary>
        /// Splits an equation in its subparts, each subpart being a parentheses pair. 
        /// </summary>
        /// <param name="mathWithSpaces">The string containing the equation. Each part of the equation should be seperated using a space.</param>
        /// <returns>Returns the result of the string <paramref name="mathWithSpaces"/></returns>
        private float MathCalculationsLong(string mathWithSpaces)
        {
            float result;
            string[] mathSplitted = mathWithSpaces.Trim(' ').Split(' ');
            string[] toDoMathOn;
            List<List<string>> parenthesesStringsList = new List<List<string>>(); 
            parenthesesStringsList.Add(new List<string>());
            int currentString = 0;
            for (int n = 0; n < mathSplitted.Length; n++)
            {
                if (mathSplitted[n] == "(")
                {
                    parenthesesStringsList.Add(new List<string>());
                    currentString++;
                }
                else if (mathSplitted[n] == ")")
                {
                    toDoMathOn = parenthesesStringsList[currentString].ToArray();
                    currentString--;
                    if (toDoMathOn.Length != 0)
                    {
                        result = MathComplex(toDoMathOn); 
                        parenthesesStringsList.RemoveAt(parenthesesStringsList.Count - 1);
                        parenthesesStringsList[currentString].Add(result.ToString());
                    }
                }
                else
                {
                    if(mathSplitted[n] != " ")
                        parenthesesStringsList[currentString].Add(mathSplitted[n]);
                }
            }
            toDoMathOn = parenthesesStringsList[0].ToArray();
            result = MathComplex(toDoMathOn);
            return result;
        }


        /// <summary>
        /// Gets the string in the MathLongTextBox and furhter processes it and display its result, if it is a valid equation, or display it is not a valid equation, if it is invalid. //rewrite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongMathDone_Click(object sender, RoutedEventArgs e)
        {
            float result = 0;
            string mathFullString = MathLongTextBox.Text.Trim();
            string mathWithSpaces = AddSpaceToOperators(mathFullString);
            bool validEquation = IsMathEquationProper(mathWithSpaces);
            if (validEquation)
            {
                result = MathCalculationsLong(mathWithSpaces);
                resultBox.Text = result.ToString();
                oldNumber = result;
                numberOld.Text = oldNumber.ToString();
            }
            else
            {
                resultBox.Text = "Equation is not valid";
            }

        }

        /// <summary>
        /// Checks if a string is a valid equation. 
        /// </summary>
        /// <param name="toCheck">The string to check.</param>
        /// <returns>Returns true if the string contains a valid equation, else false.</returns>
        private bool IsMathEquationProper(string toCheck)
        {
            char lastChar = '\0';
            int posistion = 0;
            int totalParenthesesPairs = 0;
            toCheck = toCheck.Trim(' ');
            List<uint> parenthesesFound = new List<uint>();
            uint nestedLevel = 0;
            if (toCheck.Length == 0)
                return false;
            char[] toCheckCharArray = toCheck.ToArray();
            foreach (char chr in toCheck)
            {
                if (chr == '(') 
                {
                    //parenthesesCheck++;
                    parenthesesFound.Add(nestedLevel);
                    nestedLevel++;
                    if (!(lastChar == '+' || lastChar == '-' || lastChar == '*' || lastChar == '/' || lastChar == '^' || lastChar == '\0' || lastChar == '('))
                    { //if there are no operator or nothing in front of the '(', the equation is invalid
                        return false;
                    }
                }
                else if (chr == ')')
                {
                    if (parenthesesFound.Count == 0)
                        return false;
                    else
                    {
                        parenthesesFound.RemoveAt(parenthesesFound.Count - 1);
                    }
                    nestedLevel--;
                    if (lastChar == '+' || lastChar == '-' || lastChar == '*' || lastChar == '/' || lastChar == '^' || lastChar == '(')
                        return false; //if there is not a number before the ')', the equation is invalid.  
                    else if (posistion != toCheckCharArray.Length - 1)
                    {
                        char nextChar;
                        uint n = 1;
                        do //ensure the nextChar is not a space. 
                        {
                            nextChar = toCheckCharArray[posistion + n];
                            n++;
                        } while (nextChar == ' ' && posistion + n != toCheckCharArray.Length);
                        if (!(nextChar == '+' || nextChar == '-' || nextChar == '*' || nextChar == '/' || nextChar == '^' || nextChar == ')'))
                            return false; //if there are no operator after ')' and it is not the end of the equation, the equation is invalid. 
                    }
                    totalParenthesesPairs++; 
                }
                else if (chr == '^')
                {
                    if (lastChar == '*' || lastChar == '/' || lastChar == '\0' || lastChar == '^')
                    {//any of these signs cannot be present before a ^ in an equation and thus the equation is invalid.
                        return false;
                    }
                }
                else if (chr == '+' || chr == '-' || chr == '*' || chr == '/')
                {
                    if (posistion == 0 && (chr == '+' || chr == '-')) //ensures that an equation can start with - or + and not considered invalid 
                    {
                    }
                    else if (lastChar == '+' || lastChar == '-' || lastChar == '*' || lastChar == '/' || lastChar == '\0' || lastChar == '^' || lastChar == '(')
                        return false; //checks if there are multiple operators in a row or signs that would invalid an equation. 
                    else if (posistion == toCheck.Length - 1)
                        return false; //a value needs to be placed after an operator and if not, equation is invalid
                }
                else if (((int)chr < 48 || (int)chr > 57) && (int)chr != 32 && chr != '.' && chr != ',')
                { //if there are any signs that are not numbers, '.' or ',', the equation is invalid. 
                    return false;
                }
                if (chr != ' ')
                { //update the last character if the current character is not a spcace
                    lastChar = chr;
                }
                else if (chr == ' ')
                {
                    if (posistion != toCheckCharArray.Length - 1 && (((int)lastChar > 48 && (int)lastChar < 57)))
                    { //if there are no operator between values, the equation is invalid.
                        char nextChar = toCheckCharArray[posistion + 1];
                        if (((int)nextChar > 48 && (int)nextChar < 57) && (int)nextChar != 32)
                        {
                            return false;
                        }
                    }
                }
                posistion++;
            }
            if (nestedLevel != 0) //if there is an uneven amount of parentheses, the equation is invalid
                return false;
            return true;
        }

        /// <summary>
        /// Ensures that there are spaces before and after all operators. 
        /// </summary>
        /// <param name="str">The string to add spaces to.</param>
        /// <returns>Returns a version of <paramref name="str"/> with spaces before and 
        /// after any operator.</returns>
        private string AddSpaceToOperators(string str)
        {
            uint posistion = 0;
            uint lastChar = '\0';
            List<char> newStringList = new List<char>(str.Length);
            char[] operatorList = { '+', '-', '*', '/', '%', '^', '(', ')' };
            foreach (char chr in str) //does not always work. At one time "5 *5" 
            {
                bool isOperator = false;
                foreach (char chrOper in operatorList)
                {
                    if (chrOper == chr) //if the string starts or ends with a ( or a ), it will add spaces around them. 
                    {
                        if (posistion != 0)
                            if (lastChar != ' ')
                                newStringList.Add(' ');
                        newStringList.Add(chr);
                        if (posistion != str.Length - 1)
                            newStringList.Add(' ');
                        isOperator = true;
                        lastChar = ' ';
                        break;
                    }
                }
                if (!isOperator)
                {
                    if (chr != lastChar)
                    {
                        newStringList.Add(chr);
                        lastChar = '\0';
                        if (chr == ' ')
                            lastChar = ' ';

                    }

                }
                posistion++;
            }
            return new string(newStringList.ToArray());
        }

        /// <summary>
        /// Resets the old number and the resultBox and numberOld Textboxes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            oldNumber = 0;
            resultBox.Text = oldNumber.ToString();
            numberOld.Text = oldNumber.ToString();
        }
    }

    /// <summary>
    /// Class related to shapes. Should be used to inheritance.  
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
        /// <returns>Returns the area.</returns>
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
        /// <returns>Returns the area.</returns>
        public double Area(float radius)
        {
            diameter = 2 * radius;
            float result = (float)(Math.PI * Math.Pow(radius, 2));
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
            Pen pen = new Pen(Color.FromArgb(255, 255, 255), 2);
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
        /// <returns>Returns the area.</returns>
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

            float biggestValue = lengthFirst > height ? lengthFirst : height;
            bool heightBiggest = height > lengthFirst ? true : false;
            biggestValue = biggestValue > lengthSecond ? biggestValue : lengthSecond;
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
            float angleFromCenterPoint = (180f - angleBetweenSides) * (float)Math.PI / 180f; //radians. Remove (float)Math.PI/180f) for degrees
            //uint CurrentSide = 0;
            visualArray = new PointF[sideAmount];
            float lastX = 200 - length / 2;
            float lastY = 150 - length / 2;
            float lengthEdgeMiddleToCenter = length / (2 * (float)Math.Tan(180 * Math.PI / 180 / sideAmount));
            float lengthVertexToCenter = (float)Math.Sqrt(Math.Pow(lengthEdgeMiddleToCenter, 2) + Math.Pow(length / 2f, 2));
            if (lengthVertexToCenter > 300 / 2)
            {
                float scale = 300 / 2 / lengthVertexToCenter;
                lengthEdgeMiddleToCenter *= scale;
                lengthVertexToCenter *= scale;
            }
            float startX = lengthVertexToCenter;
            float startY = 0;
            for (int i = 0; i < visualArray.Length; i++)
            {
                float xRotated = 200 + ((float)Math.Cos(angleFromCenterPoint * (i)) * startX - (float)Math.Sin(angleFromCenterPoint * (i)) * startY);
                float yRotated = 150 + ((float)Math.Sin(angleFromCenterPoint * (i)) * startX + (float)Math.Cos(angleFromCenterPoint * (i)) * startY);
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
        /// <returns>Returns the area.</returns>
        public override double Area(float radius, float height)
        {
            this.height = height;
            diameter = 2 * radius;
            float slantHeight = (float)Math.Sqrt(Math.Pow(radius, 2) + Math.Pow(height, 2));
            float result = (float)Math.PI * radius * slantHeight + (float)Math.PI * (float)Math.Pow(radius, 2);
            ToResultBox(result);
            return result;
        }

        /// <summary>
        /// Calculates the visual design of the object.
        /// </summary>
        public void VisualArrayCalculation()
        { //all of these should catch if Area has not been called yet or just have parameters
            bool biggestValueHeight = height > diameter ? true : false;

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
