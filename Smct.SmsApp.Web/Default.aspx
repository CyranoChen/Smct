<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Smct.SmsApp.Web._Default" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <title>上海明东生日短信发送管理页</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <asp:Button ID="btnSendSmsBirthday" runat="server" Text="发送生日短信" OnClick="btnSendSmsBirthday_Click" />
            </div>
            <asp:GridView ID="gvEmployee" runat="server" DataKeyNames="EmployeeID" PageSize="20"
                AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField HeaderText="标识" DataField="EmployeeID" />
                    <asp:BoundField HeaderText="姓名" DataField="Name" />
                    <asp:BoundField HeaderText="生日" DataField="Birthday" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField HeaderText="备注" DataField="Remark" />
                    <asp:CommandField ShowSelectButton="true" HeaderText="操作" EditText="修改"
                        SelectText="发送短信" UpdateText="保存" CancelText="取消" DeleteText="删除" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
