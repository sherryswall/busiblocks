<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DirectorySearch.aspx.cs"
    Inherits="Directory_DirectorySearch" Title="Directory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ContentPlaceHolderID="HeadContent" ID="Content2" runat="server" type="text/javascript">
    <script type="text/javascript" language="javascript">

        function button_click(tb, buttonId) {
            if (window.event.keyCode == 13) {
                document.getElementById(buttonId).focus();
                document.getElementById(buttonId).click();
            }
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <h1 class="sectionhead" id="directory">
        Directory</h1>
    <div>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="RadGrid2">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid2" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <ajax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
            <ajax:TabPanel ID="TabPanelPersonalDetails" runat="server" HeaderText="Search Users">
                <ContentTemplate>
                    <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
                        AllowFilteringByColumn="True" OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged"
                        OnNeedDataSource="RadGrid1_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
                        GroupingSettings-CaseSensitive="false" AlternatingItemStyle-CssClass="alternate">
                        <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                            AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10"
                            AllowNaturalSort="false" NoMasterRecordsText="No records to display">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderText="USER NAME" UniqueName="LoginId" DataField="User.Name"
                                    SortExpression="User.Name" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                    ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                                    SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="imgBtnView" runat="server" CommandName="view" CommandArgument='<%# Eval("Person.Id") %>'><%# Eval("User.Name") %></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="LAST NAME" DataField="Person.LastName" UniqueName="LastName"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="FIRST NAME" DataField="Person.FirstName" UniqueName="FirstName"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="EMAIL" DataField="Person.Email" UniqueName="Email"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="PHONE" DataField="Person.WorkPhone" UniqueName="Phone"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="PRIMARY SITE" DataField="PrimarySite.Name" UniqueName="PrimarySite"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="POSITION" DataField="Person.Position" UniqueName="Position"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
                            PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
                            NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
                    </telerik:RadGrid>
                </ContentTemplate>
            </ajax:TabPanel>
            <ajax:TabPanel ID="TabPanelSiteDetails" runat="server" HeaderText="Search Sites">
                <ContentTemplate>
                    <telerik:RadGrid ID="RadGrid2" runat="server" EnableEmbeddedSkins="true" Skin=""
                        AllowFilteringByColumn="True" OnItemCommand="RadGrid2_ItemCommand" OnPageSizeChanged="RadGrid2_PageSizeChanged"
                        OnNeedDataSource="RadGrid2_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
                        GroupingSettings-CaseSensitive="false" AlternatingItemStyle-CssClass="alternate">
                        <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                            AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10"
                            AllowNaturalSort="false" NoMasterRecordsText="No records to display">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderText="SITE NAME" UniqueName="Name" SortExpression="Name"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    DataField="Name" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                                    SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="imgBtnSiteView" runat="server" CommandName="view" CommandArgument='<%# Eval("Id") %>'><%# Eval("Name") %></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="PARENT REGION" DataField="Region.Name" UniqueName="RegionName"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="TYPE" DataField="SiteType.Name" UniqueName="SiteType"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="PHONE" DataField="AltPhoneNumber" UniqueName="Phone"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="EMAIL" DataField="Email" UniqueName="Email"
                                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
        PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
        NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
                    </telerik:RadGrid>
                </ContentTemplate>
            </ajax:TabPanel>
        </ajax:TabContainer>
    </div>
</asp:Content>
