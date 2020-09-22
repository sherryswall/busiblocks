<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="CreateGroup.aspx.cs" Inherits="Admin_CreateGroup"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="mg" TagName="ManageGroup" Src="~/Controls/ManageGroups.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <script type="text/javascript" language="javascript">
        var manageGroupType = 'Create';
    </script>
    <h1 id="manageGroup" class="sectionhead">
        Create Group</h1>
    <mg:ManageGroup runat="server" ID="ctrlManageGroupCreate" />
    <asp:Button runat="server" Text="Cancel" CssClass="btn" ID="btnCancel" OnClick="btnCancel_Click" />
    <asp:Button runat="server" Text="Create" CssClass="btn" ID="btnCreate" OnClick="btnCreate_Click" />
</asp:Content>
