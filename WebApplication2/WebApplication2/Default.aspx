
<script runat="server">
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            rpiCONNECT = updatePointValue("SP14VICE_Connection", rpiCONNECT.timestamp, DateTime.Now.ToString());
            RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_Flow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            POWER = updatePointValue("F13APA_POWER_BOT1", "12/6/2013 4:50:00 PM", DateTime.Now.ToString());

            setTableValues();
            
        }

        public void setTableValues()
        {
            freqlbl.Text = RPM.value;
            powerlbl.Text = POWER.value;
            costlbl.Text = "0";
        }
    </script>

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

 
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% var time = DateTime.Now; %>
    
    <script>
       <% setTableValues(); %>
    </script>

    <div class="jumbotron" style="background-color:gold; min-height:300px;">
        <div class="col-md-4" id="column1" style="float:left; width:25%;">
            <img src="BadgeFinal1.png" alt="VICE Badge" width="170" height="190"/>
        </div>
        <div class="col-md-4" id="column2" style="float:left; width:70%;">
            <h1 style="font-family:'Book Antiqua';">V.I.C.E.</h1>
            <p> VFD Integration for Conservation of Energy </p>
            <p>A group of Electrical and Computer engineers designing a VFD system for residential use to conserve energy.</p>
        </div>

    </div>

    <br />
    
    <div class="row state-overview">
        <div class="col-lg-4 col-sm-6">
            <section class="panel" style="background-color:silver; justify-content:center">
                <div class="value">
                    <h1>Frequency</h1>
                    <h3><asp:Label ID="freqlbl" runat="server"></asp:Label> rpm</h3>
                </div>
                <a class="btn btn-default" href="\DataGraphs">Change Frequency &raquo;</a>
            </section>
        </div>
        <div class="col-lg-4 col-sm-6" >
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>Power Consumption</h1>
                    <h3><asp:Label ID="powerlbl" runat="server"></asp:Label> Watts</h3>
                    <a class="btn btn-default" href="\DataGraphs">Veiw Power &raquo;</a>
                </div>
            </section>
        </div>
        <div class="col-lg-4 col-sm-6">
            <section class="panel" style="background-color:silver">
                <div class="value" style="align-content:center">
                    <h1>Energy Cost</h1>
                    <h3> $ <asp:Label ID="costlbl" runat="server"></asp:Label></h3>
                    <a class="btn btn-default" href="\DataGraphs">View Cost &raquo;</a>
                </div>
            </section>
        </div>
    </div>

    <br />
   
    <div class="row">
        <div class="col-md-4">
            <h2>System Overview</h2>
            <p>
                This section contains diagrams that describe the system in your home.
            </p>
            <p>
                <a class="btn btn-default" href="\DataGraphs">Data Graphs &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Power Consumption</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Operate VFD</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>


</asp:Content>
