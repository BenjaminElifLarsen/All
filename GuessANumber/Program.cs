using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GuesssANumber
{

    public class Rnd
    {
        private static Random rnd = new Random();
        private Rnd(){ }

        public static sbyte GetRandom
        {
            get => (sbyte)rnd.Next(0, 20);
        }
    }


        class Program
    {
        static void Main(string[] args)
        {
            Interface infa = new Interface();
            infa.MainMenu();
        }
    }


    /// <summary>
    /// class that implement the gameplay part of the game
    /// </summary>
    public static class Gameplay
    {
        /// <summary>
        /// contain the gameplay loop. When the player loose, it will return their score. 
        /// </summary>
        /// <param name="amountOfTries">The amount of attemps they have for each guess</param>
        /// <returns></returns>
        public static uint gameplayLoop(byte amountOfTries)
        {
            byte currentAmountOfTries = 0;
            uint correctAmount = 0;
            sbyte toGuess = Rnd.GetRandom;

            do
            {
                currentAmountOfTries++;
                byte guess = GetNumber();
                if(guess == toGuess)
                {
                    correctAmount++;
                    currentAmountOfTries = 0;
                    toGuess = Rnd.GetRandom;
                    Console.WriteLine("Correct. \nTry guess the new number. \nEnter to continue.");
                    Console.ReadLine();
                    Interface.PartlyClear(0, 0, Console.WindowWidth, Console.WindowHeight);
                }
                else if (guess > toGuess)
                {
                    Console.WriteLine("To high");
                    Console.ReadLine();
                }else if (guess < toGuess)
                {
                    Console.WriteLine("To low");
                    Console.ReadLine();
                }


            } while (amountOfTries != currentAmountOfTries);

            return 0;
        }


        private static byte GetNumber()
        {
            byte? numberNull;
            byte number;
            string writeOut = "Please enter a number and press enter: ";
            Console.Write("Please enter a number and press enter: ");
            bool isNumber;
            do
            {
                Interface.PartlyClear(0, 0, Console.WindowWidth, Console.WindowHeight);
                Console.WriteLine(writeOut);
                isNumber = IsNumber(Console.ReadLine(), out numberNull);
                if (!isNumber)
                    writeOut = "Not a number. Please Reenter";
            } while (!isNumber);
            number = (byte)numberNull;
            return number;
        }

        /// <summary>
        /// checks if a string can be parsed. 
        /// Returns true if it can, false otherwise. Will also return the result. 
        /// </summary>
        /// <param name="toBeNumber"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool IsNumber(string toBeNumber, out byte? result)
        {
            byte result2;
            result = 0;
            bool isNumber =  byte.TryParse(toBeNumber, out result2) ? true : false;
            if (isNumber)
            {
                result = result2;
                return true;
            }
            else
                return false;

        }


    }

    /// <summary>
    /// class that implement the interface of the game.
    /// </summary>
    public class Interface
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);


        private byte[] offset;
        private byte[] startLocation;
        private byte[] hoverColour;
        private byte[] otherColour;
        private byte totalGuessAmount;

        public Interface()
        {
            offset = new byte[] {2,2 };
            startLocation = new byte[] {4,2 };
            hoverColour = new byte[] { 255, 0, 0 };
            otherColour = new byte[] { 0, 255, 12 };
            totalGuessAmount = 3; //is not really related to the interface, move it at some point
            Setup();
        }

        /// <summary>
        /// sets up the console for further use. 
        /// </summary>
        public void Setup()
        {
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
            Console.SetWindowSize(40, 20);
            //Console.SetCursorPosition(i + offset[0], k + offset[1]);
            //Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m|" + "\x1b[0m");

        }

        /// <summary>
        /// The main menu
        /// </summary>
        public void MainMenu()
        {
            string[] options =
            {
                "New Game",
                "High Score",
                "Shurtdown"
            };

            DrawAndSelect("Select Option", options, 30, 15, startLocation, otherColour, hoverColour, out string answer, out uint _, offset[0], offset[1]);
            switch (answer)
            {
                case "New Game":
                    Gameplay.gameplayLoop(totalGuessAmount);
                    break;

                case "High Score":

                    break;

                case "Shurtdown":
                    Environment.Exit(0);
                    break;
            }
        }



        /// <summary>
        /// Clears everything between <paramref name="xStart"/> to <paramref name="xEnd"/> and <paramref name="yStart"/> to <paramref name="yEnd"/>.
        /// If <paramref name="optionalCursorX"/> and/or <paramref name="optionalCursorY"/> are not null, their values will be used to set the cursor location
        /// after clearing, else <paramref name="xStart"/> and/or <paramref name="yStart"/> are used.
        /// The function is unable to clear more columns than the width of the console window.
        /// </summary>
        /// <param name="xStart">First column to start on.</param>
        /// <param name="yStart">First row to start on.</param>
        /// <param name="xEnd">Last column to end on.</param>
        /// <param name="yEnd">Last row to end on.</param>
        /// <param name="optionalCursorX">An optional placement for the curser, left, after clearing.</param>
        /// <param name="optionalCursorY">An optional placement for the curser, top, after clearing.</param>
        public static void PartlyClear(int xStart, int yStart, int xEnd, int yEnd, int? optionalCursorX = null, int? optionalCursorY = null)
        { //When it jumps to the next line it will erase the first sign
            xEnd = xEnd == Console.WindowWidth ? xEnd - 1 : xEnd;
            int x = optionalCursorX != null ? (int)optionalCursorX : xStart;
            int y = optionalCursorY != null ? (int)optionalCursorY : yStart;
            if (!(xStart >= xEnd))
                for (int i = yStart; i <= yEnd; i++)
                {
                    Console.SetCursorPosition(xStart, i);
                    Console.Write("".PadLeft(xEnd - xStart + 1)); //plus one because of the first index is 0. E.g. xEnd = 80 and xStart is 0, it goes from 0-79 without the + 1
                    //however, if xEnd is the width, e.g. width = 80, columns go from 0 - 79 + 1, it will go down to the next line and remove the  first column there
                }
            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// 
        /// If there are more options that what it can display at ones, it will allow for multiple pages of options. 
        /// The <paramref name="startLocation"/> of x should be at least 3 for smooth display of the pages.
        /// If <paramref name="options"/> is empty, <paramref name="selectedOption"/> returns "No options." and <paramref name="optionSelected"/> returns the max value of uint.
        /// </summary>
        /// <param name="controlText">Explanational text, can e.g. be how to find and select an option.</param>
        /// <param name="options">The options to print out.</param>
        /// <param name="writtingWidth">How far on the x axi text can be printed too.</param>
        /// <param name="maxYLine">The max amount of lines that it can print on, is affected by <paramref name="rowOffset"/>.</param>
        /// <param name="startLocation">The start locations, x and y, of the text. The text can never go more left than what the first entry state.</param>
        /// <param name="colourOfOptions">The RGB colours of nonhovered over options.</param>
        /// <param name="colourOfHoveredOption">The RGB colours of the hovered over option.</param>
        /// <param name="selectedOption">The selected string.</param>
        /// <param name="optionSelected">The index of the selected string. </param>
        /// <param name="collOffset">The amount of collumns between each option.</param>
        /// <param name="rowOffset">The amount of rows between each option.</param>
        private void DrawAndSelect(string controlText, string[] options, uint writtingWidth, uint maxYLine, byte[] startLocation, byte[] colourOfOptions, byte[] colourOfHoveredOption, out string selectedOption, out uint optionSelected, byte collOffset = 2, byte rowOffset = 0)
        {
            byte longestestOption = 0;
            byte xLocation;
            byte yLocation;
            byte cursorTrackerX = 0;
            byte cursorTrackterY = 0;
            byte currentAmountOfColls = 0; //rename
            byte maxAmountOfRows = 0;
            List<byte> amountofRowsPerColls;
            List<byte> amountOfCollsPerRow;
            bool run = true;
            selectedOption = "";
            optionSelected = 0;

            Console.WriteLine(controlText);
            Console.CursorVisible = false;
            Console.CursorLeft = startLocation[0];
            Console.CursorTop = startLocation[1];

            if (options.Length == 0 && options == null)
            {
                Debug.WriteLine("DrawAndSelect's 'options' were passed as either null or empty.");
            }
            else
            {
                foreach (string option in options)
                    if (option.Length > longestestOption)
                        longestestOption = (byte)(option.Length);
                longestestOption += collOffset;

                uint amountOfOptionsOnLine = (uint)Math.Floor((double)writtingWidth / (double)(longestestOption + collOffset));
                uint amountOfYLines = (uint)Math.Floor((double)maxYLine / (double)(rowOffset + 1));
                uint amountOfOptionsShowned = amountOfYLines * amountOfOptionsOnLine;
                uint amountOFScolls = (uint)Math.Ceiling((double)options.Length / (double)amountOfOptionsShowned) - 1;
                uint scrollLocation = 0;
                uint tempScrollLocation = 1;
                int LastScrollPosistionDownX = 0;
                int LastScrollPosistionDownY = 0;
                byte maxCursorTrackterY = 0;

                if (options.Length == 0)
                {
                    run = false;
                    selectedOption = "No options.";
                    optionSelected = uint.MaxValue;
                }

                while (run)
                {
                    currentAmountOfColls = 0;
                    if (tempScrollLocation != scrollLocation && amountOFScolls > 0)
                    {
                        int x = Console.CursorLeft;
                        int y = Console.CursorTop;
                        tempScrollLocation = scrollLocation;
                        PartlyClear(0, 1, Console.WindowWidth, Console.WindowHeight); 

                        Console.SetCursorPosition(1, 2);
                        Console.Write("P");
                        Console.SetCursorPosition(1, 3);
                        Console.Write("a");
                        Console.SetCursorPosition(1, 4);
                        Console.Write("g");
                        Console.SetCursorPosition(1, 5);
                        Console.Write("e");
                        Console.SetCursorPosition(1, 7);
                        Console.Write(scrollLocation + 1);
                        Console.SetCursorPosition(1, 8);
                        Console.Write("/");
                        Console.SetCursorPosition(1, 9);
                        Console.Write(amountOFScolls + 1); //can write into the option location

                        Console.SetCursorPosition(1, 12);
                        Console.Write("|");

                        if (scrollLocation != 0)
                        {
                            Console.SetCursorPosition(1, 11);
                            Console.Write("^");
                        }
                        if (scrollLocation < amountOFScolls)
                        {
                            Console.SetCursorPosition(1, 13);
                            Console.Write("v");
                        }

                        Console.CursorLeft = x;
                        Console.CursorTop = y;
                    }
                    amountOfCollsPerRow = new List<byte>();
                    amountofRowsPerColls = new List<byte>();
                    xLocation = (byte)Console.CursorLeft;
                    yLocation = (byte)Console.CursorTop;
                    byte rowNumber = 0;
                    bool direction = false;
                    byte m = 0;
                    uint index = 0 + (amountOfOptionsShowned * scrollLocation);
                    while (index < options.Length && amountOfCollsPerRow.Count + 1 <= amountOfYLines)
                    {
                        string write = options[index];
                        Console.CursorLeft = longestestOption * m + startLocation[0];
                        Console.CursorTop = startLocation[1] + rowNumber;
                        if (xLocation == Console.CursorLeft && Console.CursorTop == yLocation)
                            Console.Write("\x1b[38;2;" + colourOfHoveredOption[0] + ";" + colourOfHoveredOption[1] + ";" + colourOfHoveredOption[2] + "m" + write + "\x1b[0m");
                        else
                            Console.Write("\x1b[38;2;" + colourOfOptions[0] + ";" + colourOfOptions[1] + ";" + colourOfOptions[2] + "m" + write + "\x1b[0m");
                        m++;

                        if (longestestOption * (m + 1) + startLocation[0] >= writtingWidth)
                        {
                            amountOfCollsPerRow.Add(m);
                            rowNumber += (byte)(1 + rowOffset);
                            currentAmountOfColls = m > currentAmountOfColls ? m : currentAmountOfColls;
                            maxAmountOfRows = maxAmountOfRows < currentAmountOfColls ? currentAmountOfColls : maxAmountOfRows;
                            m = 0;
                        }
                        index++;
                    }
                    if (rowNumber == 0 && m <= amountOfOptionsOnLine)
                    {
                        amountofRowsPerColls.Add(1);
                        for (int i = 0; i < amountofRowsPerColls[0]; i++)
                            amountOfCollsPerRow.Add(m);
                    }

                    if (currentAmountOfColls != 0)
                    {
                        if (m != 0)
                            amountOfCollsPerRow.Add(m);
                        for (int k = 0; k < currentAmountOfColls; k++)
                            amountofRowsPerColls.Add(0);
                        for (int i = 0; i < amountOfCollsPerRow.Count; i++)
                        {
                            for (int n = 0; n < amountOfCollsPerRow[i]; n++)
                            {
                                amountofRowsPerColls[n]++;
                            }
                        }
                    }

                    Console.SetCursorPosition(xLocation, yLocation);
                    do
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true); //for some reason, pressing down adds 1 to cursorLeft, just like pressing up did
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if (cursorTrackterY > 0)
                            {
                                cursorTrackterY--;
                                Console.SetCursorPosition(xLocation, Console.CursorTop - rowOffset - 1);
                            }
                            else if (cursorTrackterY == 0 && scrollLocation > 0)
                            {
                                tempScrollLocation = scrollLocation;
                                scrollLocation--;
                                Console.SetCursorPosition(Console.CursorLeft, LastScrollPosistionDownY);
                                cursorTrackterY = maxCursorTrackterY;
                            }

                            if (Console.CursorTop == startLocation[1])
                                Console.CursorLeft = xLocation;
                            direction = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            direction = true;

                            if (cursorTrackterY == amountOfCollsPerRow.Count - 1 && (scrollLocation == amountOFScolls)) //latest addetion to fix a bug with pressing down and cursorTrackerX was equal or bigger than the amount of entires in amountofRowsPerColls
                            {

                            }
                            else if (cursorTrackterY == amountofRowsPerColls[cursorTrackerX] - 1 && scrollLocation < amountOFScolls)
                            {
                                tempScrollLocation = scrollLocation;
                                scrollLocation++;
                                uint left = (uint)options.Length - amountOfOptionsShowned * scrollLocation;
                                while (cursorTrackerX >= left)
                                    cursorTrackerX--;
                                LastScrollPosistionDownY = Console.CursorTop;
                                LastScrollPosistionDownX = Console.CursorLeft = longestestOption * cursorTrackerX + startLocation[0];
                                Console.SetCursorPosition(LastScrollPosistionDownX, startLocation[1]);
                                maxCursorTrackterY = cursorTrackterY;
                                cursorTrackterY = 0;
                                currentAmountOfColls = 0;
                            }
                            else if (cursorTrackterY < amountofRowsPerColls[cursorTrackerX] - 1)
                            {
                                Console.SetCursorPosition(xLocation, Console.CursorTop + rowOffset + 1);
                                cursorTrackterY++;
                            }
                            else
                                Console.CursorLeft = xLocation;
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if (cursorTrackerX != 0)
                                cursorTrackerX--;
                            Console.CursorLeft = longestestOption * cursorTrackerX + startLocation[0];

                            direction = true;
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if (cursorTrackerX < amountOfCollsPerRow[cursorTrackterY] - 1)
                                cursorTrackerX++;
                            Console.CursorLeft = longestestOption * cursorTrackerX + startLocation[0];

                            direction = true;
                        }

                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            uint totalPassed = 0;
                            for (int i = 0; i < cursorTrackterY; i++)
                                totalPassed += amountOfCollsPerRow[i];
                            totalPassed += amountOfOptionsShowned * scrollLocation;
                            optionSelected = (uint)(cursorTrackerX + totalPassed);
                            selectedOption = options[optionSelected];
                            run = false;
                            direction = true;
                        }

                    } while (!direction);
                }
            }
        }

    }

    

}
