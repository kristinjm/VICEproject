using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Data;
using PISDK;
using PITimeServer;
using System.Windows.Forms;
using ZedGraph;


namespace WebApplication2
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        public List<string> hours = new List<string>(){"12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM",
                                                "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM",
                                                "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"};
        PISDK.Server PI_SERVER;

        public struct pointData
        {
            public string timestamp;
            public string value;
        }
        public int bestFlow = 2;
        public int maxFlow = 4;
        public int simcount = 0;
        public string lastValueTime;
        public int[] pwrlist = new int[60];
        public int[] energylist = new int[60];
        public int[] monthhours = new int[30];

        public ZedGraph.ZedGraphControl zgc;


        protected void Page_Load(object sender, EventArgs e)
        {
            ///// Connect to pi server /////
            //PISDK.PISDK SDK = new PISDK.PISDK();            //Creates new instance of PI SDK
            //PI_SERVER = SDK.Servers["esmartserver-pc"];     //Assign PI server to local machine [Piservername]
            //PI_SERVER.Open("piadmin");          //Open connection through default user

            ///// Set monthly hours array /////
            monthhours[0] = 4;
            for (int i = 1; i < 30; i++)
            {
                monthhours[i] = monthhours[i - 1] + 4; // add 4 hours for each day to run the pool pump
            }
            for (int i = 0; i < 60; i++)
            {
                pwrlist[i] = 17; //Watts for running VFD at 40Hz
            }
            int x = 0;
            for (int i = 0; i < 60; i += 2)
            {
                energylist[i] = pwrlist[i] * monthhours[x];
                energylist[i + 1] = pwrlist[i + 1] * monthhours[x];
                x++;
            }

            ///// case statment to determine OSI PI list start and stop times based on flow rates /////

            ///// For timer countdown /////
            if (!SM1.IsInAsyncPostBack)
                Session["timeout"] = DateTime.Now.AddSeconds(0).ToString();            
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (0 > DateTime.Compare(DateTime.Now,
                DateTime.Parse(Session["timeout"].ToString())))
            {
                Textlbl1.Text = "Number of seconds left: ";
                Timelbl.Text = ((Int32)DateTime.Parse(Session["timeout"].
                ToString()).Subtract(DateTime.Now).TotalSeconds).ToString();

                updateChart1(60-int.Parse(Timelbl.Text));
                if (Timelbl.Text != "0" && Timelbl.Text!="61")
                {
                    energylbl.Text = energylist[60 - int.Parse(Timelbl.Text)].ToString();
                    if (energylist[60 - int.Parse(Timelbl.Text)] > 300 && energylist[60 - int.Parse(Timelbl.Text)] < 600) tierlbl.Text = "Tier 1";
                }
            }

            if (Timelbl.Text == "0") // should only enter once, when simulation has just completed. after that timelbl will be "sim complete"
            {
                Textlbl1.Text = " ";
                Timelbl.Text = "Simulation Complete";
                updateChart1(60);
            }

            if (Timelbl.Text == "Simulation Complete")
            {
                //When sim is done, display results
                resultspan.InnerHtml = "Results:<br/>"+
                    "<div class='row'> <div class='col-sm-2'></div> <div class='col-md-4' style='color:green;'>With VFD: <br/>"+
                    "Pump ran at: "+ bestFlow.ToString()+" gpm"+"<br/>"+"Power Consumed: " + "kW" + "<br/>" + "Energy Cost: $"+
                    "</div> <div class='col-md-4'style='color:red;'> Without VFD: <br/>"+
                    "Pump ran at: "+ maxFlow.ToString()+" gpm"+"<br/>"+"Power Consumed: " + "kW" + "<br/>" + "Energy Cost: $"+
                    "</div></div>";
                updateChart1(60); //hold result on chart if a simulation has occurred
            }
            
        }

        public void setSim(object sender, EventArgs e)
        {
            if (seasonRadio.SelectedValue == "Summer") { bestFlow += 1; } // need a higher flow rate in the summer because the pool gets more use
            if (useRadio.SelectedValue == "heating") { bestFlow = maxFlow; } // to heat you want maximum flow rate

            //assign sim lists based on parameters
            for (int i = 0; i < 60; i++)
            {
                pwrlist[i] = 17; //Watts for running VFD at 40Hz
            }
            //pwrlist = getPointList("Power", startTime, stopTime); //select start and stop times for sim list based on the flow rate desired(will correspond to different lists in database) via switch statement
            int x = 0;
            for (int i = 0; i < 60; i+=2)
            {
                energylist[i] = pwrlist[i] * monthhours[x];
                energylist[i + 1] = pwrlist[i + 1] * monthhours[x];
                x++;
            }

            //Turn on vfd and pump

            ///// For timer countdown: add seconds to countdown /////
            if (!SM1.IsInAsyncPostBack)
                Session["timeout"] = DateTime.Now.AddSeconds(62).ToString();
        }

        public void updateChart1(int length)
        {  
            int[] xlist = new int[60]; //sample list 1
            for (int i = 0; i < length; i++)
            {
                xlist[i] = i;
            }
            int[] ylist = new int[60]; //sample list 2
            for(int i=0; i< length; i++){
                ylist[i]= energylist[i];
            }
            int[] fullxlist = new int[60];
            for (int i = 1; i <= 60; i++)
            {
                fullxlist[i-1] = i; // xlist = 1 to 60
            }
            int[] zerolist = new int[60];
            for (int i = 0; i < 60; i++)
            {    
                zerolist[i] = 0;
            }

            ////// Bind data to each series /////
            Chart1.Series["Without VFD"].Points.DataBindXY(xlist, ylist);
            Chart1.Series["Series2"].Points.DataBindXY(fullxlist, zerolist); // x and y = 1 to 60 to set up grid
            Chart1.Series["Series3"].Points.DataBindXY(zerolist, fullxlist);
            
        }

        public int[] getPointList(string tagName, string start, string end)
        {
            PISDK.PIValues PIconnect = new PISDK.PIValues();  //Create new instance of PI value
            PIconnect = PI_SERVER.PIPoints[tagName].Data.RecordedValues(start, end);
            pointData temp = new pointData();
            int counter = 0;
            int[] PIlist = new int[60];
            
            foreach (PIValue val in PIconnect)
            {
                if (val.Value as DigitalState == null) // if point has data, Digital state means point has no data
                {
                    object objtemp = val.Value;
                    temp.value = objtemp.ToString();
                    objtemp = val.TimeStamp.LocalDate;
                    temp.timestamp = objtemp.ToString();
                    PIlist[counter] = int.Parse(temp.value); // add each PI point to the list
                    counter++;
                }
            }
            lastValueTime = end;
            return PIlist;
        }

    }// end class
}// end namespace