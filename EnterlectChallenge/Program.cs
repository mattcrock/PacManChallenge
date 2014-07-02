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
        private const char PLAYER_A = 'A';
        private const char PLAYER_B = 'B';
        private const String OUTPUT_FILE_NAME = "game.state";
        private const String INITIAL_FILE_NAME = @"..\..\initial.state";

        static void Main(string[] args)
        {
            var maze = ReadMaze(INITIAL_FILE_NAME);
            var coordinate = GetCurrentPosition(maze);
            if (!coordinate.IsEmpty)
            {
                var possibleMoveList = DetermineMoves(coordinate, maze);
                var random = new Random();
                var randomMoveIndex = random.Next(0, possibleMoveList.Count);
                maze = MakeMove(coordinate, possibleMoveList[randomMoveIndex], maze);
                WriteMaze(maze, OUTPUT_FILE_NAME);
            }
            while (true)
            {
                maze = ReadMaze(OUTPUT_FILE_NAME);
                coordinate = GetCurrentPosition(maze);
                if (!coordinate.IsEmpty)
                {
                    var possibleMoveList = DetermineMoves(coordinate, maze);
                    var random = new Random();
                    var randomMoveIndex = random.Next(0, possibleMoveList.Count);
                    maze = MakeMove(coordinate, possibleMoveList[randomMoveIndex], maze);
                    WriteMaze(maze, OUTPUT_FILE_NAME);
                    Thread.Sleep(1000);
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

        private static List<Point> DetermineMoves(Point currentPoint, char[][] maze)
        {
            var moveList = new List<Point>();
            if (currentPoint.Y + 1 < WIDTH)
                if (!maze[currentPoint.X][currentPoint.Y + 1].Equals(WALL))
                    moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });

            if (currentPoint.Y - 1 >= 0)
                if (!maze[currentPoint.X][currentPoint.Y - 1].Equals(WALL))
                    moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });

            if (currentPoint.X + 1 < HEIGHT)
                if (!maze[currentPoint.X + 1][currentPoint.Y].Equals(WALL))
                    moveList.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });

            if (currentPoint.X - 1 >= 0)
                if (!maze[currentPoint.X - 1][currentPoint.Y].Equals(WALL))
                    moveList.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });

            if (currentPoint.X.Equals(PORTAL1_X) && currentPoint.Y.Equals(PORTAL1_Y))
                moveList.Add(new Point { X = PORTAL2_X, Y = PORTAL2_Y });

            if (currentPoint.X.Equals(PORTAL2_X) && currentPoint.Y.Equals(PORTAL2_Y))
                moveList.Add(new Point { X = PORTAL1_X, Y = PORTAL1_Y });

            return moveList;
        }

        private static char[][] MakeMove(Point currentPoint, Point movePoint, char[][] maze)
        {
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
            var output = outputGameContents.Replace("#", " #");
            output = output.Replace("A", " A");
            output = output.Replace("B", " B");
            output = output.Replace(".", " .");
            output = output.Replace("*", " *");
            output += "\r\n\r\n";
            Console.Write(output);
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