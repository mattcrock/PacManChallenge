using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using EnterlectChallenge;

namespace PacManDuelBot
{
    class Program
    {
        private const int _WIDTH = 19;
        private const int _HEIGHT = 22;
        private const int _PORTAL1_X = 10;
        private const int _PORTAL1_Y = 0;
        private const int _PORTAL2_X = 10;
        private const int _PORTAL2_Y = 18;
        private const char _WALL = '#';
        private const char _PLAYER_SYMBOL = 'A';
        private const String _OUTPUT_FILE_NAME = "game.state";

        static void Main(string[] args)
        {
            var maze = ReadMaze(args[0]);
            var coordinate = GetCurrentPosition(maze);
            if (!coordinate.IsEmpty)
            {
                var possibleMoveList = DetermineMoves(coordinate, maze);
                var random = new Random();
                var randomMoveIndex = random.Next(0, possibleMoveList.Count);
                maze = MakeMove(coordinate, possibleMoveList[randomMoveIndex], maze);
                WriteMaze(maze, _OUTPUT_FILE_NAME);
            }
        }

        private static Point GetCurrentPosition(char[][] maze)
        {
            var coordinate = new Point();
            for (var x = 0; x < _HEIGHT; x++)
            {
                for (var y = 0; y < _WIDTH; y++)
                {
                    if (maze[x][y].Equals(_PLAYER_SYMBOL))
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
            if (currentPoint.Y + 1 < _WIDTH)
                if (!maze[currentPoint.X][currentPoint.Y + 1].Equals(_WALL))
                    moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y + 1 });

            if (currentPoint.Y - 1 >= 0)
                if (!maze[currentPoint.X][currentPoint.Y - 1].Equals(_WALL))
                    moveList.Add(new Point { X = currentPoint.X, Y = currentPoint.Y - 1 });

            if (currentPoint.X + 1 < _HEIGHT)
                if (!maze[currentPoint.X + 1][currentPoint.Y].Equals(_WALL))
                    moveList.Add(new Point { X = currentPoint.X + 1, Y = currentPoint.Y });

            if (currentPoint.X - 1 >= 0)
                if (!maze[currentPoint.X - 1][currentPoint.Y].Equals(_WALL))
                    moveList.Add(new Point { X = currentPoint.X - 1, Y = currentPoint.Y });

            if (currentPoint.X.Equals(_PORTAL1_X) && currentPoint.Y.Equals(_PORTAL1_Y))
                moveList.Add(new Point { X = _PORTAL2_X, Y = _PORTAL2_Y });

            if (currentPoint.X.Equals(_PORTAL2_X) && currentPoint.Y.Equals(_PORTAL2_Y))
                moveList.Add(new Point { X = _PORTAL1_X, Y = _PORTAL1_Y });

            return moveList;
        }

        private static char[][] MakeMove(Point currentPoint, Point movePoint, char[][] maze)
        {
            maze[currentPoint.X][currentPoint.Y] = ' ';
            maze[movePoint.X][movePoint.Y] = _PLAYER_SYMBOL;
            return maze;
        }

        private static void WriteMaze(char[][] maze, String filePath)
        {
            using (var file = new System.IO.StreamWriter(filePath))
            {
                var output = "";
                for (var x = 0; x < _HEIGHT; x++)
                {
                    for (var y = 0; y < _WIDTH; y++)
                    {
                        output += maze[x][y];
                        Console.Write("hello");
                    }
                    if (x != _HEIGHT - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
            }
        }

        private static Tree ReadMazeTree(String filePath)
        {
            var fileContents = System.IO.File.ReadAllText(filePath);
            var boardTree = new Tree(fileContents);
            return boardTree;
        }

        private static char[][] ReadMaze(String filePath)
        {
            var map = new char[_HEIGHT][];
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