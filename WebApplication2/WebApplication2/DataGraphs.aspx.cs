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
        public pointData FREQ;
        public pointData FLOW;
        public pointData DF;
        public pointData PRESSURE;
        public pointData POWER;
        public pointData TEMP;
        public pointData USERLOCK;
        public pointData COST;
        public pointData VOLT;
        public pointData CURR;

        public string lastValueTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            PISDK.PISDK SDK = new PISDK.PISDK();            //Creates new instance of PI SDK
            PI_SERVER = SDK.Servers["esmartserver-pc"];     //Assign PI server to local machine [Piservername]
            PI_SERVER.Open("piadmin");          //Open connection through default user

            //Initialize point data starting at 12:50p
            rpiCONNECT = updatePointValue("SP14VICE_Connection", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FREQ = updatePointValue("SP14VICE_Freq", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            RPM = updatePointValue("SP14VICE_RPM", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_Flow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            DF = updatePointValue("SP14VICE_DesiredFlow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            VOLT = updatePointValue("SP14VICE_TopVoltage1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            CURR = updatePointValue("SP14VICE_TopCurrent1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            POWER = updatePointValue("SP14VICE_TopPower1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString()); 
            TEMP = updatePointValue("SP14VICE_Temp", "3/13/2013 1:05:00 PM", DateTime.Now.ToString());
            USERLOCK = updatePointValue("SP14VICE_Lock", "12/6/2013 4:50:00 PM", DateTime.Now.ToString()); // 1= unlocked, 0 = locked

            COST.value = findCost(120, (float)0.35).ToString(); //Assume: running pump for 4 hrs/day for 30 days; tier 3 pricing during summer ($.35/kWH)
            COST.timestamp = DateTime.Now.ToString();

            setTableValues();

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            
            rpiCONNECT = updatePointValue("SP14VICE_Connection", rpiCONNECT.timestamp, DateTime.Now.ToString());
            USERLOCK = updatePointValue("SP14VICE_Lock", "3/6/2014 1:20:00 PM", DateTime.Now.ToString()); // 1= unlocked, 0 = locked            

            RPM = updatePointValue("SP14VICE_RPM", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            FREQ = updatePointValue("SP14VICE_Freq", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            FLOW = updatePointValue("SP14VICE_Flow", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            DF = updatePointValue("SP14VICE_DesiredFlow", "2/27/2014 12:50:00 PM", DateTime.Now.ToString());
            PRESSURE = updatePointValue("SP14VICE_Pressure", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            TEMP = updatePointValue("SP14VICE_Temp", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());

            VOLT = updatePointValue("SP14VICE_TopVoltage1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            CURR = updatePointValue("SP14VICE_TopCurrent1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());
            POWER = updatePointValue("SP14VICE_TopPower1", "3/6/2014 1:20:00 PM", DateTime.Now.ToString());

            COST.value = findCost(120, (float)0.35).ToString(); //Assume: running pump for 4 hrs/day for 30 days; tier 3 pricing during summer ($.35/kWH)
            COST.timestamp = DateTime.Now.ToString();

            setTableValues();
            
        }

        public void setTableValues()
        {
            ///// Raspberry Pi Connection /////
            ConnectValue.Text = rpiCONNECT.value;
            ConnectTimestamp.Text = rpiCONNECT.timestamp;
            ConnectStatus.Text = "connected";
            ConnectStatus.ForeColor = System.Drawing.Color.Green;

            ///// User Input Lock /////
            LockValue.Text = USERLOCK.value;
            LockTimestamp.Text = USERLOCK.timestamp;
            if (USERLOCK.value == "1") Lockstatus.Text = "OFF - UNLOCKED";
            else if (USERLOCK.value == "0") Lockstatus.Text = "ON - LOCKED";

            ///// RPM /////
            RPMvalue.Text = RPM.value;
            RPMtimestamp.Text = RPM.timestamp;
            RPMstatus.Text = "high";

            ///// Frequency /////
            freqvalue.Text = FREQ.value;
            Freqtimestamp.Text = FREQ.timestamp;
            Freqstatus.Text = "high";

            ///// Flow Rate /////
            FlowValue.Text = FLOW.value;
            Flowtimestamp.Text = FLOW.timestamp;
            Flowstatus.Text = "high";

            ///// Desired Flow Rate /////
            DesiredFlowValue.Text = DF.value;
            DFtimestamp.Text = DF.timestamp;
            DFstatus.Text = "high";

            ///// Pressure /////
            PresValue.Text = PRESSURE.value;
            Prestimestamp.Text = PRESSURE.timestamp;
            PresStatus.Text = "high";

            ///// Temperature /////
            TempValue.Text = TEMP.value;
            TempTimestamp.Text = TEMP.timestamp;
            TempStatus.Text = "low";

            ///// Voltage /////
            VoltValue.Text = VOLT.value;
            VoltTimestamp.Text = VOLT.timestamp;
            VoltStatus.Text = "high";

            ///// Current /////
            CurrValue.Text = CURR.value;
            CurrTimestamp.Text = CURR.timestamp;
            CurrStatus.Text = "high";

            ///// Power /////
            PowerValue.Text = POWER.value;
            PowerTimestamp.Text = POWER.timestamp;
            PowerStatus.Text = "high";

            ///// Monthly Cost /////
            costvalue.Text = COST.value;
            costTimestamp.Text = COST.timestamp;
            costStatus.Text = "high";

            ///// Updating Time Stamp /////
            timelbl.Text = lastValueTime;
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
            if (USERLOCK.value == "1") // if user lock is unlocked
            {
                PISDK.PIValue PIconnect = new PISDK.PIValue();  //Create new instance of PI value
                PIconnect.Value = value;
                PITimeServer.PITime time = new PITimeServer.PITime();
                PIconnect.TimeStamp = time;
                PI_SERVER.PIPoints[tagName].Data.UpdateValue(PIconnect, "*", PISDK.DataMergeConstants.dmInsertDuplicates, null);
            }
        }

        protected void changeTableValues(object sender, EventArgs e)
        {
            //setPointValue("SP14VICE_Connection", "1.0");
            //setPointValue("SP14VICE_Freq", freqvalue.Text);
            
            
            //setPointValue("SP14VICE_Pressure", PresValue.Text);

            //Timer1_Tick(sender, e);

            setPointValue("SP14VICE_RPM", RPMvalue.Text);
            setPointValue("SP14VICE_DesiredFlow", DesiredFlowValue.Text);
        }

        public float findCost(int hours, float rate)
        {
            float Energy = float.Parse(POWER.value) * hours; // finds kWh (W/1000)
            return Energy * rate; // kWh * $/kWh = $
        }

        protected void RPMvalue_TextChanged(object sender, EventArgs e)
        {
            setPointValue("SP14VICE_RPM", "50");
        }

        protected void DesiredFlowValue_TextChanged(object sender, EventArgs e)
        {
            setPointValue("SP14VICE_DesiredFlow", "3");
        }

    }
}