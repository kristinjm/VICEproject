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
    public partial class _Default : Page
    {
        PISDK.Server PI_SERVER;

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
        public string lastValueTime;


        protected void Page_Load(object sender, EventArgs e)
        {
            PISDK.PISDK SDK = new PISDK.PISDK();            //Creates new instance of PI SDK
            PI_SERVER = SDK.Servers["esmartserver-pc"];     //Assign PI server to local machine [Piservername]
            PI_SERVER.Open(PI_SERVER.DefaultUser);          //Open connection through default user

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

        protected void Button1_Click(object sender, EventArgs e)
        {
            rpiCONNECT = updatePointValue("SP14VICE_Connection", rpiCONNECT.timestamp, DateTime.Now.ToString());
            RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_Flow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
        }
    }
}