﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterlectChallenge
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsEmpty { get; set; }
        public int valueOfMove { get; set; }

        public Point() { }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
