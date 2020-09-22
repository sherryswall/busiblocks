<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VersionHistory.ascx.cs"
    Inherits="Controls_VersionHistory" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="versionListDetails">
    <span>Announcement Title:
        <asp:Label runat="server" class="lblTitle" ID="lblTitle"></asp:Label>
    </span><span>Author:
        <asp:Label runat="server" class="lblAuthor" ID="lblAuthor"></asp:Label>
    </span><span>Category:
        <asp:Label runat="server" class="lblCategory" ID="lblCategory"></asp:Label>
    </span>
</div>
<br />
<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadGrid ID="RadGrid1" runat="server" OnItemDataBound="RadGrid1_ItemDataBound"
    OnNeedDataSource="RadGrid1_NeedDataSource" AllowFilteringByColumn="true" EnableEmbeddedSkins="true"
    Skin="" OnPageSizeChanged="RadGrid1_PageSizeChanged" FilterItemStyle-CssClass="radGridFilterRow"
    GroupingSettings-CaseSensitive="false" EnableLinqExpressions="false">
    <MasterTableView CssClass="standardTable" RetrieveAllDataFields="false" UseAllDataFields="false"
        AutoGenerateColumns="false" AllowSorting="true" AllowPaging="true" Width="100%"
        AllowNaturalSort="false" PageSize="10" NoMasterRecordsText="No records to display">
        <RowIndicatorColumn SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
            SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
        </RowIndicatorColumn>
        <Columns>
            <telerik:GridBoundColumn DataField="ItemId" UniqueName="ItemId" Display="false" />
            <telerik:GridTemplateColumn HeaderText="VERSION" UniqueName="Version" DataField="Version"
                SortExpression="Title" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:HyperLink NavigateUrl='<%#GetViewVersionUrl((string)Eval("Id")) %>' ID="lnkVersion"
                        runat="server"><%# Eval("VersionNumber") %></asp:HyperLink>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="DATE" UniqueName="DateCreated" SortExpression="DateCreated"
                AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                DataField="DateCreated" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png" DataFormatString="{0:dd/MM/yy - HH:mm}">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="MODIFIED BY" UniqueName="ModifiedBy" SortExpression="ModifiedBy"
                DataField="ModifiedBy" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="COMMENTS" UniqueName="Comments" SortExpression="Comments"
                DataField="Comments" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
        PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
        NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
</telerik:RadGrid>