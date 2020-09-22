<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewMessage.ascx.cs" Inherits="Controls_NewMessage" %>

<%@ Register src="AccessControl.ascx" tagname="AccessControl" tagprefix="uc1" %>

<table  class="standardTable">
    <tr>
        <td><label for="txtSubject">Subject:</label></td>
        <td>
            <asp:TextBox ID="txtSubject" runat="server" Columns="60" MaxLength="100"></asp:TextBox>
            <br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Subject field is required" ControlToValidate="txtSubject"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td><label for="txtBody">Text:</label></td>
        <td>
            <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Columns="60" Rows="20"></asp:TextBox>
            <br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Text field is required" ControlToValidate="txtBody"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr runat="server" id="sectionAttach1">
        <td><label for="fAttachment">Attachment*:</label></td>
        <td>
            <asp:FileUpload ID="fAttachment" runat="server" /><br />
            <small runat="server" id="sectionAttach2"> * Attachment - maximum file size: 
     <span id="lblMaxAttachSize" runat="server"></span> Kb, 
     accepted extensions: 
     <span id="lblAcceptedExtensions" runat="server"></span>
</small>
        </td>
    </tr>
   <%-- <tr>
        <td><label for="chkListGroups">Groups:</label></td>
        <td>
            <asp:CheckBoxList ID="chkListGroups" runat="server" />
        </td>
    </tr>

     <tr>
        <td><label for="chkListLocations">Locations:</label></td>
        <td>
            <asp:CheckBoxList ID="chkListLocations" runat="server" />
        </td>
    </tr>--%>

    <tr>
    <td valign="top"><label for="AccessControl1">Access:</label></td>
    <td>
            <uc1:AccessControl ID="AccessControl1" runat="server" />
    </td>
    </tr>

</table>


