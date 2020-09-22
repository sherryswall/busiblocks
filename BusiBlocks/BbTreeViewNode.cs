using System;
using System.Collections.Generic;
using System.Linq;

namespace BusiBlocks
{
    public class BbTreeViewNode
    {
        public string Id { get; set; }
        public string Value { get; set; }
        
        public BbTreeViewNodeMenu Menu { get; set; }

        public string Icon { get; set; }

        public bool IsLeaf { get; set; }

        public bool IsBranch {
            set { IsLeaf = !value; }
            get { return !IsLeaf; }
        }

        public List<BbTreeViewNode> ChildNodes { get; set; }
        public BbTreeViewNode ParentNode { get; set; }

        public bool Selected { get; set; }

        public BbTreeViewNode(BbTreeViewNode parentNode)
        {
            ChildNodes = new List<BbTreeViewNode>();
            ParentNode = parentNode;
        }

        public BbTreeViewNode(string id, string value, bool isLeaf, BbTreeViewNode parentNode)
            : this(parentNode)
        {
            Id = id;
            Value = value;
            IsLeaf = isLeaf;

            ChildNodes = new List<BbTreeViewNode>();
            Menu = new BbTreeViewNodeMenu(Id);
        }

        public BbTreeViewNode(string id, string value, bool isLeaf, string icon, BbTreeViewNode parentNode)
            : this(id, value, isLeaf, parentNode)
        {
            Menu = new BbTreeViewNodeMenu(id);
            Icon = icon;

        }
    }
}
