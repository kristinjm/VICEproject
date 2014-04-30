<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

<%@ Import Namespace="System.Data" %>
<script runat="server">
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            rpiCONNECT = updatePointValue("SP14VICE_Connection", rpiCONNECT.timestamp, DateTime.Now.ToString());
            RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_DesiredFlow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            POWER = updatePointValue("F13APA_POWER_BOT1", "12/6/2013 4:50:00 PM", DateTime.Now.ToString());
            USERLOCK = updatePointValue("SP14VICE_Lock", "12/6/2013 4:50:00 PM", DateTime.Now.ToString()); // 1= unlocked, 0 = locked
            
            cost = findCost(120, (float)0.35); //Assume: running pump for 4 hrs/day for 30 days; tier 3 pricing during summer ($.35/kWH)

            setPointValue("SP14VICE_DesiredFlow", desiredFlow.ToString()); // Only gets set if userlock is set to 1
            
            setTableValues();
            
        }

        public void setTableValues()
        {
            flowlbl.Text = FLOW.value;
            freqlbl.Text = RPM.value;
            powerlbl.Text = POWER.value;
            costlbl.Text = cost.ToString();
            rpmlbl.Text = RPM.value;
            preslbl.Text = PRESSURE.value;

            if (USERLOCK.value == "1")
            {
                lockoutdisp.InnerHtml = "ON";
            }
            else if (USERLOCK.value == "0")
            {
                lockoutdisp.InnerHtml = "OFF";
            }            
        }

        public void setPumpHours()
        {

            pumpStart.DataSource = hours;
            pumpStart.DataBind();

            pumpStop.DataSource = hours;
            pumpStop.DataBind();

            vfdstart.DataSource = hours;
            vfdstart.DataBind();
            string content = "<asp:ListItem>12:00 AM</asp:ListItem>";
            for (var i = 0; i < 24; i++)
            {
                content += "<asp:ListItem>" + hours[i] + "</asp:ListItem>";
            }
    

            vfdstop.DataSource = CreateDataSource();
            vfdstop.DataTextField = "vfdTextField";
            vfdstop.DataValueField = "vfdValueField";
            vfdstop.DataBind();
            vfdstop.SelectedIndex = 0;
        }
        
        ICollection CreateDataSource() {
            // Create a table to store data for the DropDownList control.
            DataTable dt = new DataTable();

            // Define the columns of the table.
            dt.Columns.Add(new DataColumn("vfdTextField", typeof(String)));
            dt.Columns.Add(new DataColumn("vfdValueField", typeof(String)));
            foreach (string s in hours)
            {
                DataRow dr = dt.NewRow();
                dr[0] = s;
                dr[1] = s;
                dt.Rows.Add(dr);
            }
            DataView dv = new DataView(dt);
            return dv;
        }

        protected void vfdstop_SelectedIndexChanged(object sender, EventArgs e)
        {
            schedcost.Text = vfdstop.Text;
        }
        
        public void flowsliderchange(object sender, EventArgs e)
        {
            //setPointValue("SP14VICE_DesiredFlow", flowrangeInput.Value);           
            //lockoutdisp.InnerHtml = flowrangeInput.Value;
        }
        
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

    <% var time = DateTime.Now; %>
    <!--<script src="defaultScript.js" type="text/javascript"></script>-->
    <script type="text/javascript" src="bindows_gauges/bindows_gauges.js"></script>
    <script type="text/javascript" src="bindows_gauges/gauge1.js"></script>

    <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="10000" /> <!--Timer1 interrupts ever 10 sec-->
    
    <script> <!-- script for slider changes -->
       <% setTableValues(); %>
       var value = "<%=slider %>";
        window.addEventListener("load", start, false);

        function start(){
        flowrangeInput.addEventListener('change', sliderchange, false);
        rpmrangeInput.addEventListener('change', sliderchange, false);
        }

        function sliderchange(){
            //alert("slider change");
            //flowout.innerHTML = flowrangeInput.value;
            //rpmout.innerHTML = rpmrangeInput.value;
            //input1.value = flowrangeInput.value;
            //desiredRPM.value = rpmrangeInput.value;
            //desiredFlow.value = flowrangeInput.value;
            //alert(input1.value);
            
        }
    </script>
    

    <br />
    <br />
    <!-- Header panel-->
    <div class="panel" style="background-color:gold; min-height:300px;">
        <div class="container">
        <div class="row">
        <div class="col-md-4" style="align-content:center; justify-content:center; width:25% ">
            <br />
            <img src="Images\BadgeFinal1.png" alt="VICE Badge" height="250"/>
        </div>
        <div class="col-md-4" style="align-content:center; justify-content:center; width:70%">
            <br />
            <h1 style="font-size:50px;">V.I.C.E.</h1>
            <h2>VFD Integration for Conservation of Energy</h2> 
            <h3>A group of Electrical and Computer engineers designing a VFD system for residential use to conserve energy.</h3>
        </div>
        </div>

        </div>
    </div>
    <br />

    <!-- Slider and Gauge Inputs-->
    <div class="row">
        <div id="flowslider" class="col-md-4">
            <div id="gaugeDivgpm" style="width: 300px; height: 300px; float:left"></div>
        </div>
        <div class="col-md-4">
            <div id="gaugeDivpsi" style="width: 300px; height: 300px; float:left"></div>  
        </div>
        <div class="col-md-4">
            <div id="gaugeDivrpm" style="width: 300px; height: 300px; float:left"></div>
       </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <asp:Label runat="server">Flow Input:</asp:Label>
            <input type="range" id="flowrangeInput" name="flowrangeInput" min="0" max="10" value="0"
                oninput="flowamount.value=flowrangeInput.value" onchange="flowsliderchange">   
            <output id="flowamount" name="flowamount" for="flowrangeInput">0</output>                                                    
            <span id="flowout"></span>
        </div>
        <div class="col-md-4">
            Pressure:
        </div>
        <div class="col-md-4">
            RPM Input:
            <input type="range" id="rpmrangeInput" name="rpmrangeInput" min="0" max="3600" value="0"
                oninput="rpmamount.value=rpmrangeInput.value" onmouseup="rpmsliderchange">     
            <output id="rpmamount" name="rpmamount" for="rpmrangeInput">0</output>                                                  
            <span id="rpmout"></span>
        </div>
    </div>

    
    <!-- Script for Gauges -->
    <asp:UpdatePanel ID="GuagesPanel" runat="server" UpdateMode="Conditional"> 
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
        <ContentTemplate>
            <!-- User lock out interface -->
            User input control:
            <span id="lockoutdisp" runat="server"></span>

            <!-- Script for Gauges -->
            <script type="text/javascript">

                // Load the gauge into the div
                var gauge_rpm = bindows.loadGaugeIntoDiv("gauges/rpm_gauge.xml", "gaugeDivrpm");
                var gauge_psi = bindows.loadGaugeIntoDiv("gauges/psi_gauge.xml", "gaugeDivpsi");
                var gauge_gpm = bindows.loadGaugeIntoDiv("gauges/gpm_gauge.xml", "gaugeDivgpm");

                var t1 = 0;
                var t2 = 0;
                var t3 = 0;

                var interval1 = 1;
                var interval2 = 1;
                var interval3 = 1;

                function updateGauge1() {
                    if ("<%=RPM.value%>" >= t1) {
                         if (t1 == "<%=RPM.value%>") { return }
                         t1 += interval1;
                         gauge_rpm.needle.setValue(t1);
                         gauge_rpm.label.setText(Math.round(t1));
                     }
                     if ("<%=RPM.value%>" < t1) {
                         if (t1 == "<%=RPM.value%>") { return }
                         t1 -= interval1;
                         gauge_rpm.needle.setValue(t1);
                         gauge_rpm.label.setText(Math.round(t1));
                     }
                 }
                 setInterval(updateGauge1, interval1);   //1000 == 1 second

                 function updateGauge2() {
                     if ("<%=PRESSURE.value%>" >= t2) {
                         if (t2 == "<%=PRESSURE.value%>") { return }
                         t2 += interval2;
                         gauge_psi.needle.setValue(t2);
                         gauge_psi.label.setText(Math.round(t2));
                     }
                     if ("<%=PRESSURE.value%>" < t2) {
                         if (t2 == "<%=PRESSURE.value%>") { return }
                         t2 -= interval2;
                         gauge_psi.needle.setValue(t2);
                         gauge_psi.label.setText(Math.round(t2));
                     }
                 }
                 setInterval(updateGauge2, interval2);

                 function updateGauge3() {
                     if ("<%=FLOW.value%>" >= t3) {
                         if (t3 == "<%=FLOW.value%>") { return }
                         t3 += interval3;
                         gauge_gpm.needle.setValue(t3);
                         gauge_gpm.label.setText(Math.round(t3));
                     }
                     if ("<%=FLOW.value%>" < t3) {
                         if (t3 == "<%=FLOW.value%>") { return }
                         t3 -= interval3;
                         gauge_gpm.needle.setValue(t3);
                         gauge_gpm.label.setText(Math.round(t3));
                     }
                 }
                 setInterval(updateGauge3, interval3);
    </script>
    </ContentTemplate>
    </asp:UpdatePanel>
  

    <!--Rows of panels for displaying main values-->
    <asp:UpdatePanel ID="MainPagePanel" runat="server" UpdateMode="Conditional"> 
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" />
        </Triggers>
        <ContentTemplate>
            

    <div class="row state-overview">
        <!-- Flow rate panel-->
        <div class="col-md-4">
            <section class="panel" style="background-color:silver; justify-content:center">
                <div class="value">
                    <h1>Flow Rate</h1>
                    <!--<asp:DropDownList runat="server" ID="FlowList" OnTextChanged="flowChange">
                        <asp:ListItem>LOW</asp:ListItem>
                        <asp:ListItem>MEDIUM</asp:ListItem>
                        <asp:ListItem>HIGH</asp:ListItem>
                        <asp:ListItem>MAX</asp:ListItem>
                    </asp:DropDownList>-->
                    <h3><asp:Label ID="flowlbl" runat="server"></asp:Label> Gallons/min</h3>
                </div>
                <!--<a class="btn btn-default" href="\WebApplication2\DataGraphs">Change Flow Rate &raquo;</a>-->
            </section>
        </div>
        
        <!--Frequency panel-->
        <div class="col-md-4">
            <section class="panel" style="background-color:silver; justify-content:center">
                <div class="value">
                    <h1>Frequency</h1>
                    <h3><asp:Label ID="freqlbl" runat="server"></asp:Label> Hz</h3>
                </div>
                <!--<a class="btn btn-default" href="\WebApplication2\DataGraphs">Change Frequency &raquo;</a> <!-- for localhost webapp, need \WebApplication2\ in links-->
            </section>
        </div>
        <!-- Power panel -->
        <div class="col-md-4" >
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>Power Consumption</h1>
                    <h3><asp:Label ID="powerlbl" runat="server"></asp:Label> Watts</h3>
                    <!--<a class="btn btn-default" href="\DataGraphs">Veiw Power &raquo;</a>-->
                </div>
            </section>
        </div>
        <!-- Cost panel-->
        <div class="col-md-4">
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>Energy Cost</h1>
                    <h3> $ <asp:Label ID="costlbl" runat="server"></asp:Label></h3>
                    <!--<a class="btn btn-default" href="\DataGraphs">View Cost &raquo;</a>-->
                </div>
            </section>
        </div>
        <!-- RPM panel-->
        <div class="col-md-4">
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>RPM</h1>
                    <h3><asp:Label ID="rpmlbl" runat="server"></asp:Label> rpm</h3>
                    <!--<a class="btn btn-default" href="\DataGraphs">View Cost &raquo;</a>-->
                </div>
            </section>
        </div>
        <!-- Pressure panel-->
        <div class="col-md-4">
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>Pressure</h1>
                    <h3><asp:Label ID="preslbl" runat="server"></asp:Label> psi</h3>
                    <!--<a class="btn btn-default" href="\DataGraphs">View Cost &raquo;</a>-->
                </div>
            </section>
        </div>
    </div>

    </ContentTemplate>
    </asp:UpdatePanel>

    <br />
   
    <div class="row">
        <div class="col-md-4">
            <h2>System Overview</h2>
            <p>
                Click below to see all the values for the sensors throughout the pool pump system.
                A table shows the most recent values for VFD frequency, flow rate, pressure, pool temperature, and power consumption.
            </p>
            <p>
                <a class="btn btn-default" href="\DataGraphs">Data Graphs &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>View Savings</h2>
            <p>
                Click below to run a simulation on the pool pump system with and without the VFD controling the system. 
                With a one minute simulation you can see quickly the change in power use and energy cost. When the VFD is turned on,
                you can set the flow rate of your pool pump, so you know you are getting the desired pool functionality.These simulations 
                visually show you how much money a VFD can save you on your energy bill!
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Set VFD operation times</h2>
            <p>
                You can set up a schedule for times when the VFD is operating at different levels. This is useful for cases when you want to
                run your pool pump at maximum consumption at times when energy is the cheapest, like at night. This scheduling tool allows you to use the 
                VFD to minimize power consumption and thus energy cost while also minimizing work for you monitoring the system. Just set up
                the schedule and start saving money!
            </p>
            <p>
                <a class="btn btn-default" href="\Schedule">Set VFD Schedule &raquo;</a>
            </p>
            <details>
                <summary>Set VFD Schedule</summary>
                
                <script><% setPumpHours(); %></script>
                
                Hours of pool pump operation: <br />
                Start time: <asp:DropDownList runat="server" ID="pumpStart"></asp:DropDownList> <br />
                Stop time: <asp:DropDownList runat="server" ID="pumpStop"></asp:DropDownList> <br />

                <br />
                Hours of VFD control: <br />
                Start time: <asp:DropDownList runat="server" ID="vfdstart"></asp:DropDownList> <br />
                Stop time: <asp:DropDownList runat="server" ID="vfdstop" OnSelectedIndexChanged="vfdstop_SelectedIndexChanged" ></asp:DropDownList> <br />

                <br />
                Flow Rate: <asp:DropDownList ID="flowdropdown" OnSelectedIndexChanged="vfdstop_SelectedIndexChanged" runat="server">
                        <asp:ListItem>LOW</asp:ListItem>
                        <asp:ListItem>MEDIUM</asp:ListItem>
                        <asp:ListItem>HIGH</asp:ListItem>
                           </asp:DropDownList> <br />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"> 
                <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="setsched" />
                </Triggers>
                <ContentTemplate>
                <asp:Button runat="server" id="setsched" Text="Set Schedule" OnClick="schedbuttonClick" />
                <br />
                Estimated Monthly Energy Cost: <asp:Label runat="server" ID="schedcost"></asp:Label>
                </ContentTemplate>
                </asp:UpdatePanel>
            </details>
        </div>
    </div>


</asp:Content>
