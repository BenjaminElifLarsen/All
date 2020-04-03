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

        string mostExpensiveDough_Name = null;
        float? mostExpensiveDough_Price = null;
        bool discount = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            TotalPriceBox.Text = "0 kr.";
            DrinkPrice.Text = "0 kr";
            this.Title = "Pizzaria Venator Pizza Sanctum";

            Setup();
            PrePizzaSetup();
            currentAmountOfIngredients = 0; //needs to be after DictionarySetUp, since DictionarySetUp activate the selct code 
            ingredientsSelected.Clear();
            PizzaType.ItemsSource = prePizzaDataList;
            PizzaType.SelectedIndex = 0;
        } //should really decouple as many functions as possible to make them easier to use in other projects

        /// <summary>
        /// Sets up all the pre-existing pizzas. Also add the option for no pizza "None". 
        /// </summary>
        private void PrePizzaSetup() 
        {
            string[] lines = File.ReadAllLines("PreExistingPizza.txt");
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
                                prePizzaIngredients.TryGetValue(lines[posistion], out float price); //need to capture errors that can be caused in a line contain data that does not exist in any of the dictionaries. 
                                preIngredients.Add(lines[posistion], price); //should it discard the whole pizza? Just the ingredients? Just the ingredients would be a problem if it is a dough...
                                if (IsInDictionary(lines[posistion],dough))
                                {
                                    dough_ = lines[posistion];
                                    doug_Number = price;
                                }
                                else if (IsInDictionary(lines[posistion], tomatoSauce))
                                {
                                    tomatoSauce_ = lines[posistion];
                                    tomatoSauce_Number = price;
                                }
                                else if (IsInDictionary(lines[posistion], cheese))
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

        /// <summary>
        /// Checks if a string appears in the dough dictionary. 
        /// </summary>
        /// <param name="ingredientToCheck"></param>
        /// <returns></returns>
        private bool IsInDictionary(string ingredientToCheck, Dictionary<string,float> toCheck)
        {
            foreach (string str in toCheck.Keys)
            {
                if (str == ingredientToCheck)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reads all the lines in <paramref name="filename"/> and returns them in an array. If the file cannot be found, it returns null
        /// </summary>
        /// <param name="filename">File to find</param>
        /// <returns>If file is found, returns a string array, else it will return null.</returns>
        private string[] FileReader(string filename)
        {
            try
            {
                string[] lines = File.ReadAllLines(filename);
                return lines;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Sets up all of the dictionaries, Datalists, comboboxes, and indexes.
        /// </summary>
        private void Setup()
        {
            //https://stackoverflow.com/questions/4902039/difference-between-selecteditem-selectedvalue-and-selectedvaluepath


            string[] lines; float[] values; string[] items;
            lines = FileReader("PreExistingPizzaIngredients.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref prePizzaIngredients, out _, items, values);
            lines = FileReader("PizzaSize.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref sizePricePizza, out pizzaSizeList, items, values);
            lines = FileReader("Dough.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref dough, out doughList, items, values);
            lines = FileReader("Cheese.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref cheese, out cheeseList, items, values);
            lines = FileReader("Ingredients.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref extraIngredients, out extraIngredientsList, items, values);
            lines = FileReader("Tomato_Sauce.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref tomatoSauce, out tomatoSauceList, items, values);
            lines = FileReader("Drink.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref drink, out drinkList, items, values);
            lines = FileReader("DrinkSize.txt");
            TextSplit(lines, out items, out values);
            DictionaryAdd(ref sizePriceDrink, out drinkSizeList, items, values);

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

        /// <summary>
        /// Splits up all strings in <paramref name="textToSplit"/>. Splits a string based upong ':'. Left side is assumed to be a string, the right side a float. 
        /// If a string is not correctly set up, that string will not be placed in the two arrays.
        /// </summary>
        /// <param name="textToSplit">Array of strings to split.</param>
        /// <param name="items">The items in the string array.</param>
        /// <param name="values">The price of the items in the string array.</param>
        private void TextSplit(string[] textToSplit, out string[] items, out float[] values)
        {
            List<string> stringList = new List<string>();
            List<float> floatList = new List<float>();
            foreach (string str in textToSplit)
            {
                string[] splitStr = str.Split(':');
                try
                {
                    stringList.Add(splitStr[0].Trim());
                    floatList.Add(Single.Parse(splitStr[1].Trim()));
                }
                catch
                {
                    stringList.RemoveAt(stringList.Count - 1);
                    Debug.WriteLine("List is not in correct format: item : value");
                }
            }
            items = stringList.ToArray();
            values = floatList.ToArray();
        }

        /// <summary>
        /// Fills up a <paramref name="dictionary"/>, keys being <paramref name="keys"/> and the values being <paramref name="values"/>. Returns both the dictionary and a DataList <paramref name="list"/> containing the keys.
        /// </summary>
        /// <param name="dictionary">The dictionary to fill up.</param>
        /// <param name="list">The Datalist of keys.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="values">The values.</param>
        private void DictionaryAdd(ref Dictionary<string, float> dictionary, out DataList list, string[] keys, float[] values) //consider a better name
        { //should capture if keys and values got different lengths and handle that error. If more keys than values it will cause a indexOfOutRange, more values than keys will not cause an error.
            for (int i = 0; i < keys.Length; i++)
            {
                dictionary.Add(keys[i], values[i]);
            }
            list = new DataList(keys);
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
                pizzaNames.Add(currentPizza.GetName + " (" + currentPizza.Size + ")");
                allPizza.Add(currentPizza);
                DisplayOrderList();
                DisplayFinalPrice();
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
        /// Displays all the wares that have been added. If a discount has been earned, it will also be displayed.  
        /// </summary>
        private void DisplayOrderList()
        {
            discount = Discound(out mostExpensiveDough_Name, out mostExpensiveDough_Price);
            string listWriteOut = "";
            foreach (string str in pizzaNames)
            {
                listWriteOut += str + "\n";
            }
            foreach (string str in drinkNames)
            {
                listWriteOut += str + "\n";
            }
            if (discount)
            {
                listWriteOut += "Discount (" + mostExpensiveDough_Name + " dough) - " + mostExpensiveDough_Price + "kr.\n";
                //DisplayFinalPrice(-(float)mostExpensiveDough_Price);
            }
            PizzaListBox.Text = listWriteOut;
        }

        /// <summary>
        /// Cheecks if the customer is privileged to a discord. If they have not earned one, <paramref name="doughExpensive"/> and <paramref name="doughPrice"/> are null.
        /// </summary>
        /// <param name="doughExpensive">The name of the dough they have earned a discond on.</param>
        /// <param name="doughPrice">The price of the dough they have earned a discond on.</param>
        /// <returns>Returns true if a discound has been earned, else false.</returns>
        private bool Discound(out string doughExpensive, out float? doughPrice)
        {

            if(pizzaNames.Count >= 2 && drinkNames.Count >= 2)
            {
                doughPrice = 0;
                doughExpensive = "";
                foreach (PizzaBase pizza in allPizza)
                {
                    string doughUsed = pizza.GetDough;
                    dough.TryGetValue(doughUsed, out float value);
                    doughPrice = doughPrice > value ? doughPrice : value;
                    doughExpensive = doughPrice > value ? doughExpensive : doughUsed;
                }
                return true;
            }
            doughExpensive = null;
            doughPrice = null;
            return false;
        }

        /// <summary>
        /// Calculates the final price and displays it.
        /// </summary>
        private void DisplayFinalPrice()
        {
            float price_ = 0;
            if (discount)
                price_ -= (float)mostExpensiveDough_Price;
            foreach(PizzaBase pizza in allPizza)
            {
                price_ += pizza.GetPrice;
            }
            foreach(DrinkBase drink in allDrinks)
            {
                price_ += drink.GetPrice;
            }
            finalPrice = price_;
            TotalPriceBox.Text = finalPrice.ToString() + " kr.";
        }

        /// <summary>
        /// Resests list of added pizzas, drinks, indexes, and values regarding the textboxes, and the textboxes themselves. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_Click(object sender, RoutedEventArgs e)
        {
            TotalPriceBox.Text = "0 kr";
            allDrinks.Clear();
            allPizza.Clear();
            pizzaNames.Clear();
            drinkNames.Clear();
            PizzaListBox.Text = "";
            if (currentDrink != null)
                currentDrink = null;
            if (currentPizza != null)
                currentPizza = null;
            ResetDrinkIndexes();
            ResetIndexes();
            PizzaType.SelectedIndex = 0;
            discount = false;
            mostExpensiveDough_Name = null;
            mostExpensiveDough_Price = null;
            finalPrice = 0;

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
                currentPizza.SetBasePrice = value;
                currentPizza.Size = sizePizza;
            }
        }

        /// <summary>
        /// Sets the current pizza out from the chosen pizza on the pizza combobox. 
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
                dough.TryGetValue(prePizzaList[index - 1].GetDough, out float valuePrice);
                currentPizza.SetBasePrice = value;
                currentPizza.SetDough(prePizzaList[index - 1].GetDough, valuePrice);
                currentPizza.SetCheese(prePizzaList[index - 1].GetCheese, valuePrice);
                currentPizza.SetTomatoSauce(prePizzaList[index - 1].GetTomatoSauce, valuePrice);
            }
        }

        /// <summary>
        /// Adds extra ingredients to a pizza, if the limit on extra ingredients have not been reached. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Extra_Ingredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Extra_Ingredients.SelectedIndex;
            Extra_Ingredients.SelectedIndex = index;
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

        /// <summary>
        /// Gets the string keys from <paramref name="getKeyFrom"/> dictionary.
        /// </summary>
        /// <param name="getKeyFrom">Dictionary to get the keys from.</param>
        /// <returns>Returns an string array consisting of the keys of <paramref name="getKeyFrom"/> dictionary.</returns>
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

        /// <summary>
        /// Chekcs if a string key <paramref name="keyToFind"/> already exist or not in dictionary <paramref name="keyToFind"/>.
        /// </summary>
        /// <param name="toCheck">Dictionary to check.</param>
        /// <param name="keyToFind">Key to check exist.</param>
        /// <returns>Returns true if <paramref name="keyToFind"/> is present, else false.</returns>
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

        /// <summary>
        /// Sets the selected dough as the dough and its price of the pizza. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the selected tomato sauce as the tomato sauce and its price of the pizza. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the selected cheese as the cheese and its price of the pizza. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the drink. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Sets the size of the drink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Adds the drink to the order list. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDrink_Click(object sender, RoutedEventArgs e)
        {
            if (currentDrink != null)
            {
                float price;
                string withIce = currentDrink.Ice ? "(Ice)" : "";
                drinkNames.Add(currentDrink.GetName + " (" + currentDrink.Size + ") " + withIce);
                price = currentDrink.GetPrice;
                allDrinks.Add(currentDrink);
                DisplayOrderList();
                DisplayFinalPrice();
                currentDrink = null;
                DrinkPrice.Text = "0 kr";
                ResetDrinkIndexes();
            }
        }

        /// <summary>
        /// Sets whether the drink contains ice or not. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Returns the price of the product.
        /// </summary>
        public float GetPrice { get => price; }

        /// <summary>
        /// Returns the name of the product.
        /// </summary>
        public string GetName { get => name; }

        /// <summary>
        /// Returns the number of the product.
        /// </summary>
        public uint GetNumber { get => number; }

        /// <summary>
        /// True if products contain ice, false otherwise. Returns true if the products contain ice, else false. Also updates the price.  
        /// </summary>
        public bool Ice { get => ice; set { ice = value; /*icePrice = value ? icePrice : 0;*/ price = CalculatePrice(); DisplayPrice(); } }

        /// <summary>
        /// Gets and sets the size of the product.
        /// </summary>
        public string Size { get => sizeType; set { sizeType = value; } }

        /// <summary>
        /// Sets the base price of the product and updates the price. 
        /// </summary>
        public float SetBasePrize { set { basePrice = value; price = CalculatePrice(); DisplayPrice(); } }

        /// <summary>
        /// Sets the size price of the product and updates the price. 
        /// </summary>
        public float SetSizePrice { set { sizePrise = value; price = CalculatePrice(); DisplayPrice(); } }

        /// <summary>
        /// Calculates the price of the product.
        /// </summary>
        /// <returns>Returns the price of the product.</returns>
        protected float CalculatePrice()
        {
            float price_ = basePrice + sizePrise;
            price_ += ice ? icePrice : 0;
            return price_;
        }

        /// <summary>
        /// Displays the price of the product. 
        /// </summary>
        public void DisplayPrice()
        {
            priceWrite.Text = price.ToString() + " kr.";
        }

        /// <summary>
        /// Displays the size of the product. 
        /// </summary>
        public void DisplaySize()
        {
            sizeWrite.Text = sizeType;
        }

    }

    /// <summary>
    /// Class for polymorth of drinks.
    /// </summary>
    public class DrinkPolymorth : DrinkBase
    {
        /// <summary>
        /// Constructor for the drink polymorth class. 
        /// </summary>
        /// <param name="priceWrite">Price Textblock.</param>
        /// <param name="name">Name of the product.</param>
        /// <param name="number">Number of the product.</param>
        /// <param name="size">The size of the product.</param>
        /// <param name="basePrice">The base price of the product.</param>
        /// <param name="icePrice">The price of ice, if added to the product. </param>
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


    /// <summary>
    /// Class to be derive from regarding pizzas. 
    /// </summary>
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

        /// <summary>
        /// Gets the ingredients of the product.
        /// </summary>
        public string[] GetIngredients { get => GetKeys(); }

        /// <summary>
        /// Gets the price of the product.
        /// </summary>
        public float GetPrice { get => price; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string GetName { get => name; }

        /// <summary>
        /// Gets the number of the product.
        /// </summary>
        public uint GetNumber { get => number; }

        /// <summary>
        /// Get and sets the size of the products. 
        /// </summary>
        public string Size { get => sizeType; set => sizeType = value; }

        /// <summary>
        /// Sets the base price of the products and updates the price. 
        /// </summary>
        public float SetBasePrice { set { basePrice = value; price = CalculatePrice() + basePrice; DisplayPrice(); } }

        /// <summary>
        /// Gets the cheese of the product.
        /// </summary>
        public string GetCheese { get => cheese; }

        /// <summary>
        /// Gets the dough of the product.
        /// </summary>
        public string GetDough { get => dough; }

        /// <summary>
        /// Gets  the tomato sauce of the product.
        /// </summary>
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

        /// <summary>
        /// Removes an ingrdient from the product.
        /// </summary>
        /// <param name="ingredient">Ingredients to remove.</param>
        /// <returns>Returns true if the ingredient was removed, else false.</returns>
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

        /// <summary>
        /// Checks if <paramref name="keyToAdd"/> exist in the ingredient dictionary. If it does, updates its value using <paramref name="valueToAdd"/>. It it was not found it, it will be added and <paramref name="OldKeyToRemove"/> will be removed.
        /// </summary>
        /// <param name="keyToAdd">The key to add.</param>
        /// <param name="valueToAdd">The value of the key.</param>
        /// <param name="OldKeyToRemove">The old key. If <paramref name="keyToAdd"/> is found it will become the new <paramref name="OldKeyToRemove"/>.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the dictionary that contains the ingredients and their values. 
        /// </summary>
        public Dictionary<string, float> GetIngredientsDictionary { get => ingredients; }

        /// <summary>
        /// Adds a single ingredient and its cost to the pizza, if it does not exist. If it exist, updates its value.
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

    /// <summary>
    /// Class for polymorting pizzas. Derived from <c>PizzaBase</c>
    /// </summary>
    public class PizzaPolymorth : PizzaBase
    {
        /// <summary>
        /// Constructor for pizzas that does not contain any ingredients by default.
        /// </summary>
        /// <param name="priceWrite">Textblock to write price too.</param>
        /// <param name="ingredientsWrite">Textblock to write ingredients too.</param>
        /// <param name="name">The name of the pizza.</param>
        /// <param name="number">The number of the pizza.</param>
        /// <param name="size">The size of the pizza.</param>
        /// <param name="basePrice">The base price of the pizza.</param>
        public PizzaPolymorth(TextBlock priceWrite, TextBlock ingredientsWrite, string name, uint number, /*Dictionary<string, float> ingredients,*/ string size = "Normal", float basePrice = 20) : base(priceWrite, ingredientsWrite)
        {
            this.name = name;
            this.number = number;
            sizeType = size;
            price = basePrice + CalculatePrice();
            DisplayIngredients();
        }

        /// <summary>
        /// Constructor for pizzas that does contain any ingredients by default.
        /// </summary>
        /// <param name="priceWrite">Textblock to write price too.</param>
        /// <param name="ingredientsWrite">Textblock to write ingredients too.</param>
        /// <param name="name">The name of the pizza.</param>
        /// <param name="number">The number of the pizza.</param>
        /// <param name="ingredients">Dictionary containing the ingredients, and their price, of the pizza.</param>
        /// <param name="size">The size of the pizza.</param>
        /// <param name="basePrice">The base price of the pizza.</param>
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


    //------------------------------------------------------------------------------


    /// <summary>
    /// DataList class, implements ObservableCollection<string>.
    /// </summary>
    public class DataList : ObservableCollection<string>
    { //https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol?view=netframework-4.8
        /// <summary>
        /// Constructor for the Datalist class.
        /// </summary>
        /// <param name="list">Strings to add to the class.</param>
        public DataList(string[] list)
        {
            foreach (string str in list)
                Add(str);
        }
        /// <summary>
        /// Gets and sets the data in the dataList class. 
        /// </summary>
        public string[] GetSetDataList { get; set; }
    }




}
