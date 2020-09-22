<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="EditGroup.aspx.cs" Inherits="Admin_EditGroup" %>

<%@ Register TagPrefix="mg" TagName="ManageGroup" Src="~/Controls/ManageGroups.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script type="text/javascript" language="javascript">
        var manageGroupType = 'Edit';
    </script>
    <h1 class="sectionhead" id="manageGroup">Edit Group</h1>
    <mg:ManageGroup runat="server" ID="ctrlManageGroupEdit" />
    <asp:Button runat="server" Text="Cancel" CssClass="btn" ID="btnCancel" OnClick="btnCancel_Click" />
    <asp:Button runat="server" Text="Save" CssClass="btn" ID="btnSave" OnClick="btnSave_Click" />
</asp:Content>
