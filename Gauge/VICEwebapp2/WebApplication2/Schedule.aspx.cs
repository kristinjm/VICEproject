using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Data;


namespace WebApplication2
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        public List<string> hours = new List<string>(){"12:00 AM", "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM",
                                                "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM",
                                                "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM"};
   
        protected void Page_Load(object sender, EventArgs e)
        {
            Chart chart1 = new Chart();
            chart1.BackColor = System.Drawing.Color.Aquamarine;
            chart1.Visible = true;
        }

        public void pumpHourschanged(object sender, EventArgs e)
        {

            int start = 0, stop = 0;
            for (int i = 0; i < 24; i++)
            {
                if (hours.ElementAt(i) == pumpStart.Text) { start = i; vfdstart.Items.Add(hours.ElementAt(i)); }
                if (hours.ElementAt(i) == pumpStop.Text) { stop = i; vfdstart.Items.Add(hours.ElementAt(i)); }
            }
            for (int i = start; i < stop; i++)
            {
                vfdstart.Items.Add(hours.ElementAt(i));
            }

            vfdstart.Items.Add("12:00 AM");
        }

        protected void vfdstop_SelectedIndexChanged(object sender, EventArgs e)
        {
            cost.Text = "0";
            
        }
    }
}