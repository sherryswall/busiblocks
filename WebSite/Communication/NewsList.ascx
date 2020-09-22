<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsList.ascx.cs" Inherits="Controls_NewsList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock1">
    <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var radGrid = "<%= RadGrid1.ClientID %>";        
    </script>
</telerik:RadCodeBlock>
<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <ClientEvents OnResponseEnd="ResponseEnd" />
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
    AllowFilteringByColumn="True" OnItemCommand="RadGrid1ItemCommand" OnPageSizeChanged="RadGrid1PageSizeChanged"
    OnItemDataBound="RadGrid1_ItemDataBound" OnNeedDataSource="RadGrid1_NeedDataSource"
    FilterItemStyle-CssClass="radGridFilterRow" GroupingSettings-CaseSensitive="false">
    <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
        CssClass="standardTable" AllowSorting="true" AllowPaging="true" Width="100%" AllowNaturalSort="false"
        PageSize="10" BorderStyle="none" NoMasterRecordsText="No announcements to display">
        <Columns>
            <telerik:GridTemplateColumn HeaderText="TITLE" UniqueName="Title" SortExpression="Item.Title"
                DataField="Item.Title" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:LinkButton runat="server" itemid='<%#Eval("Item.Id") %>' CssClass="viewitem"
                        OnClientClick='javascript:ActionOnClick(this); return false;'>
                        <%# Eval("Item.Title") %>
                    </asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="AUTHOR" DataField="Item.Author" UniqueName="Author"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="CATEGORY" DataField="Item.Category.Name" UniqueName="Category"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="MODIFIED" DataField="Item.UpdateDate" UniqueName="Date"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="STATUS" DataField="TrafficLightUrl" UniqueName="Status"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridBoundColumn HeaderText="ID" DataField="Item.Id" UniqueName="Id" Display="false"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" />
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" />
</telerik:RadGrid>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script language="javascript" type="text/javascript">
        var gridDiv = document.getElementById('<%=RadGrid1.ClientID %>');
        checkEditAccess(gridDiv, 'NewsViewCategory', 'Announcement');

        function ResponseEnd() {
            checkEditAccess(gridDiv, 'NewsViewCategory', 'Announcement');
        }

        function ActionOnClick(Item) {
            var ItemId = Item.getAttribute("itemid");
            var IdPlaceholder = "<%=this.IdPlaceholder%>";
            var url = "<%=this.NavNewsItemView()%>";
            url = url.replace(IdPlaceholder, ItemId);
            window.location = url;
        }
    </script>
</telerik:RadCodeBlock>
