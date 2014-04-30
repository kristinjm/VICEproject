<%@ Page Title="Schedule" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Schedule.aspx.cs" Inherits="WebApplication2.WebForm2" %>


<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
<%--<%@ Import Namespace="System.Web.UI.DataVisualization.Charting" %>
<%@ Import Namespace="System.Web.UI.DataVisualization.Charting.ChartHttpHandler" %>
<%@ Import Namespace="System.Web.DataVisualization" %>--%>



<script runat="server">
    public List<string> hours = new List<string>(){"12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM",
                                                "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM",
                                                "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"};
     
</script>


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

     
    <!-- don't need script? -->   
    <script src="schedScript.js" type="text/javascript"></script>

    <!-- Timer for sim countdown -->
    <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="1000" /> <!--Timer1 interrupts every 1 sec-->
    <!--<asp:Timer ID="Timer2" runat="server" Interval="2000"></asp:Timer> <!-- Timer2 interrupts every 2 sec -->
    
    <!-- header break --><div class="panel"></div>

    <!-- Panel for sim parameters -->
    <div class="row">

        <!-- Panel for sim parameters -->
        <div class="col-md-4">
            <asp:Panel runat="server" BorderStyle="Solid" BorderColor="Black">
            <h4>Choose type of energy cost rates:</h4>
    
            <asp:RadioButtonList runat="server" ID="ratesRadio">
                <asp:ListItem Enabled="true" Selected="True" Value="tier">Tiered rates</asp:ListItem>
                <asp:ListItem Enabled="true" Selected="False" Value="time">Time of use rates</asp:ListItem>
            </asp:RadioButtonList>
        Start Time: <asp:DropDownList runat="server">
            <asp:ListItem>12:00 AM</asp:ListItem>
            <asp:ListItem>1:00 AM</asp:ListItem>
            <asp:ListItem>2:00 AM</asp:ListItem>
            <asp:ListItem>3:00 AM</asp:ListItem>
            <asp:ListItem>4:00 AM</asp:ListItem>
            <asp:ListItem>5:00 AM</asp:ListItem>
        </asp:DropDownList>

        Stop Time: <asp:DropDownList runat="server">
            <asp:ListItem>12:00 AM</asp:ListItem>
            <asp:ListItem>1:00 AM</asp:ListItem>
            <asp:ListItem>2:00 AM</asp:ListItem>
            <asp:ListItem>3:00 AM</asp:ListItem>
            <asp:ListItem>4:00 AM</asp:ListItem>
            <asp:ListItem>5:00 AM</asp:ListItem>
        </asp:DropDownList>

        <h4>Choose time of year: </h4>
        <asp:RadioButtonList runat="server" ID="seasonRadio">
            <asp:ListItem Enabled="true" Selected="True" Value="Summer">Summer</asp:ListItem>
            <asp:ListItem Enabled="true" Selected="False" Value="Winter">Winter</asp:ListItem>
        </asp:RadioButtonList>

        <h4>Choose type of pool use: </h4>
        <asp:RadioButtonList runat="server" ID="useRadio">
            <asp:ListItem Enabled="true" Selected="True" Value="filtering">Filtering/Cleaning</asp:ListItem>
            <asp:ListItem Enabled="true" Selected="False" Value="heating">Heating</asp:ListItem>
        </asp:RadioButtonList>

        <h4>Choose length of simulation: </h4>
        <asp:RadioButtonList runat="server" ID="lengthRadio">
            <asp:ListItem Enabled="true" Selected="True" Value="day">One day</asp:ListItem>
            <asp:ListItem Enabled="true" Selected="False" Value="month">One Month</asp:ListItem>
            <asp:ListItem Enabled="true" Selected="False" Value="year">One Year</asp:ListItem>
        </asp:RadioButtonList>

        <h4>Choose Flow rate: </h4>
        <asp:DropDownList runat="server">
            <asp:ListItem Enabled="true" Selected="True">Best Flow Rate</asp:ListItem>
            <asp:ListItem>1</asp:ListItem>
        </asp:DropDownList>
        <br /><br />

        <input type="submit" runat="server" id="Submit1" value="Start Simulation" onserverclick="setSim" />
        <div class="panel"></div>
            </asp:Panel>
        </div> <!-- end sim parameters panel -->

        <!-- Panel for sim results -->
        <div class="col-lg-8">
            <asp:Panel runat="server" BorderStyle="Solid" BorderColor="Black" Height="570">
            
            <asp:UpdatePanel ID="ResultsUP" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Timer1" />
                </Triggers>
                <ContentTemplate>
                    <h4>Simulation Results</h4>

                    <asp:Label runat="server" ID="Textlbl1"></asp:Label> <asp:Label runat="server" ID="Timelbl"></asp:Label>
                    <br />

                    <!-- Chart 1 = Energy Chart -->
                    <asp:Chart ID="Chart1" runat="server">
                        <series>
                            <asp:Series Name="Without VFD" ChartType="Line">
                                <Points>
                                    <asp:DataPoint YValues="0" />
                                    <asp:DataPoint XValue="60" YValues="60" />
                                </Points>
                            </asp:Series>
                            <asp:Series Name="Series2" ChartType="Line"></asp:Series>
                            <asp:Series Name="Series3" ChartType="Line"></asp:Series>
                        </series>
                        <chartareas>
                            <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                        </chartareas>
                        <Titles>
                            <asp:Title Name="Title1" Text="Energy Simulation (kWh vs. sec)"></asp:Title>
                        </Titles>
                    </asp:Chart>
                   
                    <!--<asp:Chart ID="Chart2" runat="server">
                        <series>
                            <asp:Series Name="Series1" ChartType="Line">
                                <Points>
                                    <asp:DataPoint YValues="0" />
                                    <asp:DataPoint XValue="60" YValues="60" />
                                </Points>
                            </asp:Series>
                            <asp:Series Name="Series2" ChartType="Line"></asp:Series>
                            <asp:Series Name="Series3" ChartType="Line"></asp:Series>
                        </series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Name="Title1" Text="Cost Simulation"></asp:Title>
                        </Titles>
                    </asp:Chart>-->
                    <br />
                    Energy Used: <asp:Label runat="server" ID="energylbl"></asp:Label>
                    Energy Rate Tier: <asp:Label runat="server" ID="tierlbl"></asp:Label>
                    <div id="resultspan" runat="server"></div>
                    
                </ContentTemplate>
            </asp:UpdatePanel>

        </asp:Panel>
    </div><!-- end sim results panel -->
    
    </div><!-- end row for 2 panels -->
   

</asp:Content>
