using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PISDK;
using PITimeServer;
using System.Data;

namespace WebApplication2
{
    public partial class _Default : Page
    {
        PISDK.Server PI_SERVER;

        public struct pointData
        {
            public string timestamp;
            public string value;
        }
        public List<string> hours = new List<string>(){"12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM",
                                                "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM",
                                                "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"};

        public pointData rpiCONNECT;
        public pointData RPM;
        public pointData FLOW;
        public pointData PRESSURE;
        public pointData POWER;
        public pointData TEMP;     // fo rmain page only need: flow, freq, power, and cost
        public float cost;
        public string lastValueTime;
        Timer schedtimer;


        protected void Page_Load(object sender, EventArgs e)
        {
            PISDK.PISDK SDK = new PISDK.PISDK();            //Creates new instance of PI SDK
            PI_SERVER = SDK.Servers["esmartserver-pc"];     //Assign PI server to local machine [Piservername]
            PI_SERVER.Open("piadmin");          //Open connection through default user

            //Initialize point data starting at 12:50p
            rpiCONNECT = updatePointValue("SP14VICE_Connection", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_Flow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            POWER = updatePointValue("F13APA_POWER_BOT1", "12/6/2013 4:50:00 PM", DateTime.Now.ToString());


        }

        public pointData updatePointValue(string tagName, string start, string end)
        {
            PISDK.PIValues PIconnect = new PISDK.PIValues();  //Create new instance of PI value
            PIconnect = PI_SERVER.PIPoints[tagName].Data.RecordedValues(start, end);
            pointData temp = new pointData();

            foreach (PIValue val in PIconnect)
            {
                if (val.Value as DigitalState == null) // if point has data, Digital state means point has no data
                {
                    object objtemp = val.Value;
                    temp.value = objtemp.ToString();
                    objtemp = val.TimeStamp.LocalDate;
                    temp.timestamp = objtemp.ToString();
                }
            }
            lastValueTime = end;
            return temp;
        }

        protected void flowChange(object sender, EventArgs e)
        {
            string flow = "0";
            switch (FlowList.Text)
            {
                case "LOW": flow = "40"; break;
                case "MEDIUM": flow = "48"; break;
                case "HIGH": flow = "55"; break;
                case "MAX": flow = "60"; break;
                default: break;
            }
            setPointValue("SP14VICE_DesiredFlow", flow);
        }

        public void setPointValue(string tagName, string value)
        {
            PISDK.PIValue PIconnect = new PISDK.PIValue();  //Create new instance of PI value
            PIconnect.Value = value;
            PITimeServer.PITime time = new PITimeServer.PITime();
            PIconnect.TimeStamp = time;
            PI_SERVER.PIPoints[tagName].Data.UpdateValue(PIconnect, "*", PISDK.DataMergeConstants.dmInsertDuplicates, null);
        }

        public float findCost(int hours, float rate)
        {
            float Energy = float.Parse(POWER.value) * hours / 1000;
            return Energy * rate;
        }

        public void schedbuttonClick(object sender, EventArgs e)
        {
            //int start = 0, stop = 0;
            //float tempcost = 0;
            //for (int i = 0; i < 24; i++)
            //{
            //    if (hours.ElementAt(i) == vfdstart.Text) { start = i; }
            //    if (hours.ElementAt(i) == vfdstop.Text) { stop = i; }
            //}
            //tempcost = findCost(stop - start, (float)0.35); // uses current power, need to change for power of flow rate
            schedcost.Text = flowdropdown.Text;
           // await System.Threading.Tasks.Task.Delay(3000); //method must be async
        }

        
        
    }//end
}