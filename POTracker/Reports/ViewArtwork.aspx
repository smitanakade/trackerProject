<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewArtwork.aspx.cs" Inherits="POTracker.Reports.ViewArtwork" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Files for Artwork Folder</title>
        <style type="text/css">
            a { text-decoration: none; }
            a:hover { text-decoration: underline; }
            p {font-family: verdana; font-size: 10pt; }
            h2 {font-family: verdana; }
            td {font-family: verdana; font-size: 10pt; }                           
        </style>
</head>
<body>
    <asp:Label runat="server" ID="lblProductCode" Font-Bold="True" Font-Names="Verdana" Font-Size="12pt" ></asp:Label><asp:Label runat="server" ID="lblArtworkheader" Font-Bold="True" Font-Names="Verdana" Font-Size="12pt" Text=" - Artwork Folder" Visible="False" ></asp:Label>
        <p>
        sort by name <a href="?p=<%=lblProductCode.Text%>&sortby=name">asc</a>/<a href="?p=<%=lblProductCode.Text%>&sortby=namerev">desc</a> |
        sort by date <a href="?p=<%=lblProductCode.Text%>&sortby=date">asc</a>/<a href="?p=<%=lblProductCode.Text%>&sortby=daterev">desc</a> |
        sort by size <a href="?p=<%=lblProductCode.Text%>&sortby=size">asc</a>/<a href="?p=<%=lblProductCode.Text%>&sortby=sizerev">desc</a>        
        </p>
    <form id="form1" runat="server">
    <div style="border-color:black; border-bottom:solid;border-top:solid">
    <asp:DataList id="DirectoryListing" runat="server">
                <ItemTemplate>
                     <a href="<%# ((System.IO.FileSystemInfo)Container.DataItem).FullName.Replace(Server.MapPath("~"), "/") %>" target="_blank">
                    <img alt="icon" src="<%# DisplayFileIcon((System.IO.FileSystemInfo)Container.DataItem) %>" height="50px" width="50px" /> <br /> 
                   |<%# ((System.IO.FileSystemInfo)Container.DataItem).Name %><%#GetFileSizeString((System.IO.FileSystemInfo)Container.DataItem) %>|</a>
                </ItemTemplate>
            </asp:DataList>
    </div>
        <p>
                <asp:Label runat="Server" id="FileCount" />
            </p>
    </form>
</body>
</html>
