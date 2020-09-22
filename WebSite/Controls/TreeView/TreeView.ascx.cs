using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using BusiBlocks;
using Telerik.Web.UI;

public partial class Controls_TreeView : UserControl
{
    public string OnClientNodeClicked { get; set; }
    
    public event EventHandler OnNodeClicked;

    private const string JavascriptEventArgs = "(sender, args)";

    protected void Page_Load(object sender, EventArgs e)
    {
        BindClientEvents();

        if (!IsPostBack)
        {
            BindServerEvents();
        }
    }

    public void BindEvents()
    {
        BindClientEvents();
        BindServerEvents();
    }

    protected void Node_Clicked(object sender, RadTreeNodeEventArgs e)
    {
        Selected = e.Node.Value;

        if(OnNodeClicked != null)
            OnNodeClicked.Invoke(this, new EventArgs());
    }

    public void BindServerEvents()
    {
        if (OnNodeClicked == null)
            RadTreeView1.NodeClick -= Node_Clicked;
        else
            RadTreeView1.NodeClick += Node_Clicked;
    }


    private void ExpandRootNodes()
    {
        foreach (RadTreeNode n in RadTreeView1.Nodes)
            n.Expanded = true;
    }

    public bool EnableDragAndDrop
    {
        set { RadTreeView1.EnableDragAndDrop = value; }
    }

    private string selected;
    public string Selected
    {
        set
        {
            selected = value;
            if (RadTreeView1.FindNodeByValue(value) != null)
            {
                RadTreeView1.FindNodeByValue(value).Selected = true;
                RadTreeView1.FindNodeByValue(value).ExpandParentNodes();
            }
        }
        get
        {
            if (RadTreeView1.SelectedNode != null)
                if (!string.IsNullOrEmpty(RadTreeView1.SelectedNode.Value))
                    return RadTreeView1.SelectedNode.Value;
           
            if(!string.IsNullOrEmpty(selected))
                return selected; 

            return string.Empty;            
        }
    }


    public string SelectedValue
    {
        get { return RadTreeView1.SelectedNode.Value; }
    }

    private void BindClientEvents()
    {
        if (!string.IsNullOrEmpty(OnClientNodeClicked))
        {
            //The TreeView takes a 'OnClientNodeClicked' property, however so does the RadTreeView
            //The RadTreeView only accepts a single javascript function, so two make this work
            //we must make a single function, that calls these two functions one by one

            string preventPostBack = (OnNodeClicked == null ? "return false;" : string.Empty);

            //declare the function that will call the two functions
            string javascriptOnNodeClick = String.Format(@"function OnNodeClickCustom{0} {{ {1}{0}; {2}{0}; {3}}}", JavascriptEventArgs, RadTreeView1.OnClientNodeClicked, OnClientNodeClicked, preventPostBack);

            //register the script with the page (push it to the html)
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "OnNodeClickCustom()", javascriptOnNodeClick, true);

            //assign new function to the TreeViewRad onclick event
            RadTreeView1.OnClientNodeClicked = "OnNodeClickCustom";
        }
    }

    public void BindTree(BbTreeView bbTreeView)
    {
        string selectedValue = ((!string.IsNullOrEmpty(selected)) ? selected : string.Empty);

        RemoveNodes(RadTreeView1.Nodes);

        if (bbTreeView != null && bbTreeView.Nodes.Count > 0)
        {
            foreach (BbTreeViewNode rootNode in bbTreeView.Nodes)
            {
                RadTreeNode radParentNode = new RadTreeNode(rootNode.Value + rootNode.Menu, rootNode.Id) { ImageUrl = rootNode.Icon };

                RadTreeView1.Nodes.Add(radParentNode);

                if (rootNode.IsBranch)
                    AddChildNodes(rootNode, radParentNode);
            }
        }

        Selected = selectedValue;
        
        ExpandRootNodes();
    }


    private void AddChildNodes(BbTreeViewNode rootNode, RadTreeNode rootNodeRad)
    {
        foreach (BbTreeViewNode childNode in rootNode.ChildNodes)
        {
            RadTreeNode radChildNode = new RadTreeNode(childNode.Value + childNode.Menu, childNode.Id) { ImageUrl = childNode.Icon };
            
            RadTreeView1.FindNodeByValue(rootNodeRad.Value).Nodes.Add(radChildNode);

            if (childNode.IsBranch)
                AddChildNodes(childNode, radChildNode);
        }
    }

    private static void RemoveNodes(RadTreeNodeCollection nodes)
    {
        while (nodes.Count > 0)
        {
            RemoveNodes(nodes[0].Nodes);
            nodes.Remove(nodes[0]);
        }
    }
}



