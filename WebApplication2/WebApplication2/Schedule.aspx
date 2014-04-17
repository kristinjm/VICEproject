<%@ Page Title="Schedule" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Schedule.aspx.cs" Inherits="WebApplication2.WebForm2" %>


<%@ Import Namespace="System.Data" %>
<script runat="server">
    public List<string> hours = new List<string>(){"12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM",
                                                "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM",
                                                "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"};
    public void setPumpHours()
    {
        
        pumpStart.DataSource = hours;
        pumpStart.DataBind();

        pumpStop.DataSource = hours;
        pumpStop.DataBind();

       // vfdstart.DataSource = hours;
       // vfdstart.DataBind();

        vfdstop.DataSource = CreateDataSource();
        vfdstop.DataTextField = "TextField";
        vfdstop.DataValueField = "ValueField";
        vfdstop.DataBind();
        vfdstop.SelectedIndex = 0;
    }

    ICollection CreateDataSource()
    {
        // Create a table to store data for the DropDownList control.
        DataTable dt = new DataTable();

        // Define the columns of the table.
        dt.Columns.Add(new DataColumn("TextField", typeof(String)));
        dt.Columns.Add(new DataColumn("ValueField", typeof(String)));
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


    public void setVFDhours()
    {
        int start = 0 , stop = 0;
        for (int i = 0; i < 24; i++)
        {
            if (hours.ElementAt(i) == pumpStart.Text) { start = i; }
            if (hours.ElementAt(i) == pumpStop.Text) { stop = i; }
        }
        for (int i = start; i < stop; i++)
        {
            vfdstart.Items.Add(hours.ElementAt(i));
            vfdstart.Items.Add("12:00 AM");
        }
        vfdstart.Items.Add("12:00 AM");
    }
    
</script>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        
        <script src="schedScript.js" type="text/javascript"></script>

        <h1> Pool Schedule</h1><br />
        
        <script><% setPumpHours(); %></script>
        Hours of pool pump operation: <br />
        Start time: <asp:DropDownList runat="server" ID="pumpStart"></asp:DropDownList> <br />
        Stop time: <asp:DropDownList runat="server" ID="pumpStop" OnTextChanged="pumpHourschanged"></asp:DropDownList> <br />

        <br />
        Hours of VFD control: <br />
        Start time: <asp:DropDownList runat="server" ID="vfdstart" OnSelectedIndexChanged="vfdstop_SelectedIndexChanged" OnTextChanged="vfdstop_SelectedIndexChanged">
                          <asp:ListItem>TIME</asp:ListItem>
                            <asp:ListItem>1</asp:ListItem>
                    </asp:DropDownList> <br />
        Stop time: <asp:DropDownList runat="server" ID="vfdstop" OnSelectedIndexChanged="vfdstop_SelectedIndexChanged">
                   </asp:DropDownList> <br />

        <br />
        Flow Rate: <asp:DropDownList runat="server">
                        <asp:ListItem>LOW</asp:ListItem>
                        <asp:ListItem>MEDIUM</asp:ListItem>
                        <asp:ListItem>HIGH</asp:ListItem>
                   </asp:DropDownList> <br />
        <br />
        <input type="button" id="setsched" value="Set Schedule" />
        <br />
        Estimated Monthly Energy Cost: <asp:Label runat="server" ID="cost"></asp:Label>

        <br /><br />
        <h3>Energy Cost breakdown</h3>
        <img src="tieredrates.JPG" width="550" /><br />
        Baseline : 346 kWH<br />
        100 - 130% of Baseline : 347 - 450 kWH<br />
        131 - 200% of Baseline : 451 - 692 kWH<br />
        More than 200% : 692 kWH +<br />
        (Prices are based on averages. Actual prices may vary)
<!--        <asp:Calendar runat="server">
            <DayStyle />
            
        </asp:Calendar>

        <div class="plotArea">
            <div class="border-head"></div>
            <div class="custom-bar-chart">
            <ul class="y-axis">
                <li><span>12:00 PM</span></li>
                <li><span>12:00 AM</span></li>
            </ul>
                <div class="bar"></div>
            </div>
        </div>
<!--        <asp:Table runat="server">
            <asp:TableRow>
                <asp:TableHeaderCell> Hour </asp:TableHeaderCell>
                <asp:TableHeaderCell> Pool Pump Status </asp:TableHeaderCell>
                <asp:TableHeaderCell> VFD Status </asp:TableHeaderCell>
                <asp:TableHeaderCell> Flow Rate </asp:TableHeaderCell>
                
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell> 12:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem Selected="True">ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell> 1:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem Selected="True">ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 2:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem Selected="True">ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 3:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem Selected="True">ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 4:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 5:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 6:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 7:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 8:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 9:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 10:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 11:00 AM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 12:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 1:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 2:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 3:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 4:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 5:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 6:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 7:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 8:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 9:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 10:00 PM </asp:TableCell>
                <asp:TableCell><asp:DropDownList runat="server">
                                    <asp:ListItem>OFF</asp:ListItem>
                                    <asp:ListItem>ON</asp:ListItem>
                               </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell> 11:00 PM </asp:TableCell>
                <asp:TableCell>
                    <asp:CheckBox runat="server" />
                    <asp:Label runat="server">OFF</asp:Label>
                </asp:TableCell>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>

        </asp:Table>   -->

</asp:Content>
