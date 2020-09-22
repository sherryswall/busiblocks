<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SiteDetails.aspx.cs"
    Inherits="Directory_SiteDetails" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2011.2.915.35, Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>

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
        Directory - Site Details</h1>
    <div>
    <h2 id="headingSite">
        <asp:Label ID="lblHeadingSite" runat="server"></asp:Label></h2>
        <table class="standardTable minimum">
            <tr class="primary">
                <td>
                    Site Name
                </td>
                <td class="details">
                    <asp:Label ID="txtSiteName" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Site Type
                </td>
                <td class="details">
                    <asp:Label ID="txtSiteType" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Phone Number
                </td>
                <td class="details">
                    <asp:Label ID="txtPhoneNumber" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Alternate Phone Number
                </td>
                <td class="details">
                    <asp:Label ID="txtAltPhoneNumber" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Email
                </td>
                <td class="details">
                    <asp:Label ID="txtEmail" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Postal Address 1
                </td>
                <td class="details">
                    <asp:Label ID="txtPostalAddress1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Postal Address 2
                </td>
                <td class="details">
                    <asp:Label ID="txtPostalAddress2" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Postal Suburb
                </td>
                <td class="details">
                    <asp:Label ID="txtPostalSuburb" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Postal Postcode
                </td>
                <td class="details">
                    <asp:Label ID="txtPostalPostcode" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Postal State
                </td>
                <td class="details">
                    <asp:Label ID="txtPostalState" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Physical Address 1
                </td>
                <td class="details">
                    <asp:Label ID="txtPhysicalAddress1" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Physical Address 2
                </td>
                <td class="details">
                    <asp:Label ID="txtPhysicalAddress2" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Physical Suburb
                </td>
                <td class="details">
                    <asp:Label ID="txtPhysicalSuburb" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Physical Postcode
                </td>
                <td class="details">
                    <asp:Label ID="txtPhysicalPostcode" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Physical State
                </td>
                <td class="details">
                    <asp:Label ID="txtPhysicalState" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Region
                </td>
                <td class="details">
                    <asp:Label ID="txtRegion" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <h2 id="headingUser" runat="server">
        Users : <asp:Label ID="lblHeadingUser" runat="server"></asp:Label></h2>
    <telerik:RadGrid ID="RadGrid1" runat="server" EnableEmbeddedSkins="true" Skin="" AllowFilteringByColumn="True"
        OnItemCommand="RadGrid1_ItemCommand" OnPageSizeChanged="RadGrid1_PageSizeChanged" OnNeedDataSource="RadGrid1_NeedDataSource"
        FilterItemStyle-CssClass="radGridFilterRow" GroupingSettings-CaseSensitive="false">
        <MasterTableView RetrieveAllDataFields="false" UseAllDataFields="false" AutoGenerateColumns="false"
            AllowSorting="true" AllowPaging="true" CssClass="standardTable" PageSize="10" AllowNaturalSort="false"
            NoMasterRecordsText="No records to display">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="USER NAME" UniqueName="LoginId" SortExpression="LoginId"
                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                    DataField="User.Name" SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
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
                <telerik:GridBoundColumn HeaderText="PRIMARY SITE" DataField="PrimarySite.Name" UniqueName="Primary Site"
                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="POSITION" DataField="Person.Position" UniqueName="Position"
                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"
                    SortAscImageUrl="../App_Themes/Default/icons/sortAscending.png" SortDescImageUrl="../App_Themes/Default/icons/sortDescending.png">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true" />
    </telerik:RadGrid>
</asp:Content>
