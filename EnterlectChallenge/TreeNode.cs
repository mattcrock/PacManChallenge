using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterlectChallenge
{
    class TreeNode 
    {
        private List<TreeNode> children = new List<TreeNode>();

        public readonly char Value;
        public readonly Point coOrd;
        public TreeNode Parent { get; private set; }
        

        public TreeNode(char value, Point coOrd)
        {
            this.Value = value;
            this.coOrd = coOrd;
        }

        public TreeNode GetChild(int id)
        {
            return this.children[id];
        }

        public void Add(TreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent.children.Remove(item);
            }

            item.Parent = this;
            this.children.Add(item);
        }

        public int Count
        {
            get { return this.children.Count; }
        }
    }
}
