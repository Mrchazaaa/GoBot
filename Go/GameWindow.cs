using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Go
{
    //Gamestate list is used as an index lookup for the "history" gamestates list

    public partial class GameWindow : Form
    {
        public static List<BoardPiece> tiles = new List<BoardPiece>();
        public static char currentTeam; //Stores a character representing the team that is currently playing #Shouldnt be necesary due to history list
        public double computerThinkingTimeLimit;
        public static char[] currentBoard; //Shouldnt be necesary due to history list
        public int boardWidth; //Stores the width and height of the square board
        public static int capturedWhite; //Shouldnt be necesary due to history list
        public static int capturedBlack; //Shouldnt be necesary due to history list
        public static bool blackHasPassed = false; //Used to keep track of whether the player has passed 
        public static bool whiteHasPassed = false; 
        public static int lastBlackMove;
        public static List<int> emptyIndexes;
        public bool noComputer;
        public bool onePlayerPassed;
        public bool sentFromPass;

        EventHandler[] clicks; //A list of event handlers are created, for the three actions assigned to each board piece
        EventHandler[] enters; //(Click, Enter and leave), so that the events can be removed later when the game ends.
        EventHandler[] leaves;

        //Lists of eventhandlers are set to 
        //contain as many events as there are
        //positions on the board

        public GameWindow()
        {
            InitializeComponent();
            onePlayerPassed = false;
            sentFromPass = false;
        }

        public void starter()
        {
            emptyIndexes = new List<int>();

            lastBlackMove = -1;

            AI.parentWindow = this;

            Rules.boardWidth = this.boardWidth;

            clicks = new EventHandler[boardWidth * boardWidth];
            enters = new EventHandler[boardWidth * boardWidth];
            leaves = new EventHandler[boardWidth * boardWidth];

            capturedBlack = 0;
            capturedWhite = 0;

            currentBoard = new char[boardWidth * boardWidth];

            currentTeam = '\0';
            gameStateList.Items.Add(new gameStates('\0'));

            //Board piece classes are added to the board and their inital pictures and addresses are set
            for (int y = 0; y < boardWidth; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    int count = (y * boardWidth) + x;

                    emptyIndexes.Add(count);

                    string pos = Convert.ToString(x) + Convert.ToString(y);
                    BoardPiece bp = new BoardPiece(count, this);
                    if (x == 0)
                    {
                        if (y == 0) { bp.empty = new Bitmap("Resource/TL.png"); }
                        else if (y == boardWidth - 1) { bp.empty = new Bitmap("Resource/BL.png"); }
                        else { bp.empty = new Bitmap("Resource/L.png"); }
                    }
                    else if (x == boardWidth - 1)
                    {
                        if (y == 0) { bp.empty = new Bitmap("Resource/TR.png"); }
                        else if (y == boardWidth - 1) { bp.empty = new Bitmap("Resource/BR.png"); }
                        else { bp.empty = new Bitmap("Resource/R.png"); }
                    }
                    else if (y == 0) { bp.empty = new Bitmap("Resource/T.png"); }
                    else if (y == boardWidth - 1) { bp.empty = new Bitmap("Resource/B.png"); }
                    else { bp.empty = new Bitmap("Resource/C.png"); }

                    bp.Image = bp.empty;
                    panel1.Controls.Add(bp);
                    tiles.Add(bp);

                    clicks[bp.address] = (EventHandler)delegate { bp.PlacePiece(); }; //Click, enter and leave events are created
                    enters[bp.address] = (EventHandler)delegate { bp.BoardPiece_EnterPiece(); };
                    leaves[bp.address] = (EventHandler)delegate { bp.BoardPiece_MouseLeave(); };

                    bp.Click += clicks[bp.address]; //Click, enter and leave events are set to board pieces
                    bp.MouseEnter += enters[bp.address];
                    bp.MouseLeave += leaves[bp.address];
                }
            }
            nextTurn();
        }

        private void formatBoard(object sender, EventArgs e) //Called when form size changes to keep the board as a constant square
        {
            int pieceHeight = (int)Math.Ceiling((double)((panel1.Height < panel1.Width ? panel1.Height : panel1.Width - 65)) / boardWidth);
            Point startingPoint = panel1.Height < panel1.Width ? new Point((panel1.Width - (boardWidth * pieceHeight)) / 2, 0) : new Point((panel1.Width - (boardWidth * pieceHeight)) / 2, 0);

            for (int y = 0; y < boardWidth; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    tiles[(y * boardWidth) + x].Height = pieceHeight;
                    tiles[(y * boardWidth) + x].Width = pieceHeight;
                    tiles[(y * boardWidth) + x].Location = new Point(startingPoint.X + (pieceHeight * x) - x, startingPoint.Y + (pieceHeight * y) - y) ;
                    tiles[(y * boardWidth) + x].BringToFront();
                }
            }
        }

        public void whitePasses() //Called when white passes, checks if game ends
        {
            MessageBox.Show("White team passes");

            if (blackHasPassed)
            {
                foreach (BoardPiece bp in tiles)
                {
                    bp.Click -= clicks[bp.address];
                    bp.MouseEnter -= enters[bp.address];
                    bp.MouseLeave -= leaves[bp.address];
                    button1.Enabled = false;
                    gameStateList.Enabled = false;
                }
                char[] boardRep = new char[boardWidth * boardWidth];
                for (int u = 0; u < tiles.Count; u++)
                {
                    boardRep[u] = tiles[u].team;
                }

                Tuple<int, int> scores = Rules.countScore(boardRep);
                MessageBox.Show("Game End:\nBlack Team: "
                    + Convert.ToString(scores.Item1 - capturedBlack)
                    + " (Captured Black: "
                    + Convert.ToString(capturedBlack)
                    + ")" + "\nWhite Team : "
                    + Convert.ToString(scores.Item2 - capturedWhite)
                    + " (Captured White: "
                    + Convert.ToString(capturedWhite) + ")");
            }
            else
            {
                nextTurn();
            }
        }

        private void blackPlayerPass(object sender, EventArgs e) //Player (I.E: Black team) clicks pass button
        {
            bool endGame = false;
            sentFromPass = true;

            if (!noComputer)
            {
                blackHasPassed = true;

                MessageBox.Show("Black passes");

                AI.passedNodes = null;
            }
            else
            {
                if (currentTeam == 'B')
                {
                    MessageBox.Show("Black passes");
                }
                else
                {
                    MessageBox.Show("White passes");
                }
                if (onePlayerPassed == true)
                {
                    endGame = true;
                }
                else
                {
                    onePlayerPassed = true;
                }
            }

            if (whiteHasPassed || endGame)
            {
                foreach (BoardPiece bp in tiles)
                {
                    bp.Click -= clicks[bp.address];
                    bp.MouseEnter -= enters[bp.address];
                    bp.MouseLeave -= leaves[bp.address];
                    button1.Enabled = false;
                    gameStateList.Enabled = false;
                }
                char[] boardRep = new char[boardWidth * boardWidth];
                for (int u = 0; u < tiles.Count; u++)
                {
                    boardRep[u] = tiles[u].team;
                }

                Tuple<int, int> scores = Rules.countScore(boardRep);
                MessageBox.Show("Game End:\nBlack Team: "
                    + Convert.ToString(scores.Item1 - capturedBlack)
                    + " (Black territory: "
                    + Convert.ToString(scores.Item1)
                    + ", Captured Black: "
                    + Convert.ToString(capturedBlack)
                    + ")" 
                    + "\nWhite Team : "
                    + Convert.ToString(scores.Item2 - capturedWhite)
                    + " (White territory: "
                    + Convert.ToString(scores.Item2)
                    + ", Captured White: "
                    + Convert.ToString(capturedWhite)
                    + ")"
                    );
            }
            else
            {
                nextTurn();
            }
        }

        private void gameStateList_MouseDoubleClick(object sender, MouseEventArgs e) //If the user wants to go to history of game state
        {
            int index = gameStateList.IndexFromPoint(e.Location);
            if (index != -1) //Index is -1 if the control is clicked but not on an item
            {
                gameStates currentGameState = (gameStates)gameStateList.Items[index];
                capturedBlack = currentGameState.capturedBlack;
                capturedWhite = currentGameState.capturedWhite;
                currentBoard = new char[boardWidth * boardWidth];
                Array.Copy(currentGameState.boardRep, currentBoard, boardWidth*boardWidth);

                AI.passedNodes = null;
                tree.moveNodeAddress = new Dictionary<int, List<int>>();

                currentTeam = currentGameState.currentTeam == 'W' ? 'B' : 'W';
                for (int i = gameStateList.Items.Count - 1; i > index; i--)
                {
                    gameStateList.Items.RemoveAt(i);
                }
                for (int z = 0; z < boardWidth * boardWidth; z++)
                {
                    if (currentBoard[z] != tiles[z].team)
                    {
                        tiles[z].removePiece();

                        if (currentBoard[z] == 'W')
                        {
                            tiles[z].dropStone('W');
                        }
                        else if (currentBoard[z] == 'B')
                        {
                            tiles[z].dropStone('B');
                        }
                    }
                }
                nextTurn();
            }
        }

        public class gameStates
        {
            public int capturedWhite;
            public int capturedBlack;
            public char currentTeam; //I.E: The last team that has moved resulting in this gamestate
            public char[] boardRep;

            public gameStates(char CurrentTeam)
            {
                capturedWhite = GameWindow.capturedWhite;
                capturedBlack = GameWindow.capturedBlack;
                currentTeam = CurrentTeam;
                boardRep = new char[Rules.boardWidth*Rules.boardWidth];
                Array.Copy(GameWindow.currentBoard, boardRep, GameWindow.currentBoard.Length);
            }

            public override string ToString()
            {
                return Convert.ToString(currentTeam);
            }
        }

        public void nextTurn()
        {
            if (!sentFromPass)
            {
                onePlayerPassed = false;
            }

            sentFromPass = false;

            if (currentTeam == 'B')//If the player that has just moved is black team
            {
                currentTeam = 'W';

                if (!noComputer)
                {
                    AI.makeMove();

                    gameStateList.Items.Add(new gameStates(currentTeam));
                }
            }
            else if (currentTeam == 'W' || currentTeam == '\0')//If the player that has just moved is black team
            {
                currentTeam = 'B';
            }
        }
    }
}
