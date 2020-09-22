<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArticleList.ascx.cs" Inherits="Controls_ArticleList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagName="ModalPopup" TagPrefix="uc1" Src="~/Controls/ModalPopup.ascx" %>
<%@ Reference VirtualPath="~/Controls/TreeView.ascx" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
    <script src="../jquery/GridUtility.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var nodeName = '';
        var nodeId = '';
        var radGrid = "<%= RadGrid1.ClientID %>";
        var radGridUniqueId = "<%= RadGrid1.UniqueID %>";
        var radAjaxManager = "<%= RadAjaxManager1.ClientID %>";
        var adminState = "<%= hidAminState.ClientID %>";
        var treeViewType = 'Category';
        var treeViewName = 'Doco';
        var isGridOnPage = true;
        var RadGrid1;

        var defaultColumns = ["View", "Date", "Author", "Type"];
        var adminColumns = ["Actions"];

        function pageLoad() {
            $find("<%= RadGrid1.ClientID %>").get_masterTableView().get_element().removeAttribute("style");
            $find("<%= RadGrid1.ClientID %>").get_masterTableView().get_element().removeAttribute("border");
            $find("<%= RadGrid1.ClientID %>").get_masterTableView().get_element().removeAttribute("cellSpacing");
        }

        function GetGridObject(sender, eventArgs) {
            RadGrid1 = sender;
            var adminStateValue = $("#" + adminState).val();

            if (adminStateValue == 1)
                ShowAdminClick(defaultColumns, adminColumns);
            else if (adminStateValue == 0)
                HideAdminClick(adminColumns, defaultColumns);
        }
    </script>
</telerik:RadScriptBlock>
<asp:HiddenField ID="hidAminState" runat="server" Value="0" />
<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <ClientEvents OnResponseEnd="ResponseEnd" />
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                <telerik:AjaxUpdatedControl ControlID="tree1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadGrid ID="RadGrid1" runat="server" OnItemDataBound="RadGrid1_ItemDataBound"
    OnNeedDataSource="RadGrid1_NeedDataSource" AllowFilteringByColumn="true" EnableEmbeddedSkins="true"
    Skin="" OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged"
    FilterItemStyle-CssClass="radGridFilterRow" GroupingSettings-CaseSensitive="false"
    EnableLinqExpressions="false" OnPageIndexChanged="RadGrid1_PageIndexChanged" AlternatingItemStyle-CssClass="alternate">
    <MasterTableView CssClass="standardTable" RetrieveAllDataFields="false" UseAllDataFields="false"
        AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true" Width="100%"
        OnPreRender="RadGrid1_PreRender" AllowNaturalSort="false" PageSize="10" NoMasterRecordsText="No documents to display">
        <RowIndicatorColumn SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
            SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
        </RowIndicatorColumn>
        <Columns>
            <telerik:GridBoundColumn DataField="Category.Id" UniqueName="CategoryId" Display="false" />
            <telerik:GridBoundColumn DataField="Deleted" UniqueName="Deleted" Display="false" />
            <telerik:GridTemplateColumn HeaderText="TITLE" UniqueName="Title" DataField="Title"
                SortExpression="Title" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:HyperLink NavigateUrl='<%# GetViewArticleUrl( (string)Eval("Id"),"pub" ) %>'
                        ID="lnkArticle" runat="server"><%# Eval("Title") %></asp:HyperLink>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="CATEGORY" UniqueName="Category" SortExpression="Category.DisplayName"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                DataField="Category.DisplayName" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" >
                <ItemTemplate>
                    <asp:HyperLink runat="server" ID="lnkCategory" Alt='<%#Eval("Category.DisplayName") %>'><%#Eval("Category.DisplayName") %></asp:HyperLink>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="STATUS" UniqueName="View" SortExpression="TrafficLight"
                DataField="TrafficLight" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:Image CssClass="center" runat="server" ID="imgAck" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="MODIFIED" DataField="UpdateDate" UniqueName="Date"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="OWNER" DataField="Author" UniqueName="Author"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
            </telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="TYPE" UniqueName="Type" DataField="DocumentType"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortExpression="DocumentType" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:Image CssClass="center" runat="server" ID="imgType" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn UniqueName="Show" AllowFiltering="false" ItemStyle-CssClass="showAdminColumn"
                HeaderStyle-CssClass="showAdminColumn" FooterStyle-CssClass="showAdminColumn">
                <ItemTemplate>
                    <div class="showAdmin" onclick="ShowAdminClick(defaultColumns, adminColumns);">
                        <span>S<br />
                            H<br />
                            O<br />
                            W<br />
                            <br />
                            A<br />
                            D<br />
                            M<br />
                            I<br />
                            N </span>
                    </div>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn UniqueName="Hide" AllowFiltering="false" Display="false"
                ItemStyle-CssClass="hideAdminColumn" HeaderStyle-CssClass="hideAdminColumn" FooterStyle-CssClass="hideAdminColumn">
                <ItemTemplate>
                    <div class="hideAdmin" onclick="HideAdminClick(adminColumns,defaultColumns);">
                        <span>H<br />
                            I<br />
                            D<br />
                            E<br />
                            <br />
                            A<br />
                            D<br />
                            M<br />
                            I<br />
                            N </span>
                    </div>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="ACTIONS" UniqueName="Actions" AllowFiltering="false"
                ItemStyle-CssClass="adminCells" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" Display="false">
                <ItemTemplate>
                    <asp:LinkButton CssClass="edit" ID="imgBtnEdit" runat="server" CommandName="Edt">Edit</asp:LinkButton>
                    <%--<asp:LinkButton CssClass="checkIn" ID="lnkBtnCheckIn" runat="server" CommandName="CheckIn">Check In</asp:LinkButton>--%>
                    <asp:LinkButton CssClass="deleteitem" ID="imgBtnDel" runat="server" OnClientClick="javascript:DeleteClick(this); return false;"
                        CommandName="Delete" CommandArgument='<%# Eval("Id") %>'>Delete</asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
        PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
        NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
    <ClientSettings>
        <ClientEvents OnGridCreated="GetGridObject"></ClientEvents>
    </ClientSettings>
</telerik:RadGrid>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script language="javascript" type="text/javascript">
        var gridDiv = document.getElementById('<%=RadGrid1.ClientID %>');
        checkEditAccess(gridDiv, 'ViewCategory', 'Document');
        function ResponseEnd() {
            checkEditAccess(gridDiv, 'ViewCategory', 'Document');
        }
    </script>
</telerik:RadCodeBlock>
<uc1:ModalPopup ID="pupDeleteItem" runat="server" OnClientAcceptClick="deleteItem()"
    AcceptButtonText="Delete" Content="Are you sure you want to delete this item?"
    CancelButtonText="Cancel" Width="275px" Title="Delete?" Height="100px" />
<script language="javascript" type="text/javascript">

    var clickedButton;

    var DeleteClick = function (button) {
        pupDeleteItem.Show();
        clickedButton = button;
    }

    var deleteItem = function () {
        var code = clickedButton.toString();
        code = code.replace("javascript:", "")
        code = decodeURIComponent(code);
        eval(code);

    }
</script>
