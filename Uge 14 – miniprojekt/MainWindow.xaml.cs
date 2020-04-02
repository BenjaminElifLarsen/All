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
        List<string> drinkNames = new List<string>();
        Dictionary<string, float> sizePricePizza = new Dictionary<string, float>();
        Dictionary<string, float> sizePriceDrink = new Dictionary<string, float>();
        PizzaBase currentPizza;
        DrinkBase currentDrink;
        List<PizzaBase> allPizza = new List<PizzaBase>();
        List<DrinkBase> allDrinks = new List<DrinkBase>();

        string sizePizza = null;
        string sizeDrink = null;
        float finalPrice = 0;
        Dictionary<string, float> extraIngredients = new Dictionary<string, float>();
        Dictionary<string, float> cheese = new Dictionary<string, float>();
        Dictionary<string, float> dough = new Dictionary<string, float>();
        Dictionary<string, float> tomatoSauce = new Dictionary<string, float>();
        Dictionary<string, float> ingredientsSelected = new Dictionary<string, float>();
        Dictionary<string, float> drink = new Dictionary<string, float>();
        Dictionary<string, float> prePizzaIngredients = new Dictionary<string, float>();
        uint maxIngredients = 4;
        uint currentAmountOfIngredients = 0;
        DataList extraIngredientsList;
        DataList cheeseList;
        DataList doughList;
        DataList tomatoSauceList;
        DataList drinkList;
        DataList drinkSizeList;
        DataList pizzaSizeList;

        List<PizzaBase> prePizzaList = new List<PizzaBase>();
        DataList prePizzaDataList;

        DataList testList;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            TotalPriceBox.Text = "0 kr.";
            DrinkPrice.Text = "0 kr";
            this.Title = "Pizzaria Venator Pizza Sanctum";
            //DictionarySetUp();

            Setup();
            PrePizzaSetup();
            currentAmountOfIngredients = 0; //needs to be after DictionarySetUp, since it active the selct code 
            ingredientsSelected.Clear();
            PizzaType.ItemsSource = prePizzaDataList;
            PizzaType.SelectedIndex = 0;
        }

        private void PrePizzaSetup() //how to handle custome pizza and "none"
        {
            
            string[] lines = File.ReadAllLines("PreExistingPizza.txt");
            //string[] ingredientsPre = File.ReadAllLines("PreExistingPizzaIngredients.txt");
            uint number_ = 0;
            string name_ = null;
            string dough_ = null;
            string tomatoSauce_ = null;
            string cheese_ = null;
            float doug_Number = 0;
            float tomatoSauce_Number = 0;
            float cheese_Number = 0;
            Dictionary<string, float> preIngredients = new Dictionary<string, float>();
            uint posistion = 0;
            PizzaPolymorth prePizza;
            List<string> prePizzaNames = new List<string>();
            prePizzaNames.Add("None");
            while (posistion < lines.Length) { 
                do
                {
                    if (UInt32.TryParse(lines[posistion], out uint value))
                        number_ = value;
                    else if (name_ == null)
                        name_ = lines[posistion];
                    else
                    {
                        try
                        {
                            if(lines[posistion] != "")
                            {
                                prePizzaIngredients.TryGetValue(lines[posistion], out float price); //will not work since dough and that is not part of the extraIngredients list... so new list 
                                preIngredients.Add(lines[posistion], price);
                                if (IsADough(lines[posistion]))
                                {
                                    dough_ = lines[posistion];
                                    doug_Number = price;
                                }
                                else if (IsATomatoSauce(lines[posistion]))
                                {
                                    tomatoSauce_ = lines[posistion];
                                    tomatoSauce_Number = price;
                                }
                                else if (IsACheese(lines[posistion]))
                                {
                                    cheese_ = lines[posistion];
                                    cheese_Number = price;
                                }
                            }
                        }
                        catch
                        {
                            Debug.WriteLine("Ingredient does not exist in the Ingredients text file.");
                        }
                    }

                    posistion++;
                    if (posistion == lines.Length)
                        break;
                } while (!Single.TryParse(lines[posistion], out _));
                prePizza = new PizzaPolymorth(PriceBox, IngredientsBox, name_, number_, preIngredients);
                prePizza.SetDough(dough_, doug_Number);
                prePizza.SetCheese(cheese_, cheese_Number);
                prePizza.SetTomatoSauce(tomatoSauce_, tomatoSauce_Number);
                prePizzaList.Add(prePizza);
                prePizzaNames.Add(name_);
                name_ = null;
                dough_ = null;
                cheese_ = null;
                tomatoSauce_ = null;
                preIngredients.Clear();

            }
            prePizzaDataList = new DataList(prePizzaNames.ToArray());


        }

        private bool IsADough(string ingredientToCheck)
        {
            foreach (string str in dough.Keys)
            {
                if (str == ingredientToCheck)
                    return true;
            }
            return false;
        }

        private bool IsACheese(string ingredientToCheck)
        {
            foreach (string str in cheese.Keys)
            {
                if (str == ingredientToCheck)
                    return true;
            }
            return false;
        }

        private bool IsATomatoSauce(string ingredientToCheck)
        {
            foreach (string str in tomatoSauce.Keys)
            {
                if (str == ingredientToCheck)
                    return true;
            }
            return false;
        }

        private string[] FileReader(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            return lines;
        }

        private void Setup()
        {
            //https://stackoverflow.com/questions/4902039/difference-between-selecteditem-selectedvalue-and-selectedvaluepath


            string[] lines; float[] values; string[] items;
            lines = FileReader("PreExistingPizzaIngredients.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref prePizzaIngredients, out _, items, values);
            lines = FileReader("PizzaSize.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref sizePricePizza, out pizzaSizeList, items, values);
            lines = FileReader("Dough.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref dough, out doughList, items, values);
            lines = FileReader("Cheese.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref cheese, out cheeseList, items, values);
            lines = FileReader("Ingredients.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref extraIngredients, out extraIngredientsList, items, values);
            lines = FileReader("Tomato_Sauce.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref tomatoSauce, out tomatoSauceList, items, values);
            lines = FileReader("Drink.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref drink, out drinkList, items, values);
            lines = FileReader("DrinkSize.txt");
            TextSplit(lines, out items, out values);
            DictionaryAndComboBoxAdd(ref sizePriceDrink, out drinkSizeList, items, values);

            Cheese.ItemsSource = cheeseList;
            Dough.ItemsSource = doughList;
            Tomato_Sauce.ItemsSource = tomatoSauceList;
            Extra_Ingredients.ItemsSource = extraIngredientsList;
            Drink.ItemsSource = drinkList;
            DrinkSize.ItemsSource = drinkSizeList;
            PizzaSize.ItemsSource = pizzaSizeList;
            ResetIndexes();
            ResetDrinkIndexes();
        }

        /// <summary>
        /// Sets up the dictionaries with keys and values
        /// </summary>
        private void DictionarySetUp() //no longer used and usable
        {
            sizePricePizza.Add("Small", 15); //when displaying the size, might also want to display the price
            sizePricePizza.Add("Normal", 20);
            sizePricePizza.Add("Family", 30);

            DictionaryAdd(ref dough, Dough);
            DictionaryAdd(ref tomatoSauce, Tomato_Sauce);
            DictionaryAdd(ref extraIngredients, Extra_Ingredients);
            DictionaryAdd(ref cheese, Cheese);

            PizzaType.SelectedIndex = 0;
            ResetIndexes();
            ResetDrinkIndexes();
        }

        /// <summary>
        /// Resets the comboboxes related to the pizza to their default values.
        /// </summary>
        private void ResetIndexes()
        {
            PizzaSize.SelectedIndex = 1;
            Extra_Ingredients.SelectedIndex = 0;
            Dough.SelectedIndex = 0;
            Cheese.SelectedIndex = 0;
            Tomato_Sauce.SelectedIndex = 0;
        }

        /// <summary>
        /// Resets the comboboxes related to the drink to their default values. 
        /// </summary>
        private void ResetDrinkIndexes()
        {
            Drink.SelectedIndex = 0;
            DrinkSize.SelectedIndex = 1;
            Ice.IsChecked = false;
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

        /// <summary>
        /// ... Splits a string based upong ':'. Left side is assumed to be a string, the right side a float. 
        /// </summary>
        /// <param name="textToSplit"></param>
        /// <param name="items"></param>
        /// <param name="values"></param>
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

        private void DictionaryAndComboBoxAdd(ref Dictionary<string, float> dictionary, out DataList list, string[] items, float[] values)
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
            if (currentPizza != null)
            {
                float price;
                pizzaNames.Add(currentPizza.GetName + " (" + currentPizza.Size + ")");
                price = currentPizza.GetPrice;
                allPizza.Add(currentPizza);
                DisplayFinalPrice(price);
                DisplayOrderList();
                ingredientsSelected.Clear();
                currentAmountOfIngredients = 0;
                currentPizza = null;
                PriceBox.Text = "0 kr";
                IngredientsBox.Text = "";
                ResetIndexes();
                PizzaType.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Displays all the pizzas that have been added. 
        /// </summary>
        private void DisplayOrderList()
        {
            string listWriteOut = "";
            foreach (string str in pizzaNames)
            {
                listWriteOut += str + "\n";
            }
            foreach (string str in drinkNames)
            {
                listWriteOut += str + "\n";
            }
            PizzaListBox.Text = listWriteOut;
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
            TotalPriceBox.Text = "0 kr";
            allDrinks.Clear();
            allPizza.Clear();
            pizzaNames.Clear();
            drinkNames.Clear();
            PizzaListBox.Text = "";
            if (currentDrink != null)
            { //need to update the price boxes
                currentDrink = null;
            }
            if (currentPizza != null)
            {
                currentPizza = null;
            }
            ResetDrinkIndexes();
            ResetIndexes();
            PizzaType.SelectedIndex = 0;

        }

        /// <summary>
        /// Sets the size of the pizza.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PizzaSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sizePizza = PizzaSize.SelectedValue.ToString();
            if (currentPizza != null)
            {   //make it such that custome pizza can not have slice no can any extra be placed on a slice of pizza
                sizePricePizza.TryGetValue(sizePizza, out float value);
                currentPizza.SetBasePrize = value;
                currentPizza.Size = sizePizza;
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
            sizePricePizza.TryGetValue(sizePizza, out float value);
            int index = PizzaType.SelectedIndex;
            if (index == 0)
            {
                currentPizza = null;
                PriceBox.Text = "0 kr";
                IngredientsBox.Text = "";
            }
            else
            {
                //prePizzaList[index-1]; //minus 1 because there is one more option in the combobox, "None", than there are pizzas. 
                currentPizza = new PizzaPolymorth(PriceBox, IngredientsBox, prePizzaList[index - 1].GetName, prePizzaList[index - 1].GetNumber, prePizzaList[index - 1].GetIngredientsDictionary); //prevents overwriting the "base" pizzas

            }
            if (index != 0)
            {
                dough.TryGetValue(prePizzaList[index - 1].GetDough, out float valuePrice);
                currentPizza.SetBasePrize = value;
                currentPizza.SetDough(prePizzaList[index - 1].GetDough, valuePrice);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                foreach (string ingredient in ingredientsSelected.Keys)
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
            else if (currentPizza != null && index != 0)
            {
                //find the cheese and remove it, same should be done with the tomato_Sauce
                string cheese = currentPizza.GetCheese;
                bool removed = currentPizza.RemoveIngredient(cheese);
            }

        }

        private void TestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { //used to figure out stuff...
            //var test = TestComboBox.SelectedItem; //this is working when using the new system, but the other comboboxes do not work using the new systems...
            //string test2 = TestComboBox.SelectedValue.ToString(); //it might be SelectedValuePath="Content" that is causing a problem. With it, the SelectedValue is null, without it is the same as test
            //it seems that SelectedValuePath="Content" is needed to just get the "content" of a ComboBoxItem when it is hardwritten, but if itemsSource is used it is not needed. Perhaps the "content" is not set when set using itemsSource?
            //Try and find material to read up on about this
            //maybe it is the ComboBoxItem

            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox?view=netframework-4.8

            //https://stackoverflow.com/questions/3063320/combobox-adding-text-and-value-to-an-item-no-binding-source
            //Test.Text = test + " " + test2;
        }

        private void Drink_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Drink.SelectedIndex == 0)
            {
                currentDrink = null;
                DrinkPrice.Text = "0 kr";
                Ice.IsChecked = false;
            }
            else
            {
                sizeDrink = DrinkSize.SelectedValue.ToString();
                string drinkString = Drink.SelectedValue.ToString();
                drink.TryGetValue(drinkString, out float value);
                currentDrink = new DrinkPolymorth(DrinkPrice, drinkString, 0, sizeDrink, value, 5);
                sizePriceDrink.TryGetValue(sizeDrink, out value);
                currentDrink.SetSizePrice = value;
                currentDrink.Ice = (bool)Ice.IsChecked;
            }
        }

        private void DrinkSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (currentDrink != null)
            {

                sizeDrink = DrinkSize.SelectedValue.ToString();
                currentDrink.Size = sizeDrink;
                sizePriceDrink.TryGetValue(currentDrink.Size, out float value);
                currentDrink.SetSizePrice = value;
            }

        }

        private void AddDrink_Click(object sender, RoutedEventArgs e)
        {
            if (currentDrink != null)
            {
                float price;
                string withIce = currentDrink.Ice ? "(Ice)" : "";
                drinkNames.Add(currentDrink.GetName + " (" + currentDrink.Size + ") " + withIce);
                price = currentDrink.GetPrice;
                allDrinks.Add(currentDrink);
                DisplayFinalPrice(price);
                DisplayOrderList();
                currentDrink = null;
                DrinkPrice.Text = "0 kr";
                ResetDrinkIndexes();
            }
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (currentDrink != null)
            {
                bool iceBool = (bool)Ice.IsChecked;
                currentDrink.Ice = iceBool;
            }
            else
                Ice.IsChecked = false;
        }
    }


    //------------------------------------------------------------------------------


    public class DrinkBase
    {
        protected float basePrice;
        protected float price;
        protected float sizePrise;
        protected string sizeType;
        protected bool ice = false;
        protected string name;
        protected uint number;
        protected TextBlock priceWrite;
        protected TextBlock sizeWrite;
        protected float icePrice;

        public DrinkBase(TextBlock priceWrite, float icePrice)
        {
            this.priceWrite = priceWrite;
            this.icePrice = icePrice;
        }

        public float GetPrice { get => price; }

        public string GetName { get => name; }

        public uint GetNumber { get => number; }

        public bool Ice { get => ice; set { ice = value; /*icePrice = value ? icePrice : 0;*/ price = CalculatePrice(); DisplayPrice(); } }

        public string Size { get => sizeType; set { sizeType = value; } }

        public float SetBasePrize { set { basePrice = value; price = CalculatePrice(); DisplayPrice(); } }

        public float SetSizePrice { set { sizePrise = value; price = CalculatePrice(); DisplayPrice(); } }

        protected float CalculatePrice()
        {
            float price_ = basePrice + sizePrise;
            price_ += ice ? icePrice : 0;
            return price_;
        }

        /// <summary>
        /// Displays the price of the pizza. 
        /// </summary>
        public void DisplayPrice()
        {
            priceWrite.Text = price.ToString() + " kr.";
        }

        public void DisplaySize()
        {
            sizeWrite.Text = sizeType;
        }

    }


    public class DrinkPolymorth : DrinkBase
    {
        public DrinkPolymorth(TextBlock priceWrite, string name, uint number, string size, float basePrice, float icePrice) : base(priceWrite, icePrice)
        {
            this.name = name;
            this.number = number;
            this.basePrice = basePrice;
            sizeType = size;
            price = CalculatePrice();
            DisplayPrice();
        }



    }


    //------------------------------------------------------------------------------


    public class PizzaBase
    {

        protected float basePrice;
        protected float price;
        protected Dictionary<string, float> ingredients = new Dictionary<string, float>(); //float is the value 
        protected string sizeType; //familie, normal etc.
        protected string name; //name of the pizza
        protected uint number; //the number of the pizza
        protected string dough = "";
        protected string tomatoSauce = "";
        protected string cheese = "";
        protected TextBlock priceWrite;
        protected TextBlock pizzaNameWrite;
        protected TextBlock ingredientsWrite;
        protected TextBlock pizzaSize;

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
        /// <param name="tomatoSauce">The type of tomato sauce.</param>
        /// <param name="value">The price of the tomato sauce.</param>
        public void SetTomatoSauce(string tomatoSauce, float value)
        {
            bool added = CheckAndAddToDictionary(tomatoSauce, value, ref this.tomatoSauce);
            Update();
            if (added && this.tomatoSauce != tomatoSauce)
                this.tomatoSauce = tomatoSauce;
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


        protected bool CheckAndAddToDictionary(string keyToAdd, float valueToAdd, ref string OldKeyToRemove)
        {
            if (keyToAdd != null)
            {
                bool wasFound = false;
                string[] keys = GetKeys();
                foreach (string key in keys)
                {
                    if (key == keyToAdd)
                    {
                        wasFound = true;
                        ingredients[keyToAdd] = valueToAdd;
                        break;
                    }
                }
                if (!wasFound)
                {
                    ingredients.Add(keyToAdd, valueToAdd);
                    ingredients.Remove(OldKeyToRemove);
                    OldKeyToRemove = keyToAdd;
                    return true;
                }
                else
                    return true;

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the dough of the pizza and the price of the cheese.
        /// </summary>
        /// <param name="cheese">The type of cheese.</param>
        /// <param name="value">The price of the cheese.</param>
        public void SetCheese(string cheese, float value)
        {
            bool added = CheckAndAddToDictionary(cheese, value, ref this.cheese);
            Update();
            if (added && this.cheese != cheese)
                this.cheese = cheese;
        }

        /// <summary>
        /// Sets the dough of the pizza and the price of the dough.
        /// </summary>
        /// <param name="dough">The type of dough.</param>
        /// <param name="value">The price of the dough.</param>
        public void SetDough(string dough, float value)
        {
            bool added = CheckAndAddToDictionary(dough, value, ref this.dough);
            Update();
            if (added && this.dough != dough)
                this.dough = dough;
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
            this.ingredients.Clear();
            foreach (string key in ingredients.Keys)
            {
                ingredients.TryGetValue(key, out float value);
                this.ingredients.Add(key, value);
            }
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
            //this.ingredients = ingredients;
            foreach(string key in ingredients.Keys)
            {
                ingredients.TryGetValue(key, out float value);
                this.ingredients.Add(key, value);
            }
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


    //------------------------------------------------------------------------------


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
