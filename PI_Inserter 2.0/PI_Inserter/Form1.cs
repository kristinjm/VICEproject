using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
/* This is a simple project to update a test PI point (ECE_TestPoint) every handful of seconds. */
namespace PI_Inserter
{
    public partial class Form1 : Form
    {
        OSIPI myPI = new OSIPI();
        float updatePointConnection = 1;
        int connectionCount = 0;
        public Form1()
        {
            InitializeComponent();
            label2.Text = "When you start this program, it updates the PI points from S14 with new values every 5 seconds."; //'SP14VICE_Temp', 'SP14VICE_Pressure', and 'SP14VICE_Flow'
            timer1.Interval = 5000;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Create the math manipulation at every tick //

            // flop the connection bit to 1 every 6 ticks
            if (connectionCount < 5)
            {
                updatePointConnection = 0;
                connectionCount++;
            }  
            else { 
                updatePointConnection = 1;
                connectionCount = 0;
            }
            float updatePointTemp = (float)(DateTime.Now.Minute + DateTime.Now.Second) / 3; //random generation equation for rpm
            float updatePointFlow = (float)(DateTime.Now.Minute * 4 + DateTime.Now.Second) / 3; //random generation equation for flow
            float updatePointPress = (float)(DateTime.Now.Minute / 7 + DateTime.Now.Second) / 3; //random generation equation for press
            manipulatePIPoint(updatePointTemp, updatePointFlow, updatePointPress, updatePointConnection);
            label1.Text = updatePointTemp.ToString();
            label3.Text = updatePointFlow.ToString();
            label4.Text = updatePointPress.ToString();
            label8.Text = updatePointConnection.ToString();
        }

        /// <summary>
        /// Updates PI point 'ECE_TestPoint' on the ESMARTSERVER-PC PI Server with a value manipulated by the current time.
        /// </summary>
        private void manipulatePIPoint(float value, float value2, float value3, float value4)
        {
            try
            {
                myPI.connectToServer("ESMARTSERVER-PC");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (myPI.isConnectedToServer())
                {

                    myPI.setPiPointValue("SP14VICE_Connection", myTruncate(value4, 2));
                    myPI.setPiPointValue("SP14VICE_Temp", myTruncate(value, 2));
                    myPI.setPiPointValue("SP14VICE_Pressure", myTruncate(value2, 2));
                    myPI.setPiPointValue("SP14VICE_Flow", myTruncate(value3, 2));
                }
            }
        }

        /// <summary>
        /// The radio checked event will handle enabling/disabling the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
            }
        }

        /// <summary>
        /// Truncates a float value to the specified number of digits
        /// </summary>
        /// <param name="value">The given float value</param>
        /// <param name="digits">The number of digits to truncate to</param>
        /// <returns></returns>
        public float myTruncate(float value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
