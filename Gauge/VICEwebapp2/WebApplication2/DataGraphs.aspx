<%@ Page Title="DataGraphs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataGraphs.aspx.cs" Inherits="WebApplication2._DataGraphs" %>

<script runat="server">
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        rpiCONNECT = updatePointValue("SP14VICE_Connection", rpiCONNECT.timestamp, DateTime.Now.ToString());
        RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
        FLOW = updatePointValue("SP14VICE_Flow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
        PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
        POWER = updatePointValue("F13APA_POWER_BOT1", "12/6/2013 4:50:00 PM", DateTime.Now.ToString());
        TEMP = updatePointValue("SP14VICE_Temp", "3/13/2013 1:05:00 PM", DateTime.Now.ToString());

        setTableValues();

    }

    public void setTableValues()
    {
        ConnectValue.Text = rpiCONNECT.value;
        Textbox2.Text = rpiCONNECT.timestamp;
        Textbox6.Text = "connected";
        Textbox6.ForeColor = System.Drawing.Color.Green;
        
        RPMvalue.Text = RPM.value;
        Textbox3.Text = RPM.timestamp;
        Textbox7.Text = "high";

        Textbox10.Text = RPM.timestamp;
        
        FlowValue.Text = FLOW.value;
        Textbox4.Text = FLOW.timestamp;
        Textbox8.Text = "high";
        
        PresValue.Text = PRESSURE.value;
        Textbox5.Text = PRESSURE.timestamp;
        Textbox9.Text = "high";

        TempValue.Text = TEMP.value;
        Textbox12.Text = TEMP.timestamp;
        Textbox13.Text = "low";

        PowerValue.Text = POWER.value;
        Textbox14.Text = POWER.timestamp;
        
        
        timelbl.Text = lastValueTime;
    }
</script>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% setTableValues(); %>

    <h2> Change System Parameters:</h2>
    
    <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="10000" />

    <asp:UpdatePanel ID="DataTablePanel" runat="server" UpdateMode="Conditional"> 
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
        <ContentTemplate>
            <div class="panel" style="border-style:solid; background-color:silver; width: 938px;">
            
        <h2> Data Table </h2>
        <p>Table shows data from sensors communicating with OSI PI</p>
         <table style="border-color:gold; border-style:solid;">
        <tr>
            <th> Data Point</th> 
            <th> Value</th> 
            <th> Time Stamp</th> 
            <th> Status </th>
            <th> Descriptor</th> 
        </tr>
        <tr>
            <td>Connection: </td>
            <td><asp:Textbox ID="ConnectValue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox2" runat="server" /></td>
            <td><asp:Textbox ID="Textbox6" runat="server" /></td>
            <td>Verifies connection between raspberry pi and OSI PI database.</td>
        </tr>
        <tr>
            <td>RPM: </td>
            <td><asp:TextBox ID="RPMvalue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox3" runat="server" /></td>
            <td><asp:Textbox ID="Textbox7" runat="server" /></td>
            <td>Shows rotations per minute of the motor controlling the pool pump.</td>
        </tr>
        <tr>
            <td>VFD Frequency: </td>
            <td>
                <asp:DropDownList runat="server" ID="Freq" OnTextChanged="Button1_Click">
                    <asp:ListItem>OFF</asp:ListItem>
                    <asp:ListItem>LOW</asp:ListItem>
                    <asp:ListItem>MEDIUM</asp:ListItem>
                    <asp:ListItem>HIGH</asp:ListItem>
                    <asp:ListItem>MAX</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td><asp:Textbox ID="Textbox10" runat="server" /></td>
            <td><asp:Textbox ID="Textbox11" runat="server" /></td>
            <td>Shows frequency VFD is operating at.</td>
        </tr>
        <tr>
            <td>Flow: </td>
            <td><asp:TextBox ID="FlowValue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox4" runat="server" /></td>
            <td><asp:Textbox ID="Textbox8" runat="server" /></td>
            <td>Shows the flow rate of water through the pool pump.</td>
        </tr>
        <tr>
            <td>Pressure: </td>
            <td><asp:TextBox ID="PresValue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox5" runat="server" /></td>
            <td><asp:Textbox ID="Textbox9" runat="server" /></td>
            <td>Shows the pressure of the water coming out of the pool pump.</td>
        </tr>
        <tr>
            <td>Pool Temperature: </td>
            <td><asp:TextBox ID="TempValue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox12" runat="server" /></td>
            <td><asp:Textbox ID="Textbox13" runat="server" /></td>
            <td>Shows the temperature of the water in the pool.</td>
        </tr>
        <tr>
            <td>Power Consumption: </td>
            <td><asp:TextBox ID="PowerValue" runat="server" /></td>
            <td><asp:Textbox ID="Textbox14" runat="server" /></td>
            <td><asp:Textbox ID="Textbox15" runat="server" /></td>
            <td>Shows the power being used by the pool system.</td>
        </tr>
        </table>
        Last updated : <asp:Label ID="timelbl" runat="server"/>
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Change Frequency" />
        </div>
        </ContentTemplate>
        </asp:UpdatePanel>
        
</asp:Content>