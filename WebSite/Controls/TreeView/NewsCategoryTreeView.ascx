<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsCategoryTreeView.ascx.cs" Inherits="Controls_NewsCategoryTreeView" %>

<%@ Register TagName="TreeView" TagPrefix="uc1" Src="~/Controls/TreeView/TreeView.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>

<uc1:TreeView ID="treeView" runat="server" />

<uc1:ModalPopup ID="popDeleteCategory" runat="server" Content="" AcceptButtonText="Delete"
    Title="Delete Category" CancelButtonText="Cancel" OnClientAcceptClick="deleteCategoryAcceptClick()" />

<uc1:ModalPopup ID="popAddCategory" runat="server" Content="" InputName="Category Name"
    InputMaxLength="100" Title="Create Category" AcceptButtonText="Save" CancelButtonText="Cancel"
    OnAcceptClick="AddCategoryClick" />

<uc1:ModalPopup ID="popMoveCategory" runat="server" Content="" AcceptButtonText="Move"
    Title="Move Category" CancelButtonText="Cancel" OnClientAcceptClick="confirmMove()" />

<script language="javascript" type="text/javascript">
    
    var <%=this.ID%> = new function () {

        this.SelectedId = function () {
            return treeView.SelectedId();
        }

        this.SelectedValue = function () {
            return treeView.SelectedValue();
        }
    };


    var deleteNewsCategory = function (id, name) {
        popDeleteCategory.ClearContent();
        popDeleteCategory.SetRefId(id);
        popDeleteCategory.SetContent("Are you sure you want to delete the\n<b>" + name + "</b> category?");
        popDeleteCategory.Show();
    }

    var deleteCategoryAcceptClick = function () {

        var id = popDeleteCategory.GetRefId();
        var dataraw = { "id": id };
        var data = JSON.stringify(dataraw);

        $.ajax({
            type: "POST",
            url: "<%= this.UrlNewsWebService %>/DeleteCategory",
            data: data,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (msg) {
                if (msg.d == null || msg.d == false)
                    alert("Unable to delete category");
                else
                    treeView.Remove(id);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert(xhr.status);
                //alert(thrownError);
            }
        });
    }


    var addNewsCategory = function (parentId, parentName) {
        popAddCategory.ClearContent();
        popAddCategory.SetRefId(parentId);
        popAddCategory.SetValue("");
        popAddCategory.SetContent("<span>Parent Category:</span>" + unescape(parentName));
        popAddCategory.Show();
    }


    //TO DO - move these globals to comma/pipe delimetered string or json string and pass to modalpopup value
    var destinationNode;
    var sourceNode;
    var dropPosition;

    
    function moveItem(sender, args) {

        destinationNode = args.get_destNode();
        sourceNode = args.get_sourceNode();
        dropPosition = args.get_dropPosition();

        popMoveCategory.SetContent('Are you sure you want to move ' + sourceNode.get_text() + ' category to ' + destinationNode.get_text() + ' category');
        popMoveCategory.Show();
    }


    function confirmMove() {

        var dataraw = { "source": sourceNode.get_value(), "destination" : destinationNode.get_value() };
        var data = JSON.stringify(dataraw);

          $.ajax({
            type: "POST",
            url: "<%= this.UrlNewsWebService %>/MoveCategory",
            data: data,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (msg) {
                if (msg.d == null || msg.d == false)
                    alert("Unable to move category");
                else
                {
                    $(".tvContext").hide();
                    if (dropPosition == 'over') {
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
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }


</script>

