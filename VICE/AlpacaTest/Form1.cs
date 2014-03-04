using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlpacaFinal
{
    public partial class Form1 : Form
    {
        OSIPI myPI = new OSIPI();
        Random rand = new Random();
        Calculations mycal = new Calculations();        // Not needed for PI tag updating demonstration
        List<float> ValueList = new List<float>();      // all the user values are entered here


        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 15000;
            timer1.Enabled = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set the MinDate and MaxDate.
            dateTimePickerStart.MinDate = new DateTime(1985, 6, 20);// Can get data from this date.

            // The max date that we can choose is up to a date in advance..
            //the reason why I choose to add a day in advace is because without that that feature
            // once the program is running and a value is added then it would not let the user access anything 
            //that was added after the program was executed
            dateTimePickerStart.MaxDate = DateTime.Now + new TimeSpan(1, 0, 0, 0);

            // Set the CustomFormat string.
            dateTimePickerStart.CustomFormat = "hh:mm:ss tt MM/dd/yyyy"; // displays hours/min/sec/ampm/month/day/year in that order
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;

            // Set the MinDate and MaxDate.
            dateTimePickerEnd.MinDate = new DateTime(1985, 6, 20);
            dateTimePickerEnd.MaxDate = DateTime.Now + new TimeSpan(1, 0, 0, 0);

            // Set the CustomFormat string.
            dateTimePickerEnd.CustomFormat = "hh:mm:ss tt MM/dd/yyyy";
            dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
        }


        /// <summary>
        /// Creates a PI point on the PI server with a given value
        /// </summary>
        /// <param name="value"></param>
        private void createPIPoint(float value)
        {
            try
            {
                myPI.connect_Server("ESMARTSERVER-PC");

                if (myPI.check_connection())
                {
                    string usertag = textBox_tag.Text;//set new value to a certain point

                    //Creating a point
                    myPI.createPiPoint(usertag, 2);

                    // Setting the value of the point
                    // Also have the power to only update the value on an existing 
                    // tag instead of creating a new tag.
                    myPI.setPiPointValue(usertag, myTruncate(value, 6));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void manipulatePIPoint(float value)
        {
            try
            {
                myPI.connect_Server("ESMARTSERVER-PC");

                if (myPI.check_connection())
                {
                    string usertag = textBox_tag.Text;//set new value to a certain point

                    //Setting the value of the point
                    myPI.setPiPointValue(usertag, myTruncate(value, 6));

                    string delete = textBoxDelete.Text;
                    myPI.deletePiPoint(delete);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Updates a given PI point with a PI value
        /// </summary>
        /// <param name="tag">The PI point to be updated</param>
        /// <param name="value">The value to set the PI point to</param>
        private void updatePIPoint(string tag, float value)
        {
            try
            {
                myPI.connect_Server("ESMARTSERVER-PC");

                if (myPI.check_connection())
                {
                    //Setting the value of the point
                    myPI.setPiPointValue(tag, value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public float myTruncate(float value, int digits)// manages the computation for a random value being set to a point
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
        
        private void Enterbutton_Click(object sender, EventArgs e)
        {
            try
            {
                float updatePoint2 = float.Parse(textBox_tagvalue.Text);
                manipulatePIPoint(updatePoint2);
            }
            catch (Exception)
            {
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string delete = textBoxDelete.Text; // delete = what user enters
            myPI.deletePiPoint(delete); // deletes the tag the user enters
            listView1.Visible = false; // hides the listbox
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            //Check for empty tag
            if (textBoxDisplayTag.Text.Count() == 0)// if nothing is in textbox
            {
                MessageBox.Show("Invalid tag");// show error messege
                return;
            }

            if (dateTimePickerStart.Value > DateTime.Now)
            {
                MessageBox.Show("Invalid start time");
                return;
            }

            if (dateTimePickerEnd.Value > DateTime.Now)
            {
                MessageBox.Show("Invalid end time");
                return;
            }

            listView1.Items.Clear();// clear the list view box after new search
            listView1.Visible = true;

            myPI.connect_Server("ESMARTSERVER-PC");

            if (myPI.check_connection())
            {
                // create list with the points and the start/end times
                List<PointVal> list = myPI.searchPiPoints(textBoxDisplayTag.Text, dateTimePickerStart.Value.ToString(), dateTimePickerEnd.Value.ToString());
                

                foreach (PointVal item in list)
                {

                    //adds date and value in list view
                    ListViewItem listItem = listView1.Items.Add(item.date);
                    listItem.SubItems.Add(item.value);
                }
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                float updatePoint2 = float.Parse(textBox_tagvalue.Text);
                createPIPoint(updatePoint2);
            }
            catch (Exception)
            {
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateCost();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //float updatePoint = (float)(DateTime.Now.Minute + DateTime.Now.Second) / 3;
            //manipulatePIPoint(updatePoint);
            //label5.Text = updatePoint.ToString();

            UpdateCost();
        }

        private void UpdateCost()
        {
           // mycal.hours += (float)(rand.NextDouble() * 4.0f) + 1.0f;
           // mycal.kilowatts += (float)(rand.NextDouble() * 19.0f) + 1.0f;
            //mycal.power += (float)(rand.NextDouble() * 19.0f) + 1.0f;
            mycal.kilowatts += 400;
            mycal.Calculate();

            updatePIPoint("cost", mycal.Cost);

            this.textBoxCost.Text = mycal.Cost.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
