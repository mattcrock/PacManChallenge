using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using EnterlectChallenge;
using System.Threading;

namespace PacManDuelBot
{
    class Program
    {
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
            if (!coordinate.IsEmpty)
            {
                var possibleMoveList = DetermineMoves(coordinate, board);
                var bestMove = DetermineBestMove(possibleMoveList, coordinate, board);
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
                    var bestMove = DetermineBestMove(possibleMoveList, coordinate, board);
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

        public static Point DetermineBestMove(List<Point> possibleMoves ,Point currentPosition, char[][] board)
        {
            var random = new Random();
            var randomMoveIndex = random.Next(0, possibleMoves.Count);
            var bestMoveSoFar = possibleMoves[0];
            foreach (var move in possibleMoves)
            {
                if (board[move.X][move.Y] != board[bestMoveSoFar.X][bestMoveSoFar.Y])
                {
                    if (board[move.X][move.Y] == TEN_POINTER)
                        bestMoveSoFar = move;
                    else if (board[move.X][move.Y] == ONE_POINTER)
                        bestMoveSoFar = move;
                    //else if (board[move.X][move.Y] == EMPTY)
                    //    bestMoveSoFar = move;
                }
            }
            return bestMoveSoFar;
        }

        private static List<Point> DetermineMoves(Point currentPosition, char[][] board)
        {
            var moveList = new List<Point>();
            if (currentPosition.Y + 1 < WIDTH)
                if (!board[currentPosition.X][currentPosition.Y + 1].Equals(WALL))
                    moveList.Add(new Point { X = currentPosition.X, Y = currentPosition.Y + 1 });

            if (currentPosition.Y - 1 >= 0)
                if (!board[currentPosition.X][currentPosition.Y - 1].Equals(WALL))
                    moveList.Add(new Point { X = currentPosition.X, Y = currentPosition.Y - 1 });

            if (currentPosition.X + 1 < HEIGHT)
                if (!board[currentPosition.X + 1][currentPosition.Y].Equals(WALL))
                    moveList.Add(new Point { X = currentPosition.X + 1, Y = currentPosition.Y });

            if (currentPosition.X - 1 >= 0)
                if (!board[currentPosition.X - 1][currentPosition.Y].Equals(WALL))
                    moveList.Add(new Point { X = currentPosition.X - 1, Y = currentPosition.Y });

            if (currentPosition.X.Equals(PORTAL1_X) && currentPosition.Y.Equals(PORTAL1_Y))
                moveList.Add(new Point { X = PORTAL2_X, Y = PORTAL2_Y });

            if (currentPosition.X.Equals(PORTAL2_X) && currentPosition.Y.Equals(PORTAL2_Y))
                moveList.Add(new Point { X = PORTAL1_X, Y = PORTAL1_Y });

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
            outputGameContents += "\r\n\r\n";
            Console.Write(outputGameContents);
            Console.Write(String.Format("Current Tally: {0} \r\n", runningTally));

        }

        private static TreeNode ReadMazeTree(String filePath)
        {
            var fileContents = System.IO.File.ReadAllText(filePath);
            var boardTree = new TreeNode(fileContents);
            return boardTree;
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

    }
}