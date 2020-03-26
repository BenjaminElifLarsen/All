﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chess
{

    public class MapMatrix
    {
        private MapMatrix() { }
        public static sbyte[,] map = new sbyte[8, 8];
    }

    public class ChessList
    {
        private ChessList() { }
        private List<ChessPiece> chessListBlack = new List<ChessPiece>();
        private List<ChessPiece> chessListWhite = new List<ChessPiece>();
        public void SetChessListBlack(List<ChessPiece> list)
        {
            chessListBlack = list;
        }
        public void SetChessListWhite(List<ChessPiece> list)
        {
            chessListWhite = list;
        }
        public List<ChessPiece> GetList(bool team)
        {
            return team == true ? chessListBlack : chessListWhite;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ChessTable chess = new ChessTable();

            Console.ReadLine();
        }
    }

    class ChessTable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        private Player white; //white top
        private Player black; //black bottom
        private uint[,] whiteSpawnLocation; //since the map matrix is being used, no real reason for having these outside their player class
        private uint[,] blackSpawnLocation;
        private byte squareSize;
        private byte[] lineColour;
        private byte[] lineColourBase;
        private byte[] squareColour1;
        private byte[] squareColour2;
        private byte[] offset;
        private byte[] windowsSize = new byte[2];


        public ChessTable()
        {
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);

            squareSize = 5;
            lineColour = new byte[] { 122, 122, 122 };
            lineColourBase = new byte[] { 87, 65, 47 };
            squareColour1 = new byte[] { 182, 123, 91 };
            squareColour2 = new byte[] { 135, 68, 31 };
            offset = new byte[] { 2, 2 };

            windowsSize[0] = (byte)(9 + 8 * squareSize + 10);
            windowsSize[1] = (byte)(9 + 8 * squareSize + 10);
            Console.SetWindowSize(windowsSize[0], windowsSize[1]);
            BoardSetup();
        }


        private void BoardSetup()
        {//8 squares in each direction. Each piece is 3*3 currently, each square is 5*5 currently. 
            Console.CursorVisible = false;
            ushort distance = (ushort)(9 + 8 * squareSize);
            for (int k = 0; k < distance; k++)
                for (int i = 0; i < distance; i += 1 + squareSize)
                {
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m|" + "\x1b[0m");
                }
            for (int k = 0; k < distance; k += 1 + squareSize)
                for (int i = 1; i < distance - 1; i++)
                {
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[48;2;" + lineColourBase[0] + ";" + lineColourBase[1] + ";" + lineColourBase[2] + "m ");
                    Console.SetCursorPosition(i + offset[0], k + offset[1]);
                    Console.Write("\x1b[38;2;" + lineColour[0] + ";" + lineColour[1] + ";" + lineColour[2] + "m-" + "\x1b[0m");
                }
            BoardColouring();

        }

        private void BoardColouring()
        {
            ushort distance = (ushort)(8 + 8 * squareSize);
            byte location = 1;
            for (int n = 0; n < distance; n += 1 + squareSize)
            {
                for (int m = 0; m < distance; m += 1 + squareSize)
                {
                    for (int i = 0; i < squareSize; i++)
                    {
                        for (int k = 0; k < squareSize; k++)
                        {
                            Console.SetCursorPosition(i + offset[0] + 1 + n, k + offset[1] + 1 + m);
                            if (location % 2 == 0)
                                Console.Write("\x1b[48;2;" + squareColour1[0] + ";" + squareColour1[1] + ";" + squareColour1[2] + "m " + "\x1b[0m");
                            else if (location % 2 == 1)
                                Console.Write("\x1b[48;2;" + squareColour2[0] + ";" + squareColour2[1] + ";" + squareColour2[2] + "m " + "\x1b[0m");
                        }
                    }
                    location++;
                }
                location++;
            }

        }

        private void PlayerSetup()
        {
            byte[] colourWhite =
            {
                255,255,255
            };
            white = new Player(colourWhite, true, whiteSpawnLocation);
            byte[] colourBlack =
            {
                0,0,0
            };
            black = new Player(colourBlack, false, blackSpawnLocation);
        }

    }

    class Player
    { //this class is set to be an abstract in the UML, but is that really needed? 
        private byte[] colour;
        private bool currentTurn; //rename to either black or white so it makes more sense 
        private List<ChessPiece> chessPieces = new List<ChessPiece>();
        private uint[,] spawnLocations; //start with the pawns, left to right and then the rest, left to right
        private sbyte directionMultiplier;

        public Player(byte[] colour, bool startTurn, uint[,] spawnLocations)
        {
            this.colour = colour;
            this.currentTurn = startTurn;
            this.spawnLocations = spawnLocations;
            directionMultiplier = startTurn ? (sbyte)1 : (sbyte)-1;
            CreatePieces();
        }

        public void Control()
        {
            HoverOver();
            MovePiece();
        }

        private void HoverOver()
        {
            //go through the list, e.g. if player press left arror it goes - 1 and if at 0 it goes to 7? Or should the code let the player move through the board and hightlight the square they are standing on and if there 
            //is a chess piece in their control its gets highlighted instead of and they can select it? 

            SelectPiece();
        }

        private void SelectPiece()
        {

        }

        private void MovePiece()
        {

        }

        private void CreatePieces()
        {
            string[] pawnDesign =
            {
                " - ",
                " | ",
                "-P-"
            };
            string[] rockDesign =
            {
                "^^^",
                "|=|",
                "-R-"
            };

            string[] knightDesign =
            {
                " ^_",
                " |>",
                "-k-"
            };

            string[] bishopDesign =
            {
                "_+_",
                "|O|",
                "-B-"
            };

            string[] queenDesign =
            {
                "_w_",
                "~|~",
                "-Q-"
            };

            string[] kingDesign =
            {
                "^V^",
                "*|*",
                "-K-"
            };

            uint[][] pawnMove =
            { //first move, it got the possiblity of moving 1 or 2 //the pawn class should check if it, that is a specific pawn, have moved or not. If not it should give the option of moving 2 squares forward
                new uint[] {1 }
            };

            uint[][] rockMove =
            { //it can move 1 to 7 squares in each direciton. With current design decision it will contain 7*4 arrays, a little to much
                //consider another way to do these moves. Also since the queen have many more moves, 7*8 moves
                //maybe other than just the 1-4 values used, use 5 for unlimited move in diagonal directions and 6 for unlimited non-diagnonal directions, where the code should calculate the max amount of distance the 
                //piece can move in each direction, i.e. when the piece hits a wall or another piece. 
                    //This just leave how to select the different direction, but then again, it same should be done for the "normal" 1-4 values. 
                    //Of course, the maximum move distance and move selection should be done over in the specific chesspiece. 
                        //change done to the base class, no need for these Move variables, also the design strings. 
            };

            string team;
            team = currentTurn == true ? "-" : "+";

            for (int i = 0; i < 8; i++)
            {//loop that creates each piece 
                string pawnID = String.Format("{0}6:{1}", team, i);
                //set other values for each piece and create them.
                //chessPieces.Add
            }

            for (int i = 0; i < 2; i++)
            { //loop that creates each piece 
                string rockID = String.Format("{0}5:{1}", team, i);
                string bishopID = String.Format("{0}3:{1}", team, i);
                string knightID = String.Format("{0}4:{1}", team, i);
                //set other values for each piece and create them.
                //chessPieces.Add
            }

            string queenID = String.Format("{0}2:{1}", team, 0);
            string kingID = String.Format("{0}1:{1}", team, 0);
        }

        public bool Turn(bool turn)
        {


            return currentTurn;
        }

    }

    sealed class King : ChessPiece
    { //this one really need to keep an eye on all other pieces and their location
        public King(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^V^",
                "*|*",
                "-K-"
            };
        }

        public bool IsInDanger()
        { //if true, it should force the player to move it. Also, it needs to check each time the other player has made a move 
            //should also check if it even can move, if it cannot the game should end

            return false;
        }

    }

    sealed class Queen : ChessPiece
    {
        public Queen(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) 
        {
            Design = new string[]
            {
                "_w_",
                "~|~",
                "-Q-"
            };
        }

    }

    sealed class Pawn : ChessPiece
    {
        private bool firstTurn = false;
        private bool canAttactLeft;
        private bool canAttackRight; 
        public Pawn(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                " - ",
                " | ",
                "-P-"
            };
        }

    }

    sealed class Rock : ChessPiece
    {
        public Rock(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID)
        {
            Design = new string[]
            {
                "^^^",
                "|=|",
                "-R-"
            };
        }

    }

    sealed class Bishop : ChessPiece
    {
        public Bishop(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_, team_, spawnLocation_, ID) 
        {
            Design = new string[]
            {
                "_+_",
                "|O|",
                "-B-"
            };
        }

    }

    sealed class Knight : ChessPiece
    {

        //Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : this(location_, colour_, design_, team_, spawnLocation_, ID) { }

        public Knight(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] spawnLocation_, string ID) : base(location_, colour_, design_,team_,spawnLocation_,ID)
        { //maybe do not have the moves and attacks, design and suck as parameters, but rather part of the code, since you have changed from abstract to non abstract class
          //redo the constructors when you are sure what you will need. So far: spawnlocation, id and team
            Design = new string[]
            {
                " ^_",
                " |>",
                "-k-"
            };



        }




    }

    abstract public class ChessPiece //still got a lot to read and learn about what is the best choice for a base class, class is abstract, everything is abstract, nothing is abstract and so on. 
    {//when put on a location, check if there is an allie, if there is invalid move, if enemy, call that pieces removeDraw and call their Taken using TakeEnemyPiece
        protected uint[] location; //x,y
        protected byte[] colour; // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/inheritance 
        protected string[] design;
        protected byte[][] movePattern;
        protected byte[][] attack; //in almost everycase it is the same as movePattern, so it can be set to null. If it is not null, i.e. pawn, it should be called when moving if there are enemies at the right location
        protected bool team;
        protected uint[] mapLocation;
        protected string id;
        protected bool canDoMove;
        protected bool hasBeenTaken = false;
        protected byte squareSize; 

        public ChessPiece(uint[] location_, byte[] colour_, string[] design_, bool team_, uint[] mapLocation_, string ID)
        { //for testing the code, just create a single player and a single piece. 
            Location = location; //location should be the console x,y values, but instead of being given, it should be calculated out from the maplocation and square size
            Colour = colour;
            Design = design_;
            SetTeam(team_);
            MapLocation = mapLocation_; //what should this actually be done, is it the actually values on the console or is it values that fits the map matrix and location is then the actually console location...
            this.ID = ID; //String.Format("{0}n:{1}", team, i); team = currentTurn == true ? "-" : "+"; n being the chesspiece type
            Draw();
        }

        /// <summary>
        /// Returns a bool that indicate if this piece has been "taken" by another player's piece. 
        /// </summary>
        public bool BeenTaken { get => hasBeenTaken; } //use by other code to see if the piece have been "taken" and should be removed from game. 


        protected bool SetBeenTaken { set => hasBeenTaken = value; }

        protected uint[] Location { get => location; set => location = value; } //consider for each of the properties what kind they should have

        protected byte[] Colour { set => colour = value; }

        protected string[] Design { get => design; set => design = value; }
        protected byte[][] MovePattern { set => movePattern = value; }

        protected byte[][] AttackPattern { get => attack; set => attack = value; } 

        protected bool Team { get => team; } //need to know it own team, but does it need to know the others's teams, the IDs can be used for that or just the matrix map. 

        protected uint[] MapLocation { set => mapLocation = value; }

        /// <summary>
        /// Gets and set the ID of the chesspiece. //maybe have some code that ensures the ID is unique 
        /// </summary>
        protected string ID { get => id; set => id = value; } //maybe split into two. Get being protected and the set being public 

        protected bool CanDoMove { get => canDoMove; set => canDoMove = value; } //what was the canDoMove suppose to be for again?

        /// <summary>
        /// Function that "controls" a piece. What to explain and how to..
        /// </summary>
        public virtual void Control()
        {
            Move();
            RemoveDraw(); //maybe move it into the Move function
            Draw();
            //UpdateMapMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Move()
        { 
            //calculate each possible, legal, end location. Maybe have, in the class scope, a variable byte[,] legalEndLocations that the DisplayPossibleMove can use. 
            DisplayPossibleMove(); //actually all of this is needed be done in the derived classes, since movement (and attacks) depends on the type of piece. 
            //how to best do this and DisplayPossibleMove()... 
            //how to know which location they have selected out of all the possible location?
            //before fully starting to implement the move and display, focus on just moving a single piece around to ensure the (remove)draw function work and the matrix map is updated and all of that. 
            //Then set up two pieces, one of each player, and see if the map and such are working correctly and if they got the same location if the correct one is removed. 

        }

        /// <summary>
        /// updates the location that is used for displaying the chesspiece on the chessboard
        /// </summary>
        protected void LocationUpdate()
        { //should squareSize be given using the consturctor or via a class. If given using the constructor the player constructur needs it too and having a single parameter just to give it on since wierd.
            //also need the offset, since the map matrix does not contain the any offset. 
            //maybe have a class that contain all "settings" of the game, e.g. offsets, colours, symbols '-' and '|', and so on
        }

        /// <summary>
        /// Draws the chesspiece at its specific location
        /// </summary>
        protected void Draw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++) //why does all the inputs, length, count and so on use signed variable types... 
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("\x1b[48;2;" + colour[0] + ";" + colour[1] + ";" + colour[2] + "m{0}",design[i]); //be careful, this one is not ending with a "\x1b[0m".
            }
        }

        /// <summary>
        /// Displays a piece in another colour if it is hovered over. 
        /// </summary>
        /// <param name="hover">If true, the piece will be coloured in a different colour. If false, the piece will have its normal colour.</param>
        public void IsHoveredOn(bool hover) //when a player hovers over a piece all this code with true, if and when they move to another piece call this piece again but with false 
        { //consider allowing a custom colour or just inverse colour. 
            if (hover)
            {
                byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
                int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
                int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
                for (int i = 0; i < design[0].Length; i++)
                {
                    Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                    Console.Write("\x1b[48;2;" + 255 + ";" + 0 + ";" + 0 + "m{0}", design[i]); //this one is not ending with a "\x1b[0m".
                }
            }
            else
                Draw();
        }

        /// <summary>
        /// removes the visual identication of a chesspiece at its current location.
        /// </summary>
        protected void RemoveDraw()
        {
            byte[] designSize = new byte[] { (byte)Design[0].Length, (byte)Design.Length };
            int drawLocationX = (int)Location[0] + (int)(squareSize - designSize[0]) / 2; //consider a better way for this calculation, since if squareSize - designSize[n] does not equal an even number
            int drawLocationY = (int)Location[1] + (int)(squareSize - designSize[1]) / 2; //there will be lost of precision and the piece might be drawned at a slightly off location
            for (int i = 0; i < design[0].Length; i++) 
            {
                Console.SetCursorPosition(drawLocationX, drawLocationY + i);
                Console.Write("".PadRight(design[0].Length,' ')); 
            }
        }

        /// <summary>
        /// updates the map matrix with the new location of the chess piece and sets the old location to zero. 
        /// </summary>
        protected void UpdateMapMatrix(uint[] oldMapLocation)
        { //need to either give the array[,] or have a class that it can acess it from. Since it is an array, an update in one instance will update the array in all instances. 
            string[] splittedID = ID.Split(':');
            sbyte piece = sbyte.Parse(splittedID[0]);
            MapMatrix.map[mapLocation[0], mapLocation[1]] = piece;
            MapMatrix.map[oldMapLocation[0], oldMapLocation[1]] = 0;
        }


        /// <summary>
        /// Set a chesspeice set to be taken so it can be removed. 
        /// </summary>
        public void Taken()
        {//call by another piece, the one that takes this piece. 
            hasBeenTaken = true;
            //it should remove itself from the map matrix. 
            RemoveDraw(); //if the piece is taken, the other piece stands on this ones location, so removeDraw might remove the other piece. Consider how to implement the Taken/Move regarding that. 
        }

        /// <summary>
        /// Sets the team of the chesspiece.
        /// </summary>
        /// <param name="team_">True for .... False for ....</param>
        protected void SetTeam(bool team_)
        {
            team = team_;
        }

        /// <summary>
        /// Display the possible, legal, moves a chesspiece can take. 
        /// </summary>
        protected void DisplayPossibleMove()
        {
            //needs to draw at every end location
            //what should be drawn, where should it and how to restore back to the default design and colour
        }

        protected void TakeEnemyPiece()
        {
            //how to find and get the enemy piece. The lists so far only exist in the players. Maybe have a class that only contains the two lists and when called a method to get a list, 
            //it use a conditional operator to return the list of the other team. 
            //the lists cannot be inheritance, since the piece will need to the other team's list and the player only contains their team's list.   
            //got the list now. Now to figure out how to find a specific piece in that list...
            //1) can look at the map location of each piece and see if one fits the new location of this piece
            //2) look at the map, see what value is at the location and cheeck the the number part of the ID... actually maybe instead of using a sbyte for the map, consider use a string
                //so you can write the entire ID... why did I not think of that before... then you can just look for the whole ID 
        }

    }


}