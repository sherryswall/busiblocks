<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditorialComments.ascx.cs"
    Inherits="Controls_EditorialComments" %>

    <tr>
        <td class="editCommentsLabel">
            <label for="txtComments">
                Editorial Comments:</label>
        </td>
        <td>
            <asp:TextBox CssClass="editCommentsField" TextMode="MultiLine" ID="txtComments" runat="server"></asp:TextBox>
            <br />
            <label class="small">
                Editorial comments allow users to identify what changes have been made since updating
                this document. It is important that these are included as this description will
                be displayed with each document for all readers to see</label>
        </td>
    </tr>
