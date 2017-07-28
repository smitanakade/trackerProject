<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="commentsViewer.aspx.cs" Inherits="POTracker.Reports.commentsViewer" %>
<%--<%@ Register TagPrefix="My" TagName="RoleUserBoxControl" Src="~/UserControls/RoleUsers.ascx" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Purchase Order PDF Viewer</title>
    <style type="text/css">
        .style1 {
            width: 475px;
        }

        .style2 {
            width: 148px;
        }
    </style>
    <script type="text/javascript">
        function RedirectToParent(ri) {
            //window.opener.location.href = window.opener.location.href + "?RI=" + ri;

            window.opener.commentsGridRefresh(ri);
        }
    </script>
</head>
<body style="background-color: #d5d0d0;">
    <form style="background-color: #d5d0d0;" id="form1" runat="server">
        <div>
            <%--  <iframe id="fred" style="border:1px solid #666CCC" title="PDF in an i-Frame" src="PURCHASEORDERS/Arlec-PURCHASE ORDER-503.pdf" frameborder="1" scrolling="auto" height="1200" width="1000" ></iframe>--%>
            <asp:Panel ID="Panel1" runat="server">
                <table style="width: 100%;">
                    <tr>
                        <td class="style2">
                            <asp:Literal ID="Literal1" runat="server">Purchase Order No.:</asp:Literal>
                        </td>
                        <td>
                            <asp:TextBox ID="testText" runat="server" ReadOnly="True" Font-Bold="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Literal ID="Literal2" runat="server">Your Username:</asp:Literal>
                        </td>
                        <td>
                            <asp:LoginName ID="HeadLoginName" runat="server" />
                        </td>
                    </tr>
                </table>
                <p></p>
                <p></p>

                <p>
                    <asp:TextBox ID="TextBox2" runat="server" Height="154px" Width="400px"
                        Wrap="True" TextMode="MultiLine"></asp:TextBox>
                </p>
              <asp:Repeater ID="Repeater1" runat="server" >
                <ItemTemplate>
                    <strong><asp:Label ID="LabelRoleName" Text='<%# Eval("RoleUser") %>' runat="server"></asp:Label></strong>
            <asp:CheckBoxList ID="CheckBoxListUsers" runat="server"
                DataTextField="UserName"
                DataValueField="UserName" 
                Datasource= '<%# DataBinder.Eval(Container.DataItem, "Users")%>'
                RepeatColumns="6">
            </asp:CheckBoxList>
                </ItemTemplate>
            </asp:Repeater>
                <%--<My:RoleUserBoxControl runat="server" ID="MyUserInfoBoxControl"/>--%>
                    <p>
                        <asp:Button ID="Button1" runat="server" Text="Send" OnClick="Button1_Click" /></p>
                    <p style="color: #FF0000; font-style: normal; font-weight: bold">
                        <asp:Literal ID="error" runat="server"></asp:Literal>
                    </p>
                    <p></p>
            </asp:Panel>

        </div>
    </form>
</body>
</html>
