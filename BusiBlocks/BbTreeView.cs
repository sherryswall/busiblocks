using System;
using System.Collections.Generic;
using System.Linq;

namespace BusiBlocks
{
    public class BbTreeView
    {
        public List<BbTreeViewNode> Nodes;

        public BbTreeView()
        {
            Nodes = new List<BbTreeViewNode>();
        }


        public BbTreeView(List<BbTreeViewNode> Nodes)
        {
            Nodes = new List<BbTreeViewNode>(Nodes);
        }


        public bool Contains(string Id)
        {
            bool found = false;

            foreach (BbTreeViewNode node in Nodes)
            {
                if (!found)
                {
                    if (node.Id == Id)
                        found = true;
                    else if(node.IsBranch)
                        found = ChildrenContains(Id, node.ChildNodes);
                }
            }

            return found;
        }

        private static bool ChildrenContains(string Id, List<BbTreeViewNode> Nodes)
        {
            bool found = false;

            foreach (BbTreeViewNode node in Nodes)
            {
                if (!found)
                {
                    if (node.Id == Id)
                        found = true;
                    else if (node.IsBranch)
                        found = ChildrenContains(Id, node.ChildNodes);
                }
            }
            return found;
        }
    }
}
