<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintArticle.aspx.cs" Inherits="Doco_PrintArticle" 
            Title="Print Document" EnableTheming="false" %>

<%@ Register TagPrefix="uc" TagName="ViewArticle" Src="~/Controls/ViewArticle.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link href="../App_Themes/Default/Styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    
        <uc:ViewArticle ID="viewArticle" runat="server" Printing="true" />    

    </form>
</body>
</html>
