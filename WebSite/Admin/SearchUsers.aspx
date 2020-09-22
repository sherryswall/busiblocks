<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SearchUsers.aspx.cs"
    Inherits="Admin_SearchUsers" Title="Manage Users" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<%@ Register TagPrefix="pd" TagName="PersonalDetails" Src="~/Controls/PersonalDetailsView.ascx" %>
<asp:Content ContentPlaceHolderID="HeadContent" ID="Content2" runat="server" type="text/javascript">
    <script src="../jquery/DocoBlock.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var pdUser = "<%=pdUser.ClientID %>";
        function button_click(tb, buttonId) {
            if (window.event.keyCode == 13) {
                document.getElementById(buttonId).focus();
                document.getElementById(buttonId).click();
            }
        }
        function showDeleteUserPopup(id, name) {
            popDeleteUser.ClearContent();
            popDeleteUser.SetRefId(id);
            popDeleteUser.SetContent("Are you sure you want to delete the\n" + name + " user?");
            popDeleteUser.Show();
        }
        function showPopup(id) {
            pdUser.Load(id);
            popUserDetails.Show();
        } 
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <h1 class="sectionhead" id="user">
        Manage Users</h1>
    <div>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <ClientEvents OnResponseEnd="refreshGrid" />
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
            AllowFilteringByColumn="True" OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged"
            OnNeedDataSource="RadGrid1_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
            AlternatingItemStyle-CssClass="alternate" OnItemDataBound="RadGrid1_ItemDataBound"
            GroupingSettings-CaseSensitive="false" EnableLinqExpressions="false">
            <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                AllowPaging="true" CssClass="standardTable" PageSize="10" AllowNaturalSort="false"
                AllowSorting="true" NoMasterRecordsText="No records to display">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="USER NAME" UniqueName="LoginId" DataField="User.Name"
                        SortExpression="User.Name" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                        <ItemTemplate>
                            <a href='#' onclick="showPopup('<%# Eval("User.Name") %>');">
                                <%# Eval("User.Name") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="LAST NAME" DataField="Person.LastName" UniqueName="FirstName"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="FIRST NAME" DataField="Person.FirstName" UniqueName="Email"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="WORK EMAIL" DataField="Person.Email" UniqueName="Phone"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="SITES" DataField="PrimarySite.Name" UniqueName="PrimarySite"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortExpression="PrimarySite.Name" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                        SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPrimarySite" CssClass="bold"></asp:Label>
                            <asp:HiddenField runat="server" ID="hfPersonId" Value='<%# Eval("Person.Id") %>' />
                            <asp:Label runat="server" ID="lblSecondarySites"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="ACTIONS" UniqueName="Actions" AllowFiltering="false"
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                        <ItemTemplate>
                            <asp:LinkButton CssClass="edit" ID="lnkBtnEdit" runat="server" CommandName="edit"
                                CommandArgument='<%# Eval("Person.Id") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton CssClass="deleteitem" ID="lnkBtnDelete" runat="server" CommandName="delete"
                                CommandArgument='<%# Eval("Person.Id") %>'>Delete</asp:LinkButton>
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
                //directly call creation row since the non-admin staff won't have access to manage users page.
                createCreationRow(gridDiv, '../Admin/CreateUser.aspx', 'User');
                function refreshGrid() {
                    createCreationRow(gridDiv, '../Admin/CreateUser.aspx', 'User');
                }
            </script>
        </telerik:RadCodeBlock>
    </div>
    <uc1:ModalPopup ID="popDeleteUser" runat="server" Content="" AcceptButtonText="Delete"
        Title="Delete User" CancelButtonText="Cancel" OnAcceptClick="DeleteUserClick" />
    <uc1:ModalPopup ID="popUserDetails" runat="server" CancelButtonText="Cancel">
        <FormTemplateContainer>
            <pd:PersonalDetails runat="server" ID="pdUser" />
        </FormTemplateContainer>
    </uc1:ModalPopup>
</asp:Content>
