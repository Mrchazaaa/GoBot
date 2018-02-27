using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Go
{
    static class AI
    {
        public static GameWindow parentWindow;
        public static char teamSim;
        public static int CapturedWhite;
        public static int CapturedBlack;
        public static Stopwatch koTimer;
        public static List<node> passedNodes;
        public static List<int> workingEmptyPieces;
        static Random rand = new Random();

        public static void passNodeRecursible(int currentIndex, int parentNodeIndex) //currentIndex is in regards of tree.Nodes / parentNodeIndex is in terms of passedNodes
        {
            passedNodes.Add( tree.Nodes[currentIndex].copyNode() );
            int address = passedNodes.Count - 1;
            passedNodes[address].address = address;
            passedNodes[address].parentNodeAddress = parentNodeIndex;

            if (tree.moveNodeAddress.Keys.Contains(passedNodes[address].move))
            {
                tree.moveNodeAddress[passedNodes[address].move].Add(address);
            }
            else
            {
                tree.moveNodeAddress.Add(passedNodes[address].move, new List<int>());
                tree.moveNodeAddress[passedNodes[address].move].Add(address);
            }

            if (address != 0)
            {
                passedNodes[parentNodeIndex].moveNodeAddress[passedNodes[address].move] = address;
            }

            if (tree.Nodes[currentIndex].moveNodeAddress.Count != 0)
            {
                foreach (int positAddress in tree.Nodes[currentIndex].moveNodeAddress.Values)
                {
                    passNodeRecursible(positAddress, address);
                }
            }

            return;
        }

        public static void makeMove()
        {
            tree.moveNodeAddress = new Dictionary<int, List<int>>();

            List<char> connected = new List<char>();
            foreach (int i in Rules.returnConnectedTiles(GameWindow.lastBlackMove))
            {
                connected.Add(GameWindow.currentBoard[i]);
            }



            if (passedNodes != null && connected.Count(x => x == 'W' ||x == '\0') > 0) //So that nodes are not passed on the first call of makeMove (as no prior move has been made)
            {
                passedNodes = new List<node>();

                if (tree.currentNode.moveNodeAddress.Keys.Contains(GameWindow.lastBlackMove))
                {
                    if (tree.currentNode.moveNodeAddress.Count != 0 && tree.Nodes[tree.currentNode.moveNodeAddress[GameWindow.lastBlackMove]].moveNodeAddress.Count != 0) //No child addresseses
                    {
                        int nodeAddress = tree.currentNode.moveNodeAddress[GameWindow.lastBlackMove]; //Sets current node to the current position of the game (what if the pc has not yet simulated this black move?)
                        tree.currentNode = tree.Nodes[nodeAddress];
                        passedNodes = new List<node>();
                        passNodeRecursible(tree.currentNode.address, 0);

                        tree.Nodes = new List<node>();

                        foreach (node n in passedNodes)
                        {
                            tree.Nodes.Add(n);
                        }

                        tree.currentNode = tree.Nodes[0];
                    }
                    else
                    {
                        tree.starterTree();
                    }
                }
                else
                {
                    tree.starterTree();
                }
            }
            else
            {
                tree.starterTree();
            }


            passedNodes = new List<node>();

            Stopwatch Sw = new Stopwatch();

            int wins = 0;

            if (GameWindow.blackHasPassed)
            {
                Tuple<int, int> blackTeamWhiteTeam = Rules.countScore(GameWindow.currentBoard);

                if (blackTeamWhiteTeam.Item1 - GameWindow.capturedBlack < blackTeamWhiteTeam.Item2 - GameWindow.capturedWhite) //black score smaller than white score
                {
                    parentWindow.whitePasses();
                    return;
                }
            }
            //set a time limit by which time a move must be returned
            Sw.Start();

            while (Sw.Elapsed.TotalSeconds < parentWindow.computerThinkingTimeLimit) //a move must be determined in ... seconds
            {
                //selection
                //UCB will add all of the immediate possible nodes to every node it encounters and run atleast one simulation on each. However, this will still lead to asymetrical tree growth

                tree.currentNode = tree.Nodes[0];

                char[] localsBoardRep = new char[GameWindow.currentBoard.Length];

                Array.Copy(GameWindow.currentBoard, localsBoardRep, GameWindow.currentBoard.Length);

                workingEmptyPieces = new List<int>(GameWindow.emptyIndexes);

                while (tree.currentNode.moveNodeAddress.Count > 0) //whilst the current node has children
                {
                    char ct = tree.currentNode.team;
                    double ucbMAX = 0;
                    int addressing = 0;

                    foreach (int movee in tree.currentNode.moveNodeAddress.Values)
                    {
                        double ubcos = tree.Nodes[movee].UCBvalue();
                        if (ubcos >= ucbMAX)
                        {
                            ucbMAX = ubcos;
                            addressing = movee;
                        }
                    }
                    tree.currentNode = tree.Nodes[addressing];
                    HashSet<int> placedPiecesHashSet = Rules.placePiece(tree.currentNode.move, ref localsBoardRep, tree.currentNode.team);
                    workingEmptyPieces.Remove(tree.currentNode.move);
                    foreach (int removed in placedPiecesHashSet)
                    {
                        workingEmptyPieces.Add(removed);
                    }
                }
                char opt = tree.currentNode.team == 'W' ? 'B' : 'W';
                List<int> possibillies = new List<int>(Rules.LegalMoves(opt, localsBoardRep));
                foreach (int i in possibillies)
                {
                    tree.CreateChildNode(i);
                }

                int reminder = tree.currentNode.address;

                foreach (int childAddress in tree.currentNode.moveNodeAddress.Values)
                {
                    tree.currentNode = tree.Nodes[reminder];
                    Rules.placePiece(tree.currentNode.move, ref localsBoardRep, tree.currentNode.team);
                    tree.currentNode = tree.Nodes[childAddress];

                    HashSet<int> placedPiecesHashSet = Rules.placePiece(tree.currentNode.move, ref localsBoardRep, tree.currentNode.team);

                    workingEmptyPieces.Remove(tree.currentNode.move);
                    foreach (int removed in placedPiecesHashSet)
                    {
                        workingEmptyPieces.Add(removed);
                    }

                    teamSim = tree.currentNode.team == 'W' ? 'B' : 'W';

                    //Last leaf is left as the current node (for backpropogation)

                    Stopwatch localStopWatch = new Stopwatch(); //Used to end the current simulation if it continues for too long
                    localStopWatch.Start();

                    CapturedBlack = 0;
                    CapturedWhite = 0;

                    koTimer = new Stopwatch();

                    koTimer.Start();

                    simulateRecursion(ref localsBoardRep, teamSim);

                    koTimer.Stop();

                    Tuple<int, int> scores = Rules.countScore(localsBoardRep);

                    int score = (scores.Item2 - CapturedWhite) - (scores.Item1 - CapturedBlack);

                    wins = 0;

                    if (score > 0) wins++; //White wins

                    bool endOfTheLine = false;

                    foreach (int posit in tree.moveNodeAddress[tree.currentNode.move]) //RAVE
                    {
                        tree.Nodes[posit].simulations++;
                        tree.Nodes[posit].wins += wins;
                    }

                    tree.upTree();

                    while (!endOfTheLine)
                    {
                        tree.currentNode.simulations++;
                        tree.currentNode.wins += wins; // += wins;
                        tree.upTree();
                        if (tree.currentNode.address == 0)
                        {
                            tree.currentNode.simulations++;
                            tree.currentNode.wins += wins; // += wins;
                            endOfTheLine = true;
                        }
                    }
                }
            }

            tree.currentNode = tree.Nodes[0]; // Move back to top of tree

            Dictionary<double, int> scoreMove = new Dictionary<double, int>();


            foreach (int key in tree.currentNode.moveNodeAddress.Keys)
            {
                tree.downTree(key);
                double score = (double)((double)tree.currentNode.wins / (double)tree.currentNode.simulations);
                scoreMove[score] = tree.currentNode.move;
                tree.upTree();
            }

            if (scoreMove.Keys.Count == 0)
            {
                parentWindow.whitePasses();
                return;
            }

            tree.currentNode = tree.Nodes[tree.Nodes[0].moveNodeAddress[scoreMove[scoreMove.Keys.Max()]]]; //Sets the currentNode to the node selected by the computer
            GameWindow.tiles[tree.currentNode.move].PlacePiece();
            return;
        }

        public static void simulateRecursion(ref char[] board, char currentTeam)
        {
            int simMove = Rules.returnALegalMove(board, currentTeam);

            if (simMove != -1 && koTimer.ElapsedMilliseconds < 250)
            {
                HashSet<int> caps = new HashSet<int>(Rules.placePiece(simMove, ref board, currentTeam));

                workingEmptyPieces.Remove(simMove);

                foreach (int removed in caps)
                {
                    workingEmptyPieces.Add(removed);
                }


                if (currentTeam == 'W') CapturedBlack += caps.Count;
                else CapturedWhite += caps.Count;
                currentTeam = currentTeam == 'W' ? 'B' : 'W';

                simulateRecursion(ref board, currentTeam);
            }
            return;
        }
    }

    static class tree
    {
        public static List<node> Nodes;
        public static node currentNode;
        public static Dictionary<int, List<int>> moveNodeAddress; //store the multiple nodes that are represented by the same move (for RAVE)

        public static void starterTree()
        {
            Nodes = new List<node>();
            node parentNode = new node(0, -1, 0, true); //Negative one to represent that this is the initial board rep
            currentNode = parentNode;
            Nodes.Add(parentNode);
        }

        public static void CreateChildNode(int move)
        {
            int passer = Nodes.Count();
            currentNode.moveNodeAddress[move] = passer; //Because the child node will always be added to the end of the array so .count is its index
            Nodes.Add(new node(Nodes.Count(), move, currentNode.address)/*depth = Nodes[parentNodeIndex].depth + 1 */);
            if (!moveNodeAddress.Keys.Contains(move))
            {
                moveNodeAddress.Add(move, new List<int>());
            }
            moveNodeAddress[move].Add(Nodes.Count() - 1);
        }

        public static void upTree()
        {
            currentNode = Nodes[currentNode.parentNodeAddress];
        }


        public static void downTree(int indexer) //The indexer is used to choose which child node will be returned
        {
            if (currentNode.moveNodeAddress.Count <= 0)
            {
                throw new System.ArgumentException("This node has no children", "original");
            }
            currentNode = Nodes[currentNode.moveNodeAddress[indexer]];
        }
    }

    public class node
    {
        public char team; //Represents the team that has moved to create this new state
        public int move;
        public int address; //Stores the address of this node in it's tree's list
        public int parentNodeAddress; //A lookup for move of a child node to that child nodes address
        public int simulations;
        public int wins;
        public Dictionary<int, int> moveNodeAddress = new Dictionary<int, int>(); //Stores the positions of this nodes child nodes


        public node(int Address, int move, int parentNode = 0, bool isParentNode = false, int simulations = 0) //Constructor
        {
            this.simulations = simulations;
            this.wins = 0;
            this.move = move;
            if (!isParentNode)
            {
                team = tree.Nodes[parentNodeAddress].team == 'B' ? 'W' : 'B'; //Alternating teams on child nodes
            }
            else
            {
                team = 'B'; //As the computer will always be taking its turn after black
            }
            this.parentNodeAddress = parentNode;
            this.address = Address;

        }

        public double UCBvalue()
        {
            double value = simulations <= 0 ? 100 : (double)((double)wins / (double)simulations) + Math.Sqrt((2 * Math.Log(tree.Nodes[parentNodeAddress].simulations)) / (simulations));
            return value;
        }

        public node copyNode()
        {
            node toReturn = new node(this.address, this.move, this.parentNodeAddress, false, this.simulations);
            toReturn.team = this.team;
            toReturn.wins = this.wins;
            toReturn.moveNodeAddress = new Dictionary<int, int>( this.moveNodeAddress );
            return toReturn;
        }

    }
}

