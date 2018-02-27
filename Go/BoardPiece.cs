using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Go
{
    public class BoardPiece : PictureBox
    {
        public Bitmap empty, placed; //Stores the background of the board and a placed piece on the board (in case a piece is captured)
        public int address;
        public char team;
        public GameWindow parentWindow;

        public BoardPiece(int Count, GameWindow parent)
        {
            this.parentWindow = parent;
            this.address = Count;
            this.Margin = new Padding(0);
            this.Padding = new Padding(0);
            this.Image = empty;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void BoardPiece_MouseLeave()
        {
            this.Image = team == 'W' || team == 'B' ? this.placed : this.empty ; //If a piece is placed here, it continues to be shown after the mouse has left
        }

        public void BoardPiece_EnterPiece()
        {
            if (team == '\0') //If not piece has been placed here, show the piece that the player hovers over
            {
                Bitmap hoveredPiece = new Bitmap(empty);
                Graphics gr = Graphics.FromImage(hoveredPiece);
                gr.DrawImage(Image.FromFile("Resource/" + GameWindow.currentTeam + "TH.png"), 0, 0, empty.Width, empty.Height);
                this.Image = hoveredPiece;
            }
        }

        public void PlacePiece()
        {
            char teamToPlace = GameWindow.currentTeam;
            if (this.team == 'W' || this.team == 'B') //If a piece is already placed here
            {
                MessageBox.Show("That position has already been taken");
            }
            else
            {
                char[] boardRep = new char[Rules.boardWidth * Rules.boardWidth];

                Array.Copy(GameWindow.currentBoard, boardRep, boardRep.Length);

                HashSet<int> removed = Rules.placePiece(address, ref boardRep, GameWindow.currentTeam);
                if (boardRep[address] != GameWindow.currentTeam)
                {
                    MessageBox.Show("You cannot commit your own pieces to suicide");
                }
                else //If placing a piece here is not condemming that piece to capture
                {
                    foreach (int i in removed)
                    {
                        GameWindow.tiles[i].removePiece();
                    }

                    if (team == 'W') //iF white is playing
                    {
                        GameWindow.capturedBlack += removed.Count();
                    }
                    else
                    {
                        GameWindow.capturedWhite += removed.Count();
                    }

                    Array.Copy(boardRep, GameWindow.currentBoard, Rules.boardWidth * Rules.boardWidth);

                    foreach (int j in removed)
                    {
                        GameWindow.emptyIndexes.Add(j);
                    }
                    GameWindow.emptyIndexes.Remove(address);

                    GameWindow.whiteHasPassed = false;
                    GameWindow.blackHasPassed = false;

                    dropStone(teamToPlace);
                    GameWindow.lastBlackMove = address; //Does not need to be checked for if the team is black as this will be set by black every time before AI.makeMove is called
                    parentWindow.nextTurn();
                }
            }
        }

        public void dropStone(char dropTeam) //Used to explicitly place pieces, ignoring rules
        {
            this.removePiece();
            Bitmap placedPiece = new Bitmap(empty);
            Graphics gr = Graphics.FromImage(placedPiece);
            gr.DrawImage(Image.FromFile("Resource/" + dropTeam /*GameWindow.currentTeam*/ + "T.png"), 0, 0, empty.Width, empty.Height);
            this.placed = placedPiece;
            this.Image = this.placed;
            team = dropTeam;
            GameWindow.currentBoard[address] = dropTeam;
        }

        public void removePiece()
        {
            this.Image = this.empty;
            team = '\0';
        }

    }
}
