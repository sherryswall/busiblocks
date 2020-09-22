<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsListRead.ascx.cs"
    Inherits="Communication_NewsListRead" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadScriptBlock runat="server" ID="radscriptblock">
    <script type="text/javascript" language="javascript">
        var radGridRead = "<%= RadGridRead.ClientID %>";
    </script>
</telerik:RadScriptBlock>
<telerik:RadAjaxManagerProxy ID="RadAjaxProxyRead" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGridRead">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGridRead" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>
<telerik:RadGrid ID="RadGridRead" runat="server" EnableEmbeddedSkins="true" Skin=""
    AllowFilteringByColumn="True" OnItemCommand="RadGrid1ItemCommand" OnItemDataBound="RadGrid1_ItemDataBound"
    OnNeedDataSource="RadGrid1_NeedDataSource" GroupingSettings-CaseSensitive="false"
    EnableLinqExpressions="false" DataMember="NewsGridItem" AlternatingItemStyle-CssClass="alternate">
    <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
        CssClass="standardTable" AllowSorting="true" AllowPaging="true" FilterItemStyle-CssClass="radGridFilterRow"
        PageSize="10" BorderStyle="none" AllowNaturalSort="false" NoMasterRecordsText="No announcements to display">
        <Columns>
            <telerik:GridHyperLinkColumn DataTextField="NewsItem.Title" AllowFiltering="true"
                HeaderText="TITLE" UniqueName="Title" SortExpression="NewsItem.Title" ShowFilterIcon="false"
                CurrentFilterFunction="Contains" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" AutoPostBackOnFilter="true"
                DataType="System.String">
            </telerik:GridHyperLinkColumn>
            <telerik:GridBoundColumn HeaderText="CATEGORY" DataField="NewsItem.Category.Name"
                UniqueName="Category" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="STATUS" DataField="TrafficLightUrl" UniqueName="Status"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
            <telerik:GridTemplateColumn HeaderText="VERSION" DataField="Draft.VersionNumber"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <%#Eval("Draft.VersionNumber").ToString().Substring(0,Eval("Draft.VersionNumber").ToString().LastIndexOf(".")) %>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="PUBLISHED" DataField="NewsItem.UpdateDate" UniqueName="Published"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png"
                DataType="System.DateTime" DataFormatString="{0:dd/MM/yy - HH:mm}" />
            <telerik:GridBoundColumn HeaderText="OWNER" DataField="NewsItem.Owner" UniqueName="Owner"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                SortAscImageUrl="~/App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="~/App_Themes/Default/icons/sortDescending.png" />
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="~/App_Themes/Default/icons/pageFirst.png"
        PrevPageImageUrl="~/App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="~/App_Themes/Default/icons/pageLast.png"
        NextPageImageUrl="~/App_Themes/Default/icons/pageNext.png" />
</telerik:RadGrid>
