using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PISDK;
using PITimeServer;

namespace WebApplication2
{
    public partial class _DataGraphs : Page
    {
        PISDK.Server PI_SERVER;
        public string pwrtxt;

        public struct pointData
        {
            public string timestamp;
            public string value;
        }
        public pointData rpiCONNECT;
        public pointData RPM;
        public pointData FLOW;
        public pointData PRESSURE;
        public pointData POWER;
        public pointData TEMP;
        public string lastValueTime;

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
            TEMP = updatePointValue("SP14VICE_Temp", "3/13/2013 1:05:00 PM", DateTime.Now.ToString());

        }

        public pointData updatePointValue(string tagName, string start, string end)
        {
            PISDK.PIValues PIconnect = new PISDK.PIValues();  //Create new instance of PI value
            PIconnect = PI_SERVER.PIPoints[tagName].Data.RecordedValues(start, end);
            pointData temp = new pointData();
            int i = 0;

            foreach (PIValue val in PIconnect)
            {
                i++;
                if (val.Value as DigitalState == null && i<PIconnect.Count ) // if point has data, Digital state means point has no data
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

        public void setPointValue(string tagName, string value)
        {
            PISDK.PIValue PIconnect = new PISDK.PIValue();  //Create new instance of PI value
            PIconnect.Value = value;
            PITimeServer.PITime time = new PITimeServer.PITime();
            PIconnect.TimeStamp = time;
            PI_SERVER.PIPoints[tagName].Data.UpdateValue(PIconnect, "*", PISDK.DataMergeConstants.dmInsertDuplicates, null);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string frequency = "0";
            switch (Freq.Text)
            {
                case "LOW": frequency = "40"; break;
                case "MEDIUM": frequency = "48"; break;
                case "HIGH": frequency = "55"; break;
                case "MAX": frequency = "60"; break;
                default: break;
            }
            setPointValue("SP14VICE_RPM", frequency);

            //setPointValue("SP14VICE_Flow", FlowValue.Text);
            //setPointValue("SP14VICE_Pressure", PresValue.Text);
        }
    }
}