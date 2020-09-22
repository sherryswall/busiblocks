<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UsersList.ascx.cs" Inherits="Controls_UsersList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script src="../jquery/DocoBlock.js" type="text/javascript"></script>
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
    OnItemDataBound="RadGrid1_ItemDataBound" GroupingSettings-CaseSensitive="false" AlternatingItemStyle-CssClass="alternate">
    <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
        AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10" AllowNaturalSort="false"
        NoMasterRecordsText="No records to display">
        <Columns>
            <telerik:GridTemplateColumn HeaderText="USER NAME" UniqueName="LoginId" DataField="User.Name"
                SortExpression="User.Name" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png"
                SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                <ItemTemplate>
                    <asp:LinkButton ID="imgBtnView" runat="server" CommandName="view" CommandArgument='<%# Eval("Person.Id") %>'><%# Eval("User.Name") %></asp:LinkButton>
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
            <telerik:GridBoundColumn HeaderText="WORK EMAIL" DataField="Person.WorkEmail" UniqueName="Phone"
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
                    <asp:LinkButton runat="server" ID="lnkBtnAdd" CommandName='<%# Eval("User.Name") %>'
                        CommandArgument='<%=Eval("Person.Id") %>'></asp:LinkButton>
                    <asp:LinkButton CssClass="edit" ID="LinkButton1" runat="server" CommandName="edit"
                        CommandArgument='<%# Eval("Person.Id") %>'>Edit</asp:LinkButton>
                    <asp:LinkButton CssClass="deleteitem" ID="LinkButton2" runat="server" OnClientClick="return confirm('Are you sure to delete person?');"
                        CommandName="delete" CommandArgument='<%# Eval("Person.Id") %>'>Delete</asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" />
</telerik:RadGrid>
<telerik:RadCodeBlock runat="server" ID="radCodeBlock">
    <script language="javascript" type="text/javascript">
        if (showCreationRow) {
            var gridDiv = document.getElementById('<%=RadGrid1.ClientID %>');
            createCreationRow(gridDiv, '../Admin/CreateUser.aspx', 'User');
        }
        function refreshGrid() {
            if (showCreationRow)
                createCreationRow(gridDiv, '../Admin/CreateUser.aspx', 'User');
        }
    </script>
</telerik:RadCodeBlock>
