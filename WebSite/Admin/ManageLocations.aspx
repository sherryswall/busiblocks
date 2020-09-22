<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ManageLocations.aspx.cs" Inherits="Admin_tree" %>

<%@ Register TagName="ModalPopup" TagPrefix="uc1" Src="~/Controls/ModalPopup.ascx" %>
<%@ Register TagPrefix="tree" TagName="TreeView" Src="~/Controls/TreeView.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
    <telerik:RadCodeBlock runat="server" ID="radCodeBlock1">
        <script type="text/javascript" language="javascript">
            var nodeName = '';
            var nodeId = '';
            var radGrid = "<%= RadGrid1.ClientID %>";
            var radGridUniqueId = "<%= RadGrid1.UniqueID %>";
            var radAjaxManager = "<%= RadAjaxManager1.ClientID %>";
            var treeViewType = 'Region';
            var treeViewName = '';
            var isGridOnPage = true;
        </script>
    </telerik:RadCodeBlock>
    <h1 id="managereg" class="sectionhead">
        Manage Locations</h1>
    <h2 id="h2RegionStructure">
        Region Structure</h2>
    <tree:TreeView runat="server" ID="tree1" />
    <h2 class="sectionhead" id="sites">
        Sites:&nbsp;<label id="lblSiteName">All</label></h2>
    <div>
        <!-- Query result -->
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <ClientEvents OnResponseEnd="refreshGrid" />
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                        <telerik:AjaxUpdatedControl ControlID="tree1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>          
        </telerik:RadAjaxManager>
        <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
            AllowFilteringByColumn="True" OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged"
            OnNeedDataSource="RadGrid1_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
            GroupingSettings-CaseSensitive="false" EnableLinqExpressions="false" OnItemDataBound="RadGrid1_ItemDataBound" AlternatingItemStyle-CssClass="alternate">
            <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                NoMasterRecordsText="No records to display" AllowSorting="true" AllowPaging="true"
                CssClass="standardTable" PageSize="10" AllowNaturalSort="false" OnPreRender="RadGrid1_PreRender">
                <Columns>
                    <telerik:GridBoundColumn DataField="Deleted" UniqueName="Deleted" Visible="false" />
                    <telerik:GridBoundColumn HeaderText="SITE" DataField="Name" UniqueName="Name" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="REGION" DataField="Region.Name" UniqueName="RegionName"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="PARENT REGION" DataField="Region.ParentRegion.Name"
                        UniqueName="ParentRegion" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="ACTIONS" UniqueName="Actions" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        AllowFiltering="false" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                        <ItemTemplate>
                            <asp:LinkButton CssClass="edit" ID="lnkBtnEdit" runat="server" CommandName="edit"
                                CommandArgument='<%# Eval("Id") %>' Text="Edit" />
                            <asp:LinkButton CssClass="deleteitem" ID="lnkBtnDelete" runat="server" CommandName="delete"
                                CommandArgument='<%# Eval("Id") %>' OnClientClick="DeleteClick(this); return false;"
                                Text="Delete" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
                PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
                NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
        </telerik:RadGrid>
        <telerik:RadCodeBlock runat="server" ID="radCodeBlock">
            <script language="javascript" type="text/javascript">
                var gridDiv = document.getElementById('<%=RadGrid1.ClientID %>');
                createCreationRow(gridDiv, '<%=NewLinkUrl%>', 'Site');
                function refreshGrid() {
                    createCreationRow(gridDiv, '<%=NewLinkUrl%>', 'Site');
                }
            </script>
        </telerik:RadCodeBlock>
    </div>
    <uc1:ModalPopup ID="pupDeleteItem" runat="server" OnClientAcceptClick="deleteItem()"
        AcceptButtonText="Delete" Content="Are you sure you want to delete this site?"
        CancelButtonText="Cancel" Width="275px" Title="Delete?" Height="100px" />
    <script language="javascript" type="text/javascript">

        var clickedButton;

        var DeleteClick = function (button) {
            pupDeleteItem.Show();
            clickedButton = button;
        }

        var deleteItem = function () {
            var code = clickedButton.toString();
            code = code.replace("javascript:", "");
            //code = code.replace("lnkBtnDelete", "lnkBtnDeleteServer");
            code = decodeURIComponent(code);
            eval(code);
        }
    </script>
</asp:Content>
