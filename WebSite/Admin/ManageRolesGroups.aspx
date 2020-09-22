<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ManageRolesGroups.aspx.cs"
    Inherits="Admin_ManageRolesGroups" Title="Manage Groups" %>

<%@ Register Src="~/Controls/ModalPopup.ascx" TagName="ModalPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script src="../jquery/matrix.js" type="text/javascript"></script>
    <script type="text/javascript">

        var UrlCreatePTR = "ManageRolesGroups.aspx/CreatePersonTypeRole";
        var UrlDeletePTR = "ManageRolesGroups.aspx/DeletePersonTypeRole";
        var UrlGetAllPTR = "ManageRolesGroups.aspx/GetPersonTypeRoles";

        var TopLeftCellText = "Group/Block";
        var tableBodyContainer = "tbyPersonTypeRole";

        var disabledCollection = { "columnName": "ADMIN", "rowName": "System Administrators" };

        var actionEdit = { "href": "EditGroup.aspx?id=[id]", "class":"edit"};
        var actionDelete = { "href": "#", "class": "deleteitem", "onclick": "showDeleteGroupPopup(this.name)", "name": "[id]" };

        var actionList = { "Edit": actionEdit, "Delete": actionDelete };

        $(document).ready(function () {
            BuildMatrix(UrlGetAllPTR, UrlDeletePTR, UrlCreatePTR, tableBodyContainer, TopLeftCellText, disabledCollection, actionList);
        });

        function showDeleteGroupPopup(refId) {
            var groupName = $('#' + refId + ' span').html();
            popDeleteGroup.SetRefId(refId);
            popDeleteGroup.SetContent('Are you sure you want to delete ' + groupName + ' group?');
            popDeleteGroup.Show();
        }

    </script>
    <h1 id="rolesgroups" class="sectionhead">Manage Groups</h1>    
    <!-- Invisible -->   
    <div id="matrix">
        <table id="tblGroupRole" class="standardTable minimum">
            <tbody id="tbyPersonTypeRole">
            </tbody>
        </table>
    </div>
    <uc1:ModalPopup ID="popDeleteGroup" runat="server" Content="" AcceptButtonText="Delete" Title="Delete Group"
        CancelButtonText="Cancel"  OnAcceptClick="DeleteGroup_Click" />
</asp:Content>
