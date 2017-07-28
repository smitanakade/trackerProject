<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewSpecifications.aspx.cs" Inherits="POTracker.Reports.ViewSpecifications" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Purchase Order PDF Viewer</title>
</head>
<body>
    <form id="form1" runat="server">
      <div>
         <h2><strong><asp:Label ID="Label2" text="Please Select to Get Specs PDF:" runat="server"></asp:Label></strong></h2>  
      <p>
        <select id="select1"  runat="server" size=5></select>
        <asp:Button ID="Submit" runat="server" Text="Get Specification PDF" 
              onclick="Submit_Click" BackColor="#0066FF" BorderColor="#0066FF" 
              Font-Bold="True" ForeColor="White" />
       </p> 
        <p style="color:Red"><strong><asp:Label ID="Label1" runat="server"></asp:Label></strong></p>  
      </div>
      <div>
        <iframe id="iframe1" style="border:1px solid #666CCC" title="PDF in an i-Frame" runat="server" frameborder="1" scrolling="auto" height="800" width="1000" ></iframe>
      </div>
   </form>
</body>

</html>
