using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace Pizzeria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //https://www.forketers.com/italian-pizza-names-list/
    {
        List<string> pizzaNames = new List<string>();
        Dictionary<string, float> sizePrice = new Dictionary<string, float>();
        PizzaBase currentPizza;
        List<PizzaBase> allPizza = new List<PizzaBase>();
        string size = null;
        float finalPrice = 0;
        Dictionary<string, float> extraIngredients = new Dictionary<string, float>();
        Dictionary<string, float> cheese = new Dictionary<string, float>();
        Dictionary<string, float> dough = new Dictionary<string, float>();
        Dictionary<string, float> tomatoSauce = new Dictionary<string, float>();
        Dictionary<string, float> ingredientsSelected = new Dictionary<string, float>();
        uint maxIngredients = 4;
        uint currentAmountOfIngredients = 0;
        DataList extraIngredientsList;
        DataList cheeseList;
        DataList doughList;
        DataList tomatoSauceList;
        DataList testList;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            TotalPriceBox.Text = "0 kr.";
            this.Title = "Pizzaria Venator Pizza Sanctum";
            DictionarySetUp();
            currentAmountOfIngredients = 0; //needs to be after DictionarySetUp, since it active the selct code 
            ingredientsSelected.Clear();

            string[] testStrings = { "Test", "TestTest" }; //to do proper binding of data to combobox it seems the entire program needs to be redone from scratch
            float[] testFloats = { 4, 5 };
            Dictionary<string, float> testDic = new Dictionary<string, float>();
            DictionaryAndComboBoxAdd(ref testDic, TestComboBox, out testList, testStrings, testFloats);
            TestComboBox.ItemsSource = testList;
            TestComboBox.SelectedIndex = 0;
            //TestComboBox.Text = testList.GetSetDataList;
            //DataContext = testList;
            //Setup();
        }

        private string[] FileReader(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            return lines;
        }

        private void Setup() //does not work. All comboboxes are null and not null at the same times, even the comboboxes that are not set using these. 
        { //if used the 3 comboboxes used here should in the XAML not have anything in them
            sizePrice.Add("Small", 15); //when displaying the size, might also want to display the price
            sizePrice.Add("Normal", 20);
            sizePrice.Add("Family", 30);
            //https://stackoverflow.com/questions/4902039/difference-between-selecteditem-selectedvalue-and-selectedvaluepath
            PizzaType.SelectedIndex = 0;

            string[] lines; float[] values; string[] items;
            lines = FileReader("Dough.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref dough, Dough, out doughList, items, values);
            lines = FileReader("Cheese.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref cheese, Cheese, out cheeseList, items, values);
            lines = FileReader("Ingredients.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref extraIngredients, Extra_Ingredients, out extraIngredientsList, items, values);
            lines = FileReader("Tomato_Sauce.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref tomatoSauce, Tomato_Sauce, out tomatoSauceList, items, values);

            Cheese.ItemsSource = cheeseList;
            
            Dough.ItemsSource = doughList;
            Tomato_Sauce.ItemsSource = tomatoSauceList;
            Extra_Ingredients.ItemsSource = extraIngredientsList;
            
            //ResetIndexes();
        }

        private void DictionarySetUp()
        {
            sizePrice.Add("Small", 15); //when displaying the size, might also want to display the price
            sizePrice.Add("Normal", 20);
            sizePrice.Add("Family", 30);

            DictionaryAdd(ref dough, Dough);
            DictionaryAdd(ref tomatoSauce, Tomato_Sauce);
            DictionaryAdd(ref extraIngredients, Extra_Ingredients);
            DictionaryAdd(ref cheese, Cheese);

            PizzaType.SelectedIndex = 0;
            ResetIndexes();
        }

        private void ResetIndexes()
        {
            PizzaSize.SelectedIndex = 1;
            Extra_Ingredients.SelectedIndex = 0;
            Dough.SelectedIndex = 0;
            Cheese.SelectedIndex = 0;
            Tomato_Sauce.SelectedIndex = 0;
        }

        private void DictionaryAdd(ref Dictionary<string, float> dictionary, ComboBox combo)
        { //https://stackoverflow.com/questions/11878217/add-items-to-combobox-in-wpf
            Random rnd = new Random();
            int amount = combo.Items.Count;
            for (int i = 0; i < amount; i++)
            {
                int value = rnd.Next(5, 25);
                combo.SelectedIndex = i;
                dictionary.Add(combo.Text, value);
            }
        }


        private void TextSplit(string[] textToSplit, out string[] items, out float[] values)
        {
            uint posistion = 0;
            items = new string[textToSplit.Length];
            values = new float[textToSplit.Length];
            foreach (string str in textToSplit)
            {
                string[] splitStr = str.Split(':');
                try
                {
                    items[posistion] = splitStr[0].Trim();
                    values[posistion] = Single.Parse(splitStr[1].Trim());
                }
                catch
                {
                    Debug.WriteLine("List is not in correct format: item : value");
                }
                posistion++;
            }
        }

        private void DictionaryAndComboBoxAdd(ref Dictionary<string, float> dictionary, ComboBox combo, out DataList list , string[] items, float[] values)
        {
            for (int i = 0; i < items.Length; i++)
            {
                dictionary.Add(items[i], values[i]);
                //combo.Items.Add(items[i]);
            }
            list = new DataList(items);
            //combo.ItemsSource = list;
        }

        /// <summary>
        /// Adds the current selected pizza with its current selected size to the list of wanted pizzas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (currentPizza != null) //if the pizza is a custom it needs to make sure it got a dough... as a minimum
            {
                float price;
                pizzaNames.Add(currentPizza.GetName + " (" + currentPizza.Size + ")");
                price = currentPizza.GetPrice;
                allPizza.Add(currentPizza);
                DisplayFinalPrice(price);
                DisplayPizzaList();
                ingredientsSelected.Clear();
                currentAmountOfIngredients = 0;
                currentPizza = null;
                PriceBox.Text = "0 kr";
                IngredientsBox.Text = "";
                PizzaSize.SelectedIndex = 1;
                PizzaType.SelectedIndex = 0;
                Extra_Ingredients.SelectedIndex = 0;
                Dough.SelectedIndex = 0;
                Cheese.SelectedIndex = 0;
                Tomato_Sauce.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Displays all the pizzas that have been added. 
        /// </summary>
        private void DisplayPizzaList()
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

            //int index = PizzaSize.SelectedIndex;
            //if (index == 0)
            //    size = "small";
            //else if (index == 1)
            //    size = "normal";
            //else if (index == 2)
            //    size = "family";
            size = PizzaSize.SelectedValue.ToString();
            if (currentPizza != null)
            {
                sizePrice.TryGetValue(size, out float value);
                currentPizza.SetBasePrize = value;
                currentPizza.Size = size;
            }
        }

        /// <summary>
        /// Sets the size of the pizza. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PizzaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetIndexes();
            sizePrice.TryGetValue(size, out float value);
            int index = PizzaType.SelectedIndex;
            if (index == 0)
            {
                currentPizza = null;
                PriceBox.Text = "0 kr";
                IngredientsBox.Text = "";
            }
            else if (index == 1)
                currentPizza = new Margherita(PriceBox, IngredientsBox, size, value);
            else if (index == 2)
                currentPizza = new Napoletana(PriceBox, IngredientsBox, size, value);
            else if (index == 3)
                currentPizza = new Sarda(PriceBox, IngredientsBox, size, value);
            else if (index == 4)
                currentPizza = new PizzaPolymorth(PriceBox, IngredientsBox, "Custom", 0, "Normal", 0);

            if (index != 0)
            {
                dough.TryGetValue("Classic", out float valuePrice);
                currentPizza.SetBasePrize = value;
                currentPizza.SetDough("Classic", valuePrice);
            }
        }

        private void Extra_Ingredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Extra_Ingredients.SelectedIndex;
            Extra_Ingredients.SelectedIndex = index;
            //string whyDoesTextGiveTheOldResultAndNotTheNewResult = Extra_Ingredients.Items[index].ToString().Split(':')[1].Trim();
            //string text = whyDoesTextGiveTheOldResultAndNotTheNewResult;
            string text = Extra_Ingredients.SelectedValue.ToString();
            extraIngredients.TryGetValue(text, out float value);

            if (currentPizza != null && index != 0)
            {
                bool exist = ExistAlreadyInDictionary(ingredientsSelected, text);
                if (currentAmountOfIngredients < maxIngredients && !exist) 
                {
                    ingredientsSelected.Add(text, value);
                    currentAmountOfIngredients++;
                    currentPizza.AddIngredients(text, value);
                }
            }
            else if (currentPizza != null && index == 0)
            {
                foreach(string ingredient in ingredientsSelected.Keys)
                {
                    currentPizza.RemoveIngredient(ingredient);
                }
                ingredientsSelected = new Dictionary<string, float>();
                currentAmountOfIngredients = 0;
            }
        }

        private string[] GetKeys(Dictionary<string, float> getKeyFrom)
        {
            uint i = 0;
            List<string> keys = new List<string>();
            Task tast = Task.Factory.StartNew(
                () =>
                {
                    foreach (var key in getKeyFrom.Keys)
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

        private bool ExistAlreadyInDictionary(Dictionary<string, float> toCheck, string keyToFind)
        {
            bool wasFound = false;
            string[] keys = GetKeys(toCheck);
            foreach (string key in keys)
            {
                if (key == keyToFind)
                {
                    wasFound = true;
                    break;
                }
            }
            return wasFound;
        }

        private void Dough_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //consider allowing the non-custom pizzas to have their dough and such changed. A foreach loop with a breaak for the ingridents list checking against the different doughs should work

            if (currentPizza != null)
            {
                int index = Dough.SelectedIndex;
                string text = Dough.SelectedValue.ToString();
                dough.TryGetValue(text, out float value);
                currentPizza.SetDough(text, value);
            }
        }

        private void Tomato_Sauce_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Tomato_Sauce.SelectedIndex;
            if (currentPizza != null)
            {
                string text = Tomato_Sauce.SelectedValue.ToString();//Items[index].ToString().Split(':')[1].Trim();
                tomatoSauce.TryGetValue(text, out float value);
                currentPizza.SetTomatoSauce(text, value);
            }
            else if (currentPizza != null && index != 0)
            {
                string tomatoSauce = currentPizza.GetTomatoSauce;
                bool removed = currentPizza.RemoveIngredient(tomatoSauce);
            }

        }

        private void Cheese_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int index = Cheese.SelectedIndex;
            if (currentPizza != null && index != 0)
            {
                string text = Cheese.SelectedValue.ToString();
                cheese.TryGetValue(text, out float value);
                currentPizza.SetCheese(text, value);
            }
            else if(currentPizza != null && index != 0)
            {
                //find the cheese and remove it, same should be done with the tomato_Sauce
                string cheese = currentPizza.GetCheese;
                bool removed = currentPizza.RemoveIngredient(cheese);
            }

        }

        private void TestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { //used to figure out stuff...
            var test = TestComboBox.SelectedItem; //this is working when using the new system, but the other comboboxes do not work using the new systems...
            string test2 = TestComboBox.SelectedValue.ToString(); //it might be SelectedValuePath="Content" that is causing a problem. With it, the SelectedValue is null, without it is the same as test
            //it seems that SelectedValuePath="Content" is needed to just get the "content" of a ComboBoxItem when it is hardwritten, but if itemsSource is used it is not needed. Perhaps the "content" is not set when set using itemsSource?
            //Try and find material to read up on about this
            //maybe it is the ComboBoxItem

            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=netframework-4.8

            //https://stackoverflow.com/questions/3063320/combobox-adding-text-and-value-to-an-item-no-binding-source
            Test.Text = test + " " + test2;
        } 
    }



    //------------------------------------------------------------------------------


    public class PizzaBase
    {

        protected float basePrice;
        protected float price;
        //protected string[] ingredients;
        protected Dictionary<string, float> ingredients = new Dictionary<string, float>(); //float is the value 
        protected string sizeType; //familie, normal etc.
        protected string name; //name of the pizza
        protected uint number; //the number of the pizza
        protected string dough = "";
        protected string tomatoSauce = "";
        protected string cheese = "";
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
        public PizzaBase(TextBlock priceWrite, TextBlock ingredientsWrite) //might not make sense to let the pizzas write out, perhaps just get the values and write out in the main code. 
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

        public string GetCheese { get => cheese; }
        public string GetDough { get => dough; }
        public string GetTomatoSauce { get => tomatoSauce; }

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

        public bool RemoveIngredient(string ingredient)
        {
            try
            {
                ingredients.Remove(ingredient);
                Update();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the dough of the pizza and the price of the tomato sauce.
        /// </summary>
        /// <param name="dough">The type of tomato sauce.</param>
        /// <param name="value">The price of the tomato sauce.</param>
        public void SetTomatoSauce(string tomatoSauce, float value)
        {
            CheckAndAddToDictionary(tomatoSauce, value, ref this.tomatoSauce);
            Update();
        }

        /// <summary>
        /// updates the price and displays the ingredients and price. 
        /// </summary>
        protected void Update()
        {
            price = basePrice + CalculatePrice();
            DisplayIngredients();
            DisplayPrice();
        }


        protected void CheckAndAddToDictionary(string keyToAdd, float valueToAdd, ref string OldKeyToRemove)
        {
            bool wasFound = false;
            string[] keys = GetKeys();
            foreach (string key in keys)
            {
                if (key == keyToAdd)
                {
                    wasFound = true;
                    ingredients[keyToAdd] = valueToAdd;
                }
            }
            if (!wasFound)
            {
                ingredients.Add(keyToAdd, valueToAdd);
                ingredients.Remove(OldKeyToRemove);
                OldKeyToRemove = keyToAdd;
            }

        }

        /// <summary>
        /// Sets the dough of the pizza and the price of the cheese.
        /// </summary>
        /// <param name="dough">The type of cheese.</param>
        /// <param name="value">The price of the cheese.</param>
        public void SetCheese(string cheese, float value)
        {
            CheckAndAddToDictionary(cheese, value, ref this.cheese);
            Update();
        }

        /// <summary>
        /// Sets the dough of the pizza and the price of the dough.
        /// </summary>
        /// <param name="dough">The type of dough.</param>
        /// <param name="value">The price of the dough.</param>
        public void SetDough(string dough, float value)
        {
            CheckAndAddToDictionary(dough, value, ref this.dough);
            Update();
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
        { //consider adding the base price to it. 
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


        public Dictionary<string, float> GetIngredientsDictionary { get => ingredients; }

        /// <summary>
        /// Adds a single ingredient and its cost to the pizza, if it does not exist. 
        /// </summary>
        /// <param name="ingredient">Ingredient to add.</param>
        /// <param name="value">Price of the ingredient.</param>
        public void AddIngredients(string ingredient, float value)
        {
            bool wasFound = false;
            string[] keys = GetKeys();
            foreach (string key in keys)
            {
                if (key == ingredient)
                {
                    wasFound = true;
                    ingredients[ingredient] = value;
                }
            }
            if (!wasFound)
            {
                ingredients.Add(ingredient, value);
                price = basePrice + CalculatePrice();
                DisplayIngredients();
                DisplayPrice();
            }
        }

        /// <summary>
        /// Uses to replace all ingredients, also dough, cheese and tomato sauce. If a single ingridents should be added use <c>AddIngredients</c>.
        /// </summary>
        /// <param name="ingredients">Dinctionary containing the new ingredients</param>
        public void SetIngredients(Dictionary<string, float> ingredients)
        {
            this.ingredients = null;
            this.ingredients = ingredients;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
            DisplayPrice();
        }

    }

    public class PizzaPolymorth : PizzaBase
    {
        public PizzaPolymorth(TextBlock priceWrite, TextBlock ingredientsWrite, string name, uint number, /*Dictionary<string, float> ingredients,*/ string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)
        {
            this.name = name;
            this.number = number;
            sizeType = size;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }

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
            ingredients.Add("Classic", 10);
            ingredients.Add("Basic Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Oregano", 5);
            name = "Margherita";
            number = 1;
            dough = "Classic";
            tomatoSauce = "Basic Sauce";
            cheese = "Mozzarella";
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
            ingredients.Add("Classic", 10);
            ingredients.Add("Basic Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Oregano", 5);
            ingredients.Add("Anchovies", 30);
            name = "Napoletana";
            number = 2;
            dough = "Classic";
            tomatoSauce = "Basic Sauce";
            cheese = "Mozzarella";
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
            ingredients.Add("Classic", 10);
            ingredients.Add("Basic Sauce", 5);
            ingredients.Add("Mozzarella", 20);
            ingredients.Add("Pecorino Cheese", 12);
            ingredients.Add("Spicy Salami", 21);
            name = "Sarda";
            number = 3;
            dough = "Classic";
            tomatoSauce = "Basic Sauce";
            cheese = "Mozzarella";
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }
    }


    public class DataList : ObservableCollection<string>
    { //https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol?view=netframework-4.8
        public DataList(string[] list)
        {
            foreach (string str in list)
                Add(str);
        }
        public string[] GetSetDataList { get; set; }
    }




}
