<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApplication2.About" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script>

        function CreatePbdControl(DivID, CLSID, ObjectID, WIDTH, HEIGHT, URL) {
            var d = parent.window.frames("PBDisplay").document.getElementById(DivID);
            d.innerHTML =
              '<object classid=' + CLSID + ' id=' + ObjectID +
              ' width=' + WIDTH + ' height=' + HEIGHT + '> <param name="DisplayURL" value="' + URL +
              '"/>';
        }

        function DsplyPB(str) {
            var Path = self.location.href
            if (Path.substr(0, 4) == "file") {
                var re = /%20/gi;
                Path = Path.replace(re, " ");
                if (Path.charAt(7) == '/')
                    Path = Path.substr(8, 2) + "\\" + Path.substr(11, Path.length - 11);
            }
            lastslash = Path.lastIndexOf("/")
            var pathx
            if (lastslash != -1)
                pathx = Path.substr(0, lastslash)
            else
                pathx = Path.substr(0, 2)
            CreatePbdControl("DIV01_ID",
                             "clsid:4F26B906-2854-11D1-9597-00A0C931BFC8",
                             "pbdCtrl_01", "100%", "100%", pathx + '/' + str)
        }

</script>

    <h2><%: Title %>.</h2>
    <h3>Your application description page.</h3>
    <p>Use this area to provide additional information.</p>
    <br />
    <a href="seniordesignlab.com"> Visit Senior Design Website</a>
    <br />
    <h2><%: Title %> Pool Pump System Diagram</h2>
    <div>
       <img src="Project1.jpg" alt="System Diagram" width="500" height="300"/>
    </div>

       <button onclick="DsplyPB('PoolPumpSystem.PDI');">Display</button>
    <br />
    <br />
    <object id="pbdcrtl_01"><param name="DisplayURL" value="C:\Users\Kristin\Documents\CompE490-SeniorDesign\webapp\WebApplication2\WebApplication2\POOLPUMPSYSTEM.PDI" /></object>

</asp:Content>
