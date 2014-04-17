

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>




<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" >
    
    
    

    <div class="jumbotron">
        <h1>Cost Comparison</h1>
        <p class="lead">We can now visualize the average cost savings related to incorperating a VFD over the course of a year</p>
        <p><a href="enter site" class="btn btn-primary btn-large">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-5 col-md-offset-2">
            <h2 >&nbsp;&nbsp; &nbsp;Without VFD</h2>
            <p>
                <img src = "Images\VFDX.jpg" class="center" />
                <br />
               
            </p>
            <p></p>
           
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;&nbsp;
            <asp:Label ID="Label5" runat="server"  Font-Bold="True" Enabled="False" >15 GPM</asp:Label>
            <p></p>
             <asp:Label ID="Label1" runat="server"   Font-Bold="True" Enabled="False"  />
            
        </div>
        <div class="col-md-5">
            <h2 ">&nbsp;&nbsp;&nbsp;&nbsp;With VFD</h2>
            <p>
                <img src = "Images\VFD.jpg" class="center" />
                <br />
            </p>
            <p></p>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
            <asp:Label ID="Label4" runat="server"  Font-Bold="True" Enabled="False" />
            <p></p>
            <asp:Label ID="Label2" runat="server"  Font-Bold="True" Enabled="False" />
        </div>
    </div>
    <div  class ="wrapper">
        <hr />
        <asp:Label ID="Label3" runat="server"   Font-Bold="True" Enabled="False" style="text-align:center;" Font-Size="X-Large" />
        <asp:Label ID="cost" runat="server"  ForeColor="Lime" Font-Bold="True" Enabled="False" Font-Size="X-Large" />
        
        <p></p>
        <asp:Label ID="LabelMult" Text="Hours Running Daily:" runat="server" Width="206px" OnClick="costButton_Click" Font-Bold="True" />
         <asp:RadioButton ID="Radio1" Text="1" runat="server" Width="116px" OnClick="costButton_Click" Height="16px" GroupName="mult" />
         <asp:RadioButton ID="Radio2" Text="2" runat="server" Width="116px" OnClick="costButton_Click" Height="19px" GroupName="mult" />
         <asp:RadioButton ID="Radio3" Text="3" runat="server" Width="116px" OnClick="costButton_Click" Height="16px" GroupName="mult" />
         <asp:RadioButton ID="Radio4" Text="4" runat="server" Width="115px" OnClick="costButton_Click" Height="18px" Checked="True" GroupName="mult" />
          <br />
        <br />
          <asp:Button ID="Button1" CssClass="btn btn-primary btn-large" Text="Recalculate Annual Savings" runat="server" Width="206px" OnClick="costButton_Click"  />
        
    </div>
</asp:Content>
