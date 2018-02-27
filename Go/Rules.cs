using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Go
{
    static class Rules
    {
        public static int boardWidth;

        public static HashSet<int> placePiece(int moveTo, ref char[] board, char currentTeam) //Returns positions of removed pieces
        {
            char[] original = new char[board.Length];

            Array.Copy(board, original, board.Length);

            HashSet<int> taken = new HashSet<int>();
            
            if (moveTo == -1) return taken;

            char opposingTeam = currentTeam == 'W' ? 'B' : 'W';
            if (board[moveTo] == '\0')
            {
                board[moveTo] = currentTeam;
                HashSet<int> immediateOpponents = new HashSet<int>();
                foreach (int i in returnConnectedTiles(moveTo))
                {
                    if (board[i] == opposingTeam)
                    {
                        immediateOpponents.Add(i);
                    }
                }
                foreach (int opponent in immediateOpponents) //immediateopponents
                {
                    if (IsTeamCaptured(board, opponent, opposingTeam))
                    {
                        HashSet<int> toBeTaken = new HashSet<int>();
                        returnConnectedTilesWrapper(opponent, board, ref toBeTaken, opposingTeam);
                        toBeTaken.Add(opponent);
                        List<int> yo = toBeTaken.ToList();

                        for (int j = 0; j < yo.Count; j++)
                        {
                            if (board[yo[j]] == opposingTeam)
                            {
                                board[yo[j]] = '\0';
                                taken.Add(yo[j]);
                            }
                        }
                    }
                }
                if (!IsTeamCaptured(board, moveTo, currentTeam))
                {

                }
                else
                {
                    Array.Copy(original, board, original.Length);
                }
            }
            return taken;
        }

        public static bool IsTeamCaptured(char[] boardRep, int movedTo, char checkingTeam)
        {
            char opposingTeam = checkingTeam == 'W' ? 'B' : 'W';

            HashSet<int> stringOfPieces = new HashSet<int>();

            returnConnectedTilesWrapper(movedTo, boardRep, ref stringOfPieces, checkingTeam);

            stringOfPieces.Remove(movedTo);

            HashSet<char> stringOfBoardTeams = new HashSet<char>(); 
            foreach (int position in stringOfPieces)
            {
                stringOfBoardTeams.Add(boardRep[position]); 
            }

            if (stringOfBoardTeams.Contains('\0')) 
            {
                return false;
            }
            else if (stringOfBoardTeams.Contains(opposingTeam) && !stringOfBoardTeams.Contains('\0')) //Contains white pieces
            {
                return true;
            }
            return true;
        }

        public static List<int> LegalMoves(char team, char[] BoardRep)
        {
            List<int> legalMoves = new List<int>();

            for (int i = 0; i < BoardRep.Length; i++)
            {
                if (BoardRep[i] == '\0')
                {
                    HashSet<char> kinex = new HashSet<char>();
                    foreach (int ay in returnConnectedTiles(i))
                    {
                        kinex.Add(BoardRep[ay]);
                    }

                    if (kinex.Contains('\0'))
                    {
                        legalMoves.Add(i);
                    }
                    else if (!kinex.Contains(team)) //Cannot move into a position directly encircled by the other team
                    {
                    }
                    else
                    {
                        char[] copyOfBoard = new char[boardWidth * boardWidth];

                        Array.Copy(BoardRep, copyOfBoard, boardWidth * boardWidth);

                        placePiece(i, ref copyOfBoard, team);
                        if (copyOfBoard[i] == team)
                        {
                            HashSet<int> immediacy = Rules.returnConnectedTiles(i);
                            HashSet<char> immed = new HashSet<char>();
                            foreach (int hint in immediacy)
                            {
                                immed.Add(BoardRep[hint]);
                            }

                            if (immed.Count == 1 && !immed.Contains(team) || immed.Count > 1) //If there is only one team surrounding the point, and the team surrounding that point it not the same team (for sims) || there is more than one type of team surrounding that position
                            {
                                legalMoves.Add(i);
                            }
                        }
                        
                    }

                }
            }
            return legalMoves;
        }

        public static int returnALegalMove(char[] board, char team)
        {
            List<int> movesToCheck = new List<int>(AI.workingEmptyPieces);

            Random rand = new Random();

            char[] testBoardRep = new char[board.Length];



            int toSelect = -1;

            while (toSelect == -1 && movesToCheck.Count > 0)
            {
                Array.Copy(board, testBoardRep, board.Length);

                int checking = movesToCheck[rand.Next(0, movesToCheck.Count - 1)];
                Rules.placePiece(checking, ref testBoardRep, team);

                if (testBoardRep[checking] == team)
                {
                    toSelect = checking;
                }
                else
                {
                    movesToCheck.Remove(checking);
                }
            }

            return toSelect;
        }

        public static HashSet<int> CapturedPieces(char[] tiles, int movedTo, char checkingTeam)
        {
            char opposingTeam = checkingTeam == 'B' ? 'W' : 'B';

            HashSet<int> piecesConnectedToInitialPiece = returnConnectedTiles(movedTo/*, tiles*/);
            HashSet<int> deletePieces = new HashSet<int>();
            foreach (int piece in piecesConnectedToInitialPiece)
            {
                if (tiles[piece] == opposingTeam)
                {
                    if (Rules.IsTeamCaptured(tiles, piece, opposingTeam))
                    {
                        HashSet<int> returnedConnectedTileWrapperResult = new HashSet<int>();
                        returnConnectedTilesWrapper(piece, tiles, ref returnedConnectedTileWrapperResult, opposingTeam);
                        deletePieces.UnionWith(returnedConnectedTileWrapperResult);
                    }
                }
            }

            return deletePieces;
        }

        public static void returnConnectedTilesWrapper(int startingIndex, char[] boardRep, ref HashSet<int> returnedConnectedPieces, char checkingTeam) //Turns returnConnectedTiles into a recursive function to find a string of pieces of the same team
        {
            HashSet<int> returnedConnectedTiles = returnConnectedTiles(startingIndex/*, tiles*/);
            HashSet<int> repeatAt = new HashSet<int>(); //Lists the positions of stones of the same team, meaning that they need to be searched from for more tiles of the same team to find the complete collection
            foreach (int bpPosition in returnedConnectedTiles)
            {
                if (boardRep[bpPosition] == checkingTeam && !returnedConnectedPieces.Contains(bpPosition)) //If the piece is of the same team, and has not already been searched (as the list to return contains all explored points)
                {
                    repeatAt.Add(bpPosition);
                }
                returnedConnectedPieces.Add(bpPosition);
            }
            if (repeatAt.Count == 0)
            {
                return;
            }
            else
            {
                foreach (int bpPosition in repeatAt)
                {
                    returnConnectedTilesWrapper(bpPosition, boardRep, ref returnedConnectedPieces, checkingTeam);
                }
            }
        }

        public static HashSet<int> returnConnectedTiles(int startingIndex)
        {
            HashSet<int> connectedTiles = new HashSet<int>();
            if (startingIndex <= boardWidth - 1) //Top row of board
            {
                connectedTiles.Add(startingIndex + boardWidth); //Tile below
                if (startingIndex == 0) //Top left
                {
                    connectedTiles.Add(startingIndex + 1); //Tile to the right
                }
                else if (startingIndex == boardWidth - 1) //Top right
                {
                    connectedTiles.Add(startingIndex - 1); //Tile to the left
                }
                else //Top row
                {
                    connectedTiles.Add(startingIndex + 1); //Tile to the right
                    connectedTiles.Add(startingIndex - 1); //Tile to the left
                }
            }

            else if (startingIndex >= boardWidth * (boardWidth - 1)) //Bottem row of board
            {
                connectedTiles.Add(startingIndex - boardWidth); //Tile to the top
                if (startingIndex == boardWidth * (boardWidth - 1)) //Bottem left
                {
                    connectedTiles.Add(startingIndex + 1); //Tile to the right
                }
                else if (startingIndex == (boardWidth * boardWidth) - 1) //Bottem right
                {
                    connectedTiles.Add(startingIndex - 1); //Tile to the left
                }
                else //Bottem row
                {
                    connectedTiles.Add(startingIndex + 1); //Tile to the right
                    connectedTiles.Add(startingIndex - 1); //Tile to the left
                }
            }

            else if (multiples(boardWidth).Contains(startingIndex) || multiples(boardWidth).Contains(startingIndex + 1)) //Left column of board or the right column
            {
                connectedTiles.Add(startingIndex - boardWidth); //Tile to the top
                connectedTiles.Add(startingIndex + boardWidth); //Tile below
                connectedTiles.Add(multiples(boardWidth).Contains(startingIndex) ? (startingIndex + 1) : (startingIndex - 1)); //If on the left column...
            }

            else //Not on the edge of the board
            {
                connectedTiles.Add(startingIndex + 1); //Tile to the right
                connectedTiles.Add(startingIndex - 1); //Tile to the left
                connectedTiles.Add(startingIndex - boardWidth); //Tile to the top
                connectedTiles.Add(startingIndex + boardWidth); //Tile below
            }
            return connectedTiles;
        }

        static List<int> multiples(int numToMult)
        {
            List<int> completeRange = new List<int>();
            for (int x = 0; x <= numToMult; x++)
            {
                completeRange.Add(x * numToMult);
            }
            return completeRange;
        }

        public static Tuple<int, int> countScore(char[] boardRep) //Needs to work by being sumbitted a board thing
        {
            int blackScore = 0;
            int whiteScore = 0;

            List<HashSet<int>> blackInfluence = new List<HashSet<int>>();
            List<HashSet<int>> whiteInfluence = new List<HashSet<int>>();

            HashSet<int> checkedIntesections = new HashSet<int>();

            int[] whiteScoreMap = new int[boardRep.Count()];
            int[] blackScoreMap = new int[boardRep.Count()];

            for (int n = 0; n < boardWidth * boardWidth; n++)
            {
                whiteScoreMap[n] = -1;
                blackScoreMap[n] = -1;
            }

            for (int j = 0; j < boardRep.Count(); j++)
            {
                if (boardRep[j] == 'B')
                {
                    if (!checkedIntesections.Contains(j))
                    {
                        HashSet<int> pieceString = getString(boardRep, 'B', j);

                        List<HashSet<int>> areas = getEyes(j, boardRep);

                        for (int x = 0; x < areas.Count(); x++)
                        {
                            blackInfluence.Add(areas[x]);
                            foreach (int i in areas[x])
                            {
                                blackScoreMap[i] = blackInfluence.Count() - 1;
                            }
                        }

                        foreach (int i in pieceString)
                        {
                            checkedIntesections.Add(i);
                        }
                    }
                }
                else if (boardRep[j] == 'W')
                {
                    if (!checkedIntesections.Contains(j))
                    {
                        HashSet<int> pieceString = getString(boardRep, 'W', j);

                        List<HashSet<int>> areas = getEyes(j, boardRep);

                        for (int x = 0; x < areas.Count(); x++)
                        {
                            whiteInfluence.Add(areas[x]);
                            foreach (int i in areas[x])
                            {
                                whiteScoreMap[i] = whiteInfluence.Count() - 1;
                            }
                        }

                        foreach (int i in pieceString)
                        {
                            checkedIntesections.Add(i);
                        }
                    }
                }
            }

            for (int x = 0; x < boardWidth * boardWidth; x++) //Sorts territory claimed by both teams
            {
                if (whiteScoreMap[x] != -1 && blackScoreMap[x] != -1)
                {
                    HashSet<int> whiteArea = whiteInfluence[whiteScoreMap[x]];
                    HashSet<int> blackArea = blackInfluence[blackScoreMap[x]];

                    HashSet<int> matches = new HashSet<int>();

                    if (whiteArea.Count < blackArea.Count)
                    {
                        foreach (int i in whiteArea)
                        {
                            if (blackArea.Contains(i))
                            {
                                matches.Add(i);
                            }
                        }
                        if (matches.Count == whiteArea.Count)
                        {
                            foreach (int i in whiteArea)
                            {
                                blackArea.Remove(i);
                            }
                        }
                        else
                        {
                            HashSet<int> copy = new HashSet<int>(whiteArea);
                            foreach (int y in copy)
                            {
                                if (whiteArea.Contains(y) && blackArea.Contains(y))
                                {
                                    whiteArea.Remove(y);
                                    blackArea.Remove(y);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (int i in blackArea)
                        {
                            if (whiteArea.Contains(i))
                            {
                                matches.Add(i);
                            }
                        }
                        if (matches.Count == blackArea.Count)
                        {
                            foreach (int i in blackArea)
                            {
                                whiteArea.Remove(i);
                            }
                        }
                        else
                        {
                            HashSet<int> copy = new HashSet<int>(whiteArea);
                            foreach (int y in copy)
                            {
                                if (whiteArea.Contains(y) && blackArea.Contains(y))
                                {
                                    whiteArea.Remove(y);
                                    blackArea.Remove(y);
                                }
                            }
                        }
                    }
                }
            }

            foreach (HashSet<int> z in blackInfluence)
            {
                foreach (int x in z)
                {
                    if (boardRep[x] == '\0')
                    {
                        blackScore++;
                    }
                }
            }
            foreach (HashSet<int> z in whiteInfluence)
            {
                foreach (int x in z)
                {
                    if (boardRep[x] == '\0')
                    {
                        whiteScore++;
                    }
                }
            }

            Tuple<int, int> blackWhiteScore = new Tuple<int, int>(blackScore, whiteScore);

            return blackWhiteScore;
        }

        public static List<HashSet<int>> getEyes(int startingPoint, char[] BoardRepresentation)
        {
            List<HashSet<int>> eyes = new List<HashSet<int>>();
            List<HashSet<int>> perimeters = new List<HashSet<int>>();
            char team = BoardRepresentation[startingPoint];

            List<int> totalParameter = findPerimeter(BoardRepresentation, startingPoint, team);

            HashSet<int> toCheck = new HashSet<int>(totalParameter);

            while (toCheck.Count != 0)
            {
                HashSet<int> oneSideParameter = new HashSet<int>();

                int chosenPoint = toCheck.ToList()[0];


                getEyeRecursible(BoardRepresentation, chosenPoint, team, totalParameter, ref oneSideParameter);

                perimeters.Add(oneSideParameter);

                foreach (int j in oneSideParameter)
                {
                    toCheck.Remove(j);
                }
            }

            int perimsIndex = 0;

            for (int i = 0; i < perimeters.Count(); i++) //removes largest perimeter
            {
                if (perimeters[perimsIndex].Count < perimeters[i].Count)
                {
                    perimsIndex = i;
                }
            }

            perimeters.RemoveAt(perimsIndex);

            foreach (HashSet<int> a in perimeters)
            {
                HashSet<int> area = findArea(BoardRepresentation, a.ToList()[0], team);
                eyes.Add(area);
            }

            return eyes;
        }

        public static void getEyeRecursible(char[] boardRepre, int position, char team, List<int> totalPerim, ref HashSet<int> eyePerim)
        {
            HashSet<int> repeatAt = new HashSet<int>();
            if (totalPerim.Contains(position))
            {
                if (!eyePerim.Contains(position))
                {
                    eyePerim.Add(position);
                    repeatAt.Add(position);
                    foreach (int x in returnConnectedTiles(position))
                    {
                        if (boardRepre[x] != team)
                        {
                            repeatAt.Add(x);
                        }
                    }
                }
            }
            else
            {
                HashSet<int> surroundings = returnConnectedTiles(position);
                foreach (int x in surroundings)
                {
                    if (boardRepre[x] != team && totalPerim.Contains(x) && !eyePerim.Contains(x))
                    {
                        repeatAt.Add(x);
                    }
                }
            }
            foreach (int i in repeatAt)
            {
                getEyeRecursible(boardRepre, i, team, totalPerim, ref eyePerim);
            }
            return;
        }

        public static HashSet<int> getString(char[] boardRepre, char team, int startingPoint)
        {
            HashSet<int> stringCoords = new HashSet<int>();

            stringCoords.Add(startingPoint);

            getStringRecursible(boardRepre, team, ref stringCoords, startingPoint);

            return stringCoords;
        }

        public static void getStringRecursible(char[] boardRepre, char team, ref HashSet<int> stringCoords, int position)
        {
            HashSet<int> repeatAt = new HashSet<int>();

            foreach (int i in returnExtendedConnectedTiles(position))
            {
                if (boardRepre[i] == team)
                {
                    if (!stringCoords.Contains(i))
                    {
                        stringCoords.Add(i);
                        repeatAt.Add(i);
                    }
                }
            }

            if (repeatAt.Count != 0)
            {
                foreach (int i in repeatAt)
                {
                    getStringRecursible(boardRepre, team, ref stringCoords, i);
                }
            }
            else
            {
                return;
            }

        }

        public static HashSet<int> returnExtendedConnectedTiles(int startingIndex)
        {
            HashSet<int> connectedTiles = new HashSet<int>();

            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;


            if (!multiples(boardWidth).Contains(startingIndex) && 0 <= (startingIndex - 1)) //Left
            {
                connectedTiles.Add(startingIndex - 1);
                left = true;
            }

            if (!multiples(boardWidth).Contains(startingIndex + 1) && (boardWidth * boardWidth) > (startingIndex + 1)) //Right
            {
                connectedTiles.Add(startingIndex + 1);
                right = true;
            }

            if (startingIndex < (boardWidth * (boardWidth - 1))) //Down
            {
                connectedTiles.Add(startingIndex + boardWidth);
                down = true;
            }

            if (startingIndex > (boardWidth - 1)) //Up
            {
                connectedTiles.Add(startingIndex - boardWidth);
                up = true;
            }

            if (up)
            {
                if (right)
                {
                    connectedTiles.Add(startingIndex - boardWidth + 1);
                }
                if (left)
                {
                    connectedTiles.Add(startingIndex - boardWidth - 1);
                }
            }
            if (down)
            {
                if (right)
                {
                    connectedTiles.Add(startingIndex + boardWidth + 1);
                }
                if (left)
                {
                    connectedTiles.Add(startingIndex + boardWidth - 1);
                }
            }

            connectedTiles.Remove(startingIndex);
            return connectedTiles;
        }

        public static List<int> findPerimeter(char[] boardRep, int startingPoint, char team)
        {
            List<int> parameter = new List<int>();
            List<int> check = new List<int>();
            if (team != boardRep[startingPoint])
            {
                return parameter;
            }

            check.Add(startingPoint);

            findPerimeterRecursible(startingPoint, boardRep, ref parameter, ref check, team);

            return parameter;

        }

        public static HashSet<int> findArea(char[] influenceMap, int startingPoint, char team)
        {
            HashSet<int> Area = new HashSet<int>();
            Area.Add(startingPoint);

            findAreaRecursible(startingPoint, influenceMap, ref Area, team);

            return Area;

        }

        public static void findAreaRecursible(int startingIndex, char[] boardRep, ref HashSet<int> Area, char team)
        {
            HashSet<int> repeatAt = new HashSet<int>(); //--------
            foreach (int i in returnConnectedTiles(startingIndex))
            {
                if (!Area.Contains(i))
                {
                    if (team != boardRep[i])
                    {
                        Area.Add(i);
                        repeatAt.Add(i);
                    }
                }
            }
            if (repeatAt.Count != 0)
            {
                foreach (int i in repeatAt)
                {
                    findAreaRecursible(i, boardRep, ref Area, team);
                }
            }
            else
            {
                return;
            }
        }

        public static void findPerimeterRecursible(int startingIndex, char[] boardRep, ref List<int> param, ref List<int> hasBeenChecked, char team)
        {
            List<int> repeatAt = new List<int>();

            foreach (int i in returnExtendedConnectedTiles(startingIndex)) 
            {
                if (!hasBeenChecked.Contains(i))
                {
                    if (boardRep[i] == team)
                    {
                        hasBeenChecked.Add(i);
                        repeatAt.Add(i);
                    }
                    else
                    {
                        HashSet<int> surrounding = returnConnectedTiles(i);
                        HashSet<char> surr = new HashSet<char>();
                        foreach (int u in surrounding)
                        {
                            surr.Add(boardRep[u]);
                        }

                        if (surr.Contains(team))
                        {
                            param.Add(i);
                        }
                    }
                }
            }
            if (repeatAt.Count != 0)
            {
                foreach (int i in repeatAt)
                {
                    findPerimeterRecursible(i, boardRep, ref param, ref hasBeenChecked, team);
                }
            }
            else
            {
                return;
            }

        }


    }
}
