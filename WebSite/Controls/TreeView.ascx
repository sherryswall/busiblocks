<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TreeView.ascx.cs" Inherits="Controls_TreeView" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script src="../jquery/TreeView.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var prevNodeId = '';
        var radTreeView = "<%= RadTreeView1.ClientID %>";
        var isNodeDblClick = false;
        var destinationNode;
        var sourceNode;
        var dropPosition = '';

        function moveItem(sender, args) {
            destinationNode = args.get_destNode();
            sourceNode = args.get_sourceNode();
            dropPosition = args.get_dropPosition();

            var type = '';
            switch (treeViewType) {
                case 'Region':
                    type = 'Region';
                    break;
                case 'Category':
                    type = 'Category';
                    break;
                default:
                    break;
            }
            popMoveCategory.SetContent('Are you sure you want to move ' + sourceNode.get_text() + ' ' + type + ' to ' + destinationNode.get_text() + " " + type);
            popMoveCategory.Show();
        }

        function confirmMove() {
            if (dropPosition == 'over') {
                if (treeViewType == 'Category') {//perform server/WS operation
                    WSTreeView.MoveCategory(sourceNode.get_text(), destinationNode.get_text(), treeViewName);
                }
                else if (treeViewType == 'Region') {
                    WSTreeView.MoveRegion(sourceNode.get_text(), destinationNode.get_text());
                }
                //perform client side changes!.
                var tree = $find(radTreeView);

                //get the node's html
                var menu = sourceNode.get_contentElement();
                var tempNode = sourceNode;

                //track changes on treeview...
                tree.trackChanges();

                sourceNode.get_parent().get_nodes().remove(tempNode);

                var node = new Telerik.Web.UI.RadTreeNode();
                destinationNode.get_nodes().add(tempNode);
                var contentElement = tempNode.get_contentElement();
                //Update its html with the template
                contentElement.innerHTML = menu.innerHTML;

                destinationNode.set_expanded(true);

                tree.commitChanges();
            }
        }
        function showAddCategoryPopup(functionName, id, name, parent, action) {
            popAddCategory.ClearContent();
            popAddCategory.SetRefId(id);
            popAddCategory.SetValue("");
            popAddCategory.SetContent("<span>Parent Category:</span>" + unescape(parent));
            popAddCategory.Show();
        }
        function showDeleteCategoryPopup(functionName, id, name, parent, action) {
            popDeleteCategory.ClearContent();
            popDeleteCategory.SetRefId(id);
            popDeleteCategory.SetContent("Are you sure you want to delete the\n<b>" + nodeName + "</b> category?");
            popDeleteCategory.Show();
        }
        function showEditCategoryPopup(functionName, id, name, parent, action) {
            popEditCategory.ClearContent();
            popEditCategory.SetContent("Parent Category\t" + unescape(parent));
            popEditCategory.SetValue(nodeName);
            popEditCategory.SetRefId(id);
            popEditCategory.Show();
        }
        function showAddRegionPopup(functionName, id, name, parent, action) {
            popAddRegion.ClearContent();
            popAddRegion.SetRefId(id);
            popAddRegion.SetValue("");
            popAddRegion.SetContent("<span>Parent Region:</span>" + unescape(nodeName));
            popAddRegion.Show();
        }
        function showDeleteRegionPopup(functionName, id, name, parent, action) {
            popDeleteRegion.ClearContent();
            popDeleteRegion.SetRefId(id);
            popDeleteRegion.SetContent("Are you sure you want to delete the\n<b>" + unescape(name) + "</b> region?");
            popDeleteRegion.Show();
        }
        function showEditRegionPopup(functionName, id, name, parent, action) {
            popEditRegion.ClearContent();
            popEditRegion.SetValue(nodeName);
            popEditRegion.SetRefId(id);
            popEditRegion.Show();
        }
        function showAddSitePopup(functionName, id, name, parent, action) {
            popAddSite.ClearContent();
            popAddSite.SetValue("");
            popAddSite.SetRefId(id);
            popAddSite.SetContent("<span>Region:</span>" + unescape(name));
            popAddSite.Show();
        }

        function editRegionClick() {
            var id = popEditRegion.GetRefId();
            var name = popEditRegion.Value();

            var nameAvailable

            var dataraw = { "Id": id, "name": name };
            var data = JSON.stringify(dataraw);
            
            var urlRegionNameAvailable = "<%= GetRegionNameAvailableUrl() %>";

            $.ajax({
                url: urlRegionNameAvailable,
                type: "POST",
                dataType: "json",
                contentType: "application/json;charset=utf-8",
                data: data,
                success: function (msg) {
                    if (msg.d) {
                        WSTreeView.EditRegion(id, name);
                        var treeView = $find(radTreeView);

                        var node = treeView.findNodeByText(nodeName);
                        treeView.trackChanges();

                        var menu = node.get_contentElement();
                        menu.innerHTML = menu.innerHTML.replace(nodeName, name);
                        node.get_textElement().innerHTML = menu.innerHTML;
                        treeView.commitChanges();
                        treeView.get_selectedNode().set_text(name);
                        nodeName = name;
                    }
                    else {
                        popNameAlreadyTaken.Show();
                    }
                }
            });


        }
        function deleteRegionClick() {
            var id = popDeleteRegion.GetRefId();
            var isDeleted = WSTreeView.DeleteRegion(id);
            if (isDeleted) {
                var tree = $find(radTreeView);
                var node = tree.get_selectedNode();
                var parentNode = node.get_parent();
                tree.trackChanges();
                parentNode.get_nodes().remove(node);
                tree.commitChanges();
                nodeName = '';
                // todo the following code doesn't reset the filter expression
                if (!(typeof radGrid === 'undefined')) {
                    var masterTableView = $find(radGrid).get_masterTableView();
                    masterTableView.filter('RegionName', '', 17);
                }
            }
        }
        function deleteCategoryClick() {
            var id = popDeleteCategory.GetRefId();
            WSTreeView.DeleteCategory(id, treeViewName);
            var tree = $find(radTreeView);
            var node = tree.findNodeByText(nodeName);
            var parentNode = node.get_parent();
            tree.trackChanges();
            parentNode.get_nodes().remove(node);
            tree.commitChanges();
            nodeName = '';
        }
        function editCategoryClick() {
            var id = popEditCategory.GetRefId();
            var name = popEditCategory.Value();

            WSTreeView.EditCategory(id, name, treeViewName);
            var tree = $find(radTreeView);
            var node = tree.findNodeByText(nodeName);

            tree.trackChanges();

            var menu = node.get_contentElement();
            menu.innerHTML = menu.innerHTML.replace(nodeName, name);
            node.get_textElement().innerHTML = menu.innerHTML;

            tree.commitChanges();
            tree.get_selectedNode().set_text(name);

            nodeName = name;
        }
        function GetSelectedNode() {
            var treeView = $find(radTreeView);
            var node = treeView.get_selectedNode();
            return node;
        }

        $(document).ready(function () {
            var treeView = $find(radTreeView);
            var nodeCount = 0;
            if (treeView != null) {
                var nodes = treeView.get_allNodes();
                for (var i = 0; i < nodes.length; i++) {
                    nodeCount++;
                }
            }

            if (nodeCount == 0) {
                $("#divToggleTreeLinks").hide();
                if ($("#h2RegionStructure") != null)
                    $("#h2RegionStructure").hide();
                if ($(".createRowTR") != null)
                    $(".createRowTR").hide();
            }
        });

    </script>
</telerik:RadCodeBlock>
<div class="toggleTreeLinks" id="divToggleTreeLinks">
    <a href="#" onclick="expandTree();" class="expandAll">Expand All</a>&nbsp;&nbsp;
    <a href="#" onclick="collapseTree();" class="collapseAll">Collapse All</a>
</div>
<telerik:RadTreeView ID="RadTreeView1" runat="server" EnableDragAndDrop="false" EnableDragAndDropBetweenNodes="false" 
    OnClientNodeClicked="nodeClicked" OnClientDoubleClick="nodeDblClicked" OnClientNodeDropping="moveItem"
    EnableEmbeddedSkins="true"  >
</telerik:RadTreeView>

<uc1:ModalPopup ID="popAddCategory" runat="server" Content="" InputName="Category Name" InputMaxLength="100"
    Title="Create Category" AcceptButtonText="Save" CancelButtonText="Cancel" OnAcceptClick="AddCategoryClick" />
<uc1:ModalPopup ID="popDeleteCategory" runat="server" Content="" AcceptButtonText="Delete"
    Title="Delete Category" CancelButtonText="Cancel" OnAcceptClick="DeleteCategoryClick" />
<uc1:ModalPopup ID="popEditCategory" runat="server" Content="" InputName="Category Name" InputMaxLength="100"
    Title="Edit Category" AcceptButtonText="Save" CancelButtonText="Cancel" OnClientAcceptClick="editCategoryClick()" />
<uc1:ModalPopup ID="popMoveCategory" runat="server" Content="" AcceptButtonText="Move"
    Title="Move Category" CancelButtonText="Cancel" OnClientAcceptClick="confirmMove()" />
<uc1:ModalPopup ID="popAddRegion" runat="server" Content="" InputName="Region Name" InputMaxLength="100"
    Title="Create Region" AcceptButtonText="Save" CancelButtonText="Cancel" OnAcceptClick="AddRegionClick" />
<uc1:ModalPopup ID="popDeleteRegion" runat="server" Content="" AcceptButtonText="Delete"
    Title="Delete Region" CancelButtonText="Cancel" OnAcceptClick="DeleteRegionClick" />
<uc1:ModalPopup ID="popEditRegion" runat="server" Content="" InputName="Region Name" InputMaxLength="100"
    Title="Edit Region" AcceptButtonText="Save" CancelButtonText="Cancel" OnClientAcceptClick="editRegionClick()" />
<uc1:ModalPopup ID="popAddSite" runat="server" Content="" InputName="Site Name" InputMaxLength="100" AcceptButtonText="Add"
    Title="Create Site" CancelButtonText="Cancel" OnAcceptClick="AddSiteClick" />
<uc1:ModalPopup ID="popNameAlreadyTaken" runat="server" Title="That region name is already in use, please choose another."
    AcceptButtonText="Ok" OnClientAcceptClick="popEditRegion.Show()" />
