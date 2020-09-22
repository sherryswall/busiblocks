<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreeView.ascx.cs" Inherits="Controls_TreeView" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>

<script language="javascript" type="text/javascript">

    var radTreeView = "<%= RadTreeView1.ClientID %>";
    
    var <%=this.ID%> = new function () {

        this.prevNodeId = '';
        this.prevNodeValue = '';

        this.Remove = function (id) {
            var tree = $find(radTreeView);
            var node = tree.findNodeByValue(id);
            var parentNode = node.get_parent();
            tree.trackChanges();
            parentNode.get_nodes().remove(node);
            tree.commitChanges();
        }

        this.Select = function (id) {
            var tree = $find(radTreeView);
            var node = tree.findNodeByValue(id);
            var parentNode = node.get_parent();
            tree.trackChanges();
            parentNode.get_nodes().remove(node);
            tree.commitChanges();
        }

        
        this.SelectedId = function () {
            return this.prevNodeId;
        }

        this.SelectedValue = function () {
           return this.prevNodeValue;
        }

    };

    var treeviewNodeClicked = function (sender, args) {
        nodeName = args.get_node().get_text();
        nodeId = args.get_node().get_value();

        //hide the menu
        $(".tvContext").hide();

        //deselect previously selected node
        if (<%=this.ID%>.prevNodeId != '')
            $('#' + <%=this.ID%>.prevNodeId).fadeOut();

        //select new node
        $('#Menu' + nodeId).show();

        //set new node as selected
        <%=this.ID%>.prevNodeId = nodeId;
        <%=this.ID%>.prevNodeValue = nodeName;
    }

    function expandTree() {
        var treeView = $find(radTreeView);
        var nodes = treeView.get_allNodes();
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].get_nodes() != null) {
                nodes[i].expand();
            }
        }
    }

    function collapseTree() {
        var treeView = $find(radTreeView);
        var nodes = treeView.get_allNodes();
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].get_nodes() != null) {
                nodes[i].collapse();
            }
        }
    }

</script>
<div class="toggleTreeLinks" id="divToggleTreeLinks">
    <a href="#" onclick="expandTree();" class="expandAll">Expand All</a>&nbsp;&nbsp;
    <a href="#" onclick="collapseTree();" class="collapseAll">Collapse All</a>
</div>

<telerik:RadTreeView 
    ID="RadTreeView1" runat="server" 
    EnableDragAndDrop="false" EnableDragAndDropBetweenNodes="false" 
    EnableEmbeddedSkins="true" OnClientNodeDropping="moveItem" 
    OnClientNodeClicked="treeviewNodeClicked" OnNodeClick="Node_Clicked" 
    />
