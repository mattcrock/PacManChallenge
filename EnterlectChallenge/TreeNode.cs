using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterlectChallenge
{
    class TreeNode : IEnumerable<TreeNode>
    {
        private readonly Dictionary<string, TreeNode> _childs = new Dictionary<string, TreeNode>();

        public readonly string Value;
        public TreeNode Parent { get; private set; }
        public TreeNode MoveUp { get; private set; }
        public TreeNode MoveDown { get; private set; }
        public TreeNode MoveLeft { get; private set; }
        public TreeNode MoveRight { get; private set; }

        public TreeNode(string value)
        {
            this.Value = value;
        }

        public TreeNode GetChild(string id)
        {
            return this._childs[id];
        }

        public void Add(TreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._childs.Remove(item.Value);
            }

            item.Parent = this;
            this._childs.Add(item.Value, item);
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            return this._childs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get { return this._childs.Count; }
        }
    }
}
