<%@ Page Title="DataGraphs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataGraphs.aspx.cs" Inherits="WebApplication2._DataGraphs" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- IMP: Script Manager -->
    <asp:ScriptManager ID="SM1" runat="server">
            <Scripts>
                <%--To learn more about bundling scripts in ScriptManager see http://go.microsoft.com/fwlink/?LinkID=301884 --%>
                <%--Framework Scripts--%>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Name="jquery" />
                <asp:ScriptReference Name="bootstrap" />
                <asp:ScriptReference Name="respond" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />
                <%--Site Scripts--%>
            </Scripts>
    </asp:ScriptManager>


    <!-- Timer to update table every 10 seconds -->
    <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="10000" />


    <!-- Update Panel to update table values -->
    <asp:UpdatePanel ID="DataTablePanel" runat="server" UpdateMode="Conditional"> 
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
        <ContentTemplate>

        <div class="panel"></div>

        <!-- grey background panel -->
        <div class="panel" style="border-style:solid; border-color:black; border-width:thick; background-color:silver; min-width: 1000px;"> 
        
        <h2> Data Table </h2>
        <p>Table shows data from sensors communicating with OSI PI</p>

        <!---- Data Table ---->
        <table style="border-color:gold; border-style:solid;">
            <!-- headers -->
            <tr>
                <th>Data Point</th>
                <th>Value</th>
                <th>Time Stamp</th>
                <th>Status </th>
                <th>Descriptor</th>
            </tr>

            <!-- RPi Connection row -->
            <tr>
                <td>Connection: </td>
                <td><asp:TextBox ID="ConnectValue" runat="server" OnTextChanged="changeTableValues" /></td>
                <td><asp:TextBox ID="ConnectTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="ConnectStatus" runat="server" /></td>
                <td>Verifies connection between raspberry pi and OSI PI database.</td>
            </tr>

            <!-- User Lock Status -->
            <tr>
                <td>User Input Lock: </td>
                <td><asp:TextBox ID="LockValue" runat="server" /></td>
                <td><asp:TextBox ID="LockTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="Lockstatus" runat="server" /></td>
                <td>Determines whether or not the user on this web application is locked out from making changes in the system.</td>
            </tr>

            <!-- RPM row -->
            <tr>
                <td>RPM: </td>
                <td><asp:TextBox ID="RPMvalue" runat="server" OnTextChanged="RPMvalue_TextChanged" /></td>
                <td><asp:TextBox ID="RPMtimestamp" runat="server" /></td>
                <td><asp:TextBox ID="RPMstatus" runat="server" /></td>
                <td>Shows rotations per minute of the motor controlling the pool pump. Can be changed here</td>
            </tr>

            <!-- Frequency -->
            <tr>
                <td>VFD Frequency: </td>
                <td><asp:TextBox ID="freqvalue" runat="server"></asp:TextBox></td>
                <td><asp:TextBox ID="Freqtimestamp" runat="server" /></td>
                <td><asp:TextBox ID="Freqstatus" runat="server" /></td>
                <td>Shows frequency VFD is operating at.Can be changed here</td>
            </tr>

            <!-- Flow rate -->
            <tr>
                <td>Flow Rate: </td>
                <td><asp:TextBox ID="FlowValue" runat="server" /></td>
                <td><asp:TextBox ID="Flowtimestamp" runat="server" /></td>
                <td><asp:TextBox ID="Flowstatus" runat="server" /></td>
                <td>Shows the flow rate of water through the pool pump.</td>
            </tr>

             <!-- Desired Flow rate -->
            <tr>
                <td>Desired Flow Rate: </td>
                <td><asp:TextBox ID="DesiredFlowValue" runat="server" OnTextChanged="DesiredFlowValue_TextChanged"/></td>
                <td><asp:TextBox ID="DFtimestamp" runat="server" /></td>
                <td><asp:TextBox ID="DFstatus" runat="server" /></td>
                <td>Shows the desired flow rate of water through the pool pump. Can be changed here</td>
            </tr>

            <!-- Pressure -->
            <tr>
                <td>Pressure: </td>
                <td><asp:TextBox ID="PresValue" runat="server"/></td>
                <td><asp:TextBox ID="Prestimestamp" runat="server" /></td>
                <td><asp:TextBox ID="PresStatus" runat="server" /></td>
                <td>Shows the pressure of the water coming out of the pool pump.</td>
            </tr>

            <!-- Temperature -->
            <tr>
                <td>Motor Temperature: </td>
                <td><asp:TextBox ID="TempValue" runat="server" /></td>
                <td><asp:TextBox ID="TempTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="TempStatus" runat="server" /></td>
                <td>Shows the temperature of the motor of the pool pump.</td>
            </tr>

            <!-- Voltage -->
            <tr>
                <td>Voltage: </td>
                <td><asp:TextBox ID="VoltValue" runat="server" /></td>
                <td><asp:TextBox ID="VoltTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="VoltStatus" runat="server" /></td>
                <td>Shows the voltage being used by the pool system.</td>
            </tr>

            <!-- Current -->
            <tr>
                <td>Current: </td>
                <td><asp:TextBox ID="CurrValue" runat="server" /></td>
                <td><asp:TextBox ID="CurrTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="CurrStatus" runat="server" /></td>
                <td>Shows the current being used by the pool system.</td>
            </tr>

            <!-- Power -->
            <tr>
                <td>Power Consumption: </td>
                <td><asp:TextBox ID="PowerValue" runat="server" /></td>
                <td><asp:TextBox ID="PowerTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="PowerStatus" runat="server" /></td>
                <td>Shows the power being used by the pool system.</td>
            </tr>

            <!-- Cost -->
            <tr>
                <td>Energy Cost: </td>
                <td><asp:TextBox ID="costvalue" runat="server" /></td>
                <td><asp:TextBox ID="costTimestamp" runat="server" /></td>
                <td><asp:TextBox ID="costStatus" runat="server" /></td>
                <td>Shows the energy cost approximated for one month.</td>
            </tr>

        </table><!-- end data table -->

        <!-- time stamp -->
        Last updated : <asp:Label ID="timelbl" runat="server"/>
        <br />
        <input type="button" runat="server" onserverclick="changeTableValues" value="Update Values" />
           
        </div> <!-- end of background panel -->
        </ContentTemplate>
        </asp:UpdatePanel>
    
        
</asp:Content>