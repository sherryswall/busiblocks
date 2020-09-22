<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ManageGroups.ascx.cs"
    Inherits="Controls_ManageGroups" %>
<%@ Register TagPrefix="ul" TagName="UsersList" Src="~/Controls/UsersList.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register TagName="ModalPopup" TagPrefix="uc1" Src="~/Controls/ModalPopup.ascx" %>


<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="RadGrid1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<uc1:ModalPopup ID="pupUsers" runat="server" CancelButtonText="Cancel" Width="500px" Title="Users" Height="150px">
    <FormTemplateContainer>
        <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin=""
            AllowFilteringByColumn="True" OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged"
            OnNeedDataSource="RadGrid1_NeedDataSource" FilterItemStyle-CssClass="radGridFilterRow"
            OnItemDataBound="RadGrid1_ItemDataBound" Visible="true" GroupingSettings-CaseSensitive="false" AlternatingItemStyle-CssClass="alternate">
            <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
                AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10"
                NoMasterRecordsText="No records to display">
                <Columns>
                    <telerik:GridBoundColumn HeaderText="USER NAME" DataField="User.Name" UniqueName="LoginId"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false" 
                        SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="LAST NAME" DataField="Person.LastName" UniqueName="FirstName"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                        SortExpression="Person.LastName"
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
                            <asp:LinkButton runat="server" ID="lnkBtnAdd" CommandName="add" CommandArgument='<%#Eval("Person.Id") %>' OnClientClick="javascript:pupUsers.Hide();"><img src='../App_Themes/default/icons/add.png'/>Add User</asp:LinkButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" FirstPageImageUrl="../App_Themes/Default/icons/pageFirst.png"
                PrevPageImageUrl="../App_Themes/Default/icons/pagePrevious.png" LastPageImageUrl="../App_Themes/Default/icons/pageLast.png"
                NextPageImageUrl="../App_Themes/Default/icons/pageNext.png" />
        </telerik:RadGrid>
    </FormTemplateContainer>
</uc1:ModalPopup>

<div style="display: none;">
    <telerik:RadGrid ID="RadGrid1_event_linker" runat="server" Visible="true" OnItemCommand="RadGrid1_ItemCommand"
        OnPageSizeChanged="RadGrid1_PageSizeChanged" OnNeedDataSource="RadGrid1_NeedDataSource"
        OnItemDataBound="RadGrid1_ItemDataBound" />
</div>

<script type="text/javascript" language="javascript">
    var showCreationRow = false;
    var listBoxUsers = "<%=lstUsers.ClientID %>";   
</script>
<div class="editGroup">
    <asp:UpdatePanel runat="server" ID="updatePnl" UpdateMode="Always">
        <ContentTemplate>
            <div id="divUsersList" class="groupUsers">
                <h3>
                    Users</h3>
                <table>
                    <tr>
                        <td class="groupLabel">
                            Users:
                        </td>
                        <td>
                            <asp:ListBox ID="lstUsers" Height="100px" runat="server" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnShowUsersList" OnClientClick="javascript:pupUsers.Show(); return false;" OnClick="btnShowUsersList_Click"
                                Text="Add User" CssClass="btn" />
                            <asp:Button runat="server" CssClass="btn" ID="btnRemoveUsers" Text="Remove Selected"
                                OnClick="btnRemoveSelected_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divGroupProperties" class="groupDetails">
                <h3>
                    Group Details</h3>
                <table>
                    <tr>
                        <td class="groupLabel">
                            Group Name:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtBxGroupName"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="groupLabel">
                            Description:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtBxDescription" TextMode="MultiLine" Height="100px"></asp:TextBox><br />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="clear">
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
