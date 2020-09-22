using System.Collections.Generic;

namespace BusiBlocks
{
    public class BusiBlocksTreeView
    {
        public List<BusiBlocksTreeNode> Nodes;

        public BusiBlocksTreeView()
        {
            Nodes = new List<BusiBlocksTreeNode>();
        }

        public bool Contains(string Id)
        {
            bool found = false;

            foreach (BusiBlocksTreeNode node in Nodes)
            {
                if (!found)
                {
                    if (node.Id == Id)
                        found = true;
                    else if(node.IsFolder)
                        found = ChildrenContains(Id, node.ChildNodes);
                }
            }

            return found;
        }

        private static bool ChildrenContains(string Id, List<BusiBlocksTreeNode> Nodes)
        {
            bool found = false;

            foreach (BusiBlocksTreeNode node in Nodes)
            {
                if (!found)
                {
                    if (node.Id == Id)
                        found = true;
                    else if (node.IsFolder)
                        found = ChildrenContains(Id, node.ChildNodes);
                }
            }
            return found;
        }
    }

    public class BusiBlocksTreeNode
    {
        public List<BusiBlocksTreeNode> ChildNodes;
        public string Id;
        public bool IsFolder;
        public string Name;

        public BusiBlocksTreeNode()
        {
            ChildNodes = new List<BusiBlocksTreeNode>();
        }
    }
}