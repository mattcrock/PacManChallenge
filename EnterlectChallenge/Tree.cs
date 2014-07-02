using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterlectChallenge
{
    class Tree
    {
        private Point coOrd;
        private Tree up, down, left, right;

        public Tree(Point coOrd, Tree left, Tree right)
        {
            this.coOrd = coOrd;
            this.left = left;
            this.right = right;
        }

        public Tree(){}

        public Tree(string fileContents)
        {
            fileContents.Split('\n');
        }
    }
}
