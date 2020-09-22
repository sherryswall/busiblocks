<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="FormDefinitions.aspx.cs" Inherits="FormDefinitions" Title="All Forms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentPlaceHolder" runat="Server">
    <h1>
        Forms</h1>
    <div>
        <asp:Repeater ID="formDefinitionList" runat="server">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href="<%# GetFormLink((string)Eval("Id")) %>"><%# Server.HtmlEncode((string)Eval("Name")) %></a>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
