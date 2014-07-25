using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using EnterlectChallenge;
using System.Threading;
using System.Linq;

namespace PacManDuelBot
{
    class Program
    {
        private static TreeNode Root;
        private static int DepthToSearch = 20;
        private const int WIDTH = 19;
        private const int HEIGHT = 22;
        private const int PORTAL1_X = 10;
        private const int PORTAL1_Y = 0;
        private const int PORTAL2_X = 10;
        private const int PORTAL2_Y = 18;
        private const char WALL = '#';
        private const char ONE_POINTER = '.';
        private const char TEN_POINTER = '*';
        private const char POISON_PILL = '!';
        private const char EMPTY = ' ';
        private const char PLAYER_A = 'A';
        private const char PLAYER_B = 'B';
        private const String OUTPUT_FILE_NAME = "game.state";
        private const String INITIAL_FILE_NAME = @"..\..\initial.state";
        private static int runningTally = 0;

        static void Main(string[] args)
        {
            //so far this will, read the file into a 2d char array, figure out possible moves, pick one of these at random, make the move, output the result, and wirte the file back.

            //what need to be fixed is:
            //      more intelligent move determination, deeper scanning
            //          this might involve not using a 2d char array.
            //          a tree or a graph might be the answer to picking better moves
            //      a better GUI / output of the file, so that it is easier to see what is going on.
            //
            //      need to make consideration of the other player, if it is worth while to eat them when in range.
            //      need to think about the poinson pill, would it be worth while to drop and eat your own at some point to get back to the cetre of the board quickly
            var board = ReadMaze(INITIAL_FILE_NAME);
            var coordinate = GetCurrentPosition(board);
            //create the root of the tree, then call the method that will create the rest of the tree recusively
            Root = new TreeNode(board[coordinate.X][coordinate.Y], new Point(coordinate.X, coordinate.Y));


            if (!coordinate.IsEmpty)
            {
                var possibleMoveList = DetermineMoves(coordinate, board);

                CreateTree(board, coordinate, possibleMoveList, DepthToSearch);

                var bestMove = DetermineBestMove(possibleMoveList, coordinate, board, 15);
                board = MakeMove(coordinate, bestMove, board);
                WriteMaze(board, OUTPUT_FILE_NAME);
            }
            while (true)
            {
                board = ReadMaze(OUTPUT_FILE_NAME);
                coordinate = GetCurrentPosition(board);
                if (!coordinate.IsEmpty)
                {
                    var possibleMoveList = DetermineMoves(coordinate, board);
                    var bestMove = DetermineBestMove(possibleMoveList, coordinate, board, 15);
                    board = MakeMove(coordinate, bestMove, board);
                    WriteMaze(board, OUTPUT_FILE_NAME);
                    Thread.Sleep(100);
                }
            }
        }

        private static Point GetCurrentPosition(char[][] maze)
        {
            var coordinate = new Point();
            for (var x = 0; x < HEIGHT; x++)
            {
                for (var y = 0; y < WIDTH; y++)
                {
                    if (maze[x][y].Equals(PLAYER_A))
                    {
                        coordinate.X = x;
                        coordinate.Y = y;
                    }
                }
            }
            return coordinate;
        }

        public static Point DetermineBestMove(List<Point> possibleMoves ,Point currentPosition, char[][] board, int depthToSearch)
        {
            //we need to recursively find the best move, based one the next possible set of moves, and so on

            //for this we could use the value property on the point object, so trace throught the possible moves after the next ones, to find the best option right now.
            var movesWithValues = new List<Point>();
            var bestMove = possibleMoves[0];
            foreach (var move in possibleMoves)
            {
                if (depthToSearch > 0)
                {
                    move.valueOfMove = board[move.X][move.Y] != EMPTY ? (board[move.X][move.Y] == TEN_POINTER ? 10 : (board[move.X][move.Y] == ONE_POINTER ? 1 : 0)) : 0;
                    movesWithValues.Add(move);
                    DetermineBestMove(DetermineMoves(move, board), move, board, depthToSearch - 1);
                }

                //if (board[move.X][move.Y] != board[bestMoveSoFar.X][bestMoveSoFar.Y])
                //{
                //    if (board[move.X][move.Y] == TEN_POINTER)
                //    {
                //        bestMoveSoFar = depthToSearch > 0 ? DetermineBestMove(DetermineMoves(move,board), move, board, depthToSearch-1) : move;
                //    }
                //    else if (board[move.X][move.Y] == ONE_POINTER)
                //    {
                //        bestMoveSoFar = depthToSearch > 0 ? DetermineBestMove(DetermineMoves(move, board), move, board, depthToSearch - 1) : move;
                //    }
                //    else if (board[move.X][move.Y] == EMPTY)
                //        bestMoveSoFar = depthToSearch > 0 ? DetermineBestMove(DetermineMoves(move, board), move, board, depthToSearch - 1) : move;
                //}
                depthToSearch--;

                foreach (var valMove in movesWithValues)
                {
                    bestMove = valMove.valueOfMove >= bestMove.valueOfMove ? valMove : bestMove;
                }
            }
            return bestMove;
        }

        private static List<Point> DetermineMoves(Point currentPosition, char[][] board)
        {
            var moveList = new List<Point>();
            if (currentPosition.Y + 1 < WIDTH)
                if (!board[currentPosition.X][currentPosition.Y + 1].Equals(WALL))
                    moveList.Add(new Point(currentPosition.X, currentPosition.Y + 1 ));

            if (currentPosition.Y - 1 >= 0)
                if (!board[currentPosition.X][currentPosition.Y - 1].Equals(WALL))
                    moveList.Add(new Point(currentPosition.X, currentPosition.Y - 1));

            if (currentPosition.X + 1 < HEIGHT)
                if (!board[currentPosition.X + 1][currentPosition.Y].Equals(WALL))
                    moveList.Add(new Point(currentPosition.X + 1, currentPosition.Y));

            if (currentPosition.X - 1 >= 0)
                if (!board[currentPosition.X - 1][currentPosition.Y].Equals(WALL))
                    moveList.Add(new Point(currentPosition.X - 1, currentPosition.Y));

            if (currentPosition.X.Equals(PORTAL1_X) && currentPosition.Y.Equals(PORTAL1_Y))
                moveList.Add(new Point(PORTAL2_X, PORTAL2_Y));

            if (currentPosition.X.Equals(PORTAL2_X) && currentPosition.Y.Equals(PORTAL2_Y))
                moveList.Add(new Point(PORTAL1_X, PORTAL1_Y));

            return moveList;
        }

        private static char[][] MakeMove(Point currentPoint, Point movePoint, char[][] maze)
        {
            switch (maze[movePoint.X][movePoint.Y])
            {
                case ONE_POINTER:
                    runningTally += 1;
                    break;
                case TEN_POINTER:
                    runningTally += 10;
                    break;
            }
            maze[currentPoint.X][currentPoint.Y] = ' ';
            maze[movePoint.X][movePoint.Y] = PLAYER_A;
            return maze;
        }

        private static void WriteMaze(char[][] maze, String filePath)
        {
            using (var file = new System.IO.StreamWriter(filePath))
            {
                var output = "";
                for (var x = 0; x < HEIGHT; x++)
                {
                    for (var y = 0; y < WIDTH; y++)
                    {
                        output += maze[x][y];
                    }
                    if (x != HEIGHT - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
                PrintToScreen(output);
            }
        }

        private static void PrintToScreen(string outputGameContents)
        {
            //in here need to do a nice gui
            outputGameContents = outputGameContents.Replace("#", "###");
            outputGameContents = outputGameContents.Replace(" ", "   ");
            outputGameContents = outputGameContents.Replace("A", " A ");
            outputGameContents = outputGameContents.Replace("B", " B ");
            outputGameContents = outputGameContents.Replace(".", " . ");
            outputGameContents = outputGameContents.Replace("*", " * ");
            outputGameContents = outputGameContents.Replace("!", " ! ");
            outputGameContents += "\r\n\r\n";
            Console.Write(outputGameContents);
            Console.Write(String.Format("Current Tally: {0} \r\n", runningTally));

        }

        private static char[][] ReadMaze(String filePath)
        {
            var map = new char[HEIGHT][];
            try
            {
                var fileContents = System.IO.File.ReadAllText(filePath);
                var rowCount = 0;
                foreach (var row in Regex.Split(fileContents, "\n"))
                {
                    map[rowCount] = row.ToCharArray();
                    rowCount++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            return map;
        }

        public static void CreateTree(char[][] board, Point currentPoint, List<Point> possibleMoves, int depth)
        {
            if (depth != 0)
            {
                depth--;
                var localBoard = board;
                foreach (var move in possibleMoves)
                {
                    MakeMove(currentPoint, move, localBoard);
                    Root.Add(new TreeNode(board[move.X][move.Y], move));
                    CreateTree(localBoard, move, DetermineMoves(move, localBoard), depth-1);
                }
            }
        }

    }
}