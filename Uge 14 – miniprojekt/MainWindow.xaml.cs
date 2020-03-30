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

namespace Pizzeria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //https://www.forketers.com/italian-pizza-names-list/
    {
        List<float> pizzaPrices = new List<float>();
        List<string> pizzaNames = new List<string>();
        Dictionary<string, float> sizePrice = new Dictionary<string, float>();
        PizzaBase currentPizza;
        List<PizzaBase> allPizza = new List<PizzaBase>();
        string size = null;
        float finalPrice = 0;

        public MainWindow()
        {
            InitializeComponent();

            sizePrice.Add("small",15); //when displaying the size, might also want to display the price
            sizePrice.Add("normal", 20);
            sizePrice.Add("family", 30); 
            PizzaSize.SelectedIndex = 1;
            PizzaType.SelectedIndex = 0;
            TotalPriceBox.Text = "0 kr.";
            this.Title = "Pizzaria Venator Pizza Sanctum";
        }

        /// <summary>
        /// Adds the current selected pizza with its current selected size to the list of wanted pizzas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            float price;
            pizzaNames.Add(currentPizza.GetName + " (" + currentPizza.Size + ")");
            price = currentPizza.GetPrice;
            allPizza.Add(currentPizza);
            DisplayFinalPrice(price);
            DIsplayPizzaList();
        }

        private void DIsplayPizzaList()
        {
            string pizzaListWriteOut = "";
            foreach (string str in pizzaNames)
            {
                pizzaListWriteOut += str + "\n";
            }
            PizzaListBox.Text = pizzaListWriteOut;
        }

        /// <summary>
        /// Adds the price of the current pizza to the overall price and displays the final price. 
        /// </summary>
        /// <param name="priceToAdd"></param>
        private void DisplayFinalPrice(float priceToAdd)
        { //it is doing two different things, split it into the functions. 
            //should update and display the final price
            finalPrice += priceToAdd;
            TotalPriceBox.Text = finalPrice.ToString() + " kr.";
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Sets the size of the pizza.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PizzaSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PizzaSize.SelectedIndex;
            if (index == 0)
                size = "small";
            else if (index == 1)
                size = "normal";
            else if (index == 2)
                size = "family";


            if (currentPizza != null)
            {
                sizePrice.TryGetValue(size, out float value);
                currentPizza.SetBasePrize = value;
                currentPizza.Size = size;
            }
                //this.Title = size;
        }

        /// <summary>
        /// Sets the size of the pizza. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PizzaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sizePrice.TryGetValue(size, out float value);
            int index = PizzaType.SelectedIndex;
            if (index == 0)
                currentPizza = new Margherita(PriceBox, IngredientsBox, size, value);
            else if (index == 1)
                currentPizza = new Napoletana(PriceBox, IngredientsBox, size, value);
            else if (index == 2)
                currentPizza = new Sarda(PriceBox, IngredientsBox, size, value);
            currentPizza.SetBasePrize = value;
        }
    }


    public class PizzaBase
    {

        protected float basePrice;
        protected float price;
        //protected string[] ingredients;
        protected Dictionary<string, float> ingredients = new Dictionary<string, float>(); //float is the value 
        protected string sizeType; //familie, normal etc.
        protected string name; //name of the pizza
        protected uint number; //the number of the pizza
        //need a constructor that contains parameters for where text should be displayed. So use thoe : base(...) next to the derived classes's constructors
        TextBlock priceWrite;
        TextBlock pizzaNameWrite;
        TextBlock ingredientsWrite;
        TextBlock pizzaSize;

        /// <summary>
        /// The basic constructor of all pizzas
        /// </summary>
        /// <param name="priceWrite">The textbox to write the price too.</param>
        /// <param name="ingredientsWrite">The textbos to write the ingredients too.</param>
        public PizzaBase(TextBlock priceWrite, TextBlock ingredientsWrite)
        {
            this.ingredientsWrite = ingredientsWrite;
            this.priceWrite = priceWrite;
            //DisplayPrice();
        }

        public string[] GetIngredients { get => GetKeys(); }

        public float GetPrice { get => price; }

        public string GetName { get => name; }

        public uint GetNumber { get => number; }

        public string Size { get => sizeType; set => sizeType = value; }

        public float SetBasePrize { set { basePrice = value; price = CalculatePrice() + basePrice; DisplayPrice(); } }

        /// <summary>
        /// Gets the keys (the names) of the ingredients of the pizza
        /// </summary>
        /// <returns></returns>
        protected string[] GetKeys()
        {
            uint i = 0;
            List<string> keys = new List<string>();
            Task tast = Task.Factory.StartNew(
                () =>
                {
                    foreach (var key in ingredients.Keys)
                        keys.Add(key.ToString());
                }
            );
            Task.WaitAll(tast);
            string[] keyArray = new string[keys.Count];
            foreach (string key in keys)
            {
                keyArray[i] = key;
                i++;
            }
            return keyArray;
        }

        /// <summary>
        /// Gets the values (the prices) of all the ingredients of the pizza. 
        /// </summary>
        /// <returns></returns>
        protected float[] GetValues()
        {
            uint i = 0;
            List<float> values = new List<float>();
            Task tast = Task.Factory.StartNew(
                () =>
                {
                    foreach (var value in ingredients.Values)
                        values.Add(value);
                }
            );
            Task.WaitAll(tast);
            float[] valueArray = new float[values.Count];
            foreach (float value in values)
            {
                valueArray[i] = value;
                i++;
            }
            return valueArray;
        }

        /// <summary>
        /// Writes out all of the ingredients.
        /// </summary>
        public void DisplayIngredients()
        {
            string[] ingredientsStrings = GetKeys();
            string writeIngredients = "";
            foreach (string str in ingredientsStrings)
            {
                writeIngredients += str + "\n";
            }
            ingredientsWrite.Text = writeIngredients;

        }

        /// <summary>
        /// Calculates the priceo of the ingredients
        /// </summary>
        /// <returns></returns>
        protected float CalculatePrice()
        { //consider adding the final price to it. 
            float tempPrice = 0;
            float[] values = GetValues();
            foreach (float value in values)
                tempPrice += value;
            return tempPrice;
        }

        /// <summary>
        /// Displays the price of the pizza. 
        /// </summary>
        public void DisplayPrice()
        {
            priceWrite.Text = price.ToString() + " kr.";

        }

    }

    public class PizzaPolymorth : PizzaBase
    {
        public PizzaPolymorth(TextBlock priceWrite, TextBlock ingredientsWrite, string name, uint number, Dictionary<string, float> ingredients, string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)
        {
            this.name = name;
            this.ingredients = ingredients;
            this.number = number;
            sizeType = size;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }
    }

    public class Margherita : PizzaBase //maybe instead of classes, use polymoth. So a single class which consturctor takes all of the variable that decide what kind of pizza it is. But for now, you do this intil the visual and that is working
    { //See the animal simulation project. Make it so no new code is needed to be added when a new pizza is added to the menu. Maybe end up with it read all txt files in a folder for the ingredients and prices. The names come from the text file name


        public Margherita(TextBlock priceWrite, TextBlock ingredientsWrite, string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)//this(priceWrite, ingredientsWrite)
        { //base consturctor is run before the code in the derived constructor
            sizeType = size;
            this.basePrice = basePrice;

            ingredients.Add("Tomate Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Oregano", 5);
            ingredients.Add("Wheat Base", 10);
            name = "Margherita";
            number = 1;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }

        //public Margherita(TextBlock priceWrite, TextBlock ingredientsWrite) : base(priceWrite, ingredientsWrite)
        //{

        //}

    }

    public class Napoletana : PizzaBase
    {
        public Napoletana(TextBlock priceWrite, TextBlock ingredientsWrite, string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)
        {
            sizeType = size;
            this.basePrice = basePrice;
            ingredients.Add("Tomate Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Oregano", 5);
            ingredients.Add("Anchovies",30);
            ingredients.Add("Wheat Base", 10);
            name = "Napoletana";
            number = 2;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }
    }

    public class Sarda : PizzaBase
    {
        public Sarda(TextBlock priceWrite, TextBlock ingredientsWrite, string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)
        {
            sizeType = size;
            this.basePrice = basePrice;
            ingredients.Add("Tomate Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Pecorino Cheese", 12);
            ingredients.Add("Spicy Salami", 21);
            ingredients.Add("Wheat Base", 10);
            name = "Sarda";
            number = 3;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }
    }



}
