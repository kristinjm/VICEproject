using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace VICE_Final
{
    public partial class DataMGT : Form
    {
        //public class PointVal                     //class to store date/time for updating PI DB
        //{
        //    public string date;
        //    public string value;
        //}
        public PythonComm PythonComm;            //instantiate TCP basic send class
        TcpClient connect = new TcpClient();     //instantiate asynchronous server 
        
        OSIPI myPI = new OSIPI();                //new PI connection
        string get = "teststring";
        //string get2 = "change";
        int count = 0;
        int oldLength;
        int outlet = 1; //store outlet # as well as text for updating PI
        int stream = 0;
        string EWO2 = "192.168.144.139";
        string EWO1 = "192.168.144.147";

        public DataMGT()
        {
            PythonComm = new PythonComm();    //initialize tcp/ip control
            InitializeComponent();            //load form
        }

        public void gotString(string message)
        {

            string[] com = message.Split(',');      //split string on ',' chars into a string array

            foreach (string s in com)                  //for each string in array split again on '='
            {
                string[] eq = s.Split('=');
                if (eq.Length > 1)                    //if successful split
                    try
                    {
                        parseVal(eq[0], eq[1]);        //call parse function
                        //Console.WriteLine(" In Parser {0}",eq[0]);
                        Validate();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" {0} Can not parse value", e);
                    }

            }
        }
        public void parseVal(string letter, string value)
        {
            string lockVal = "SP14VICE_Lock";
            string connection = "SP14VICE_Connection";
            string frequency = "SP14VICE_Freq";
            string vfdState = "SP14VICE_VfdState";
            string temp = "SP14VICE_Temp"; 
            string flow = "SP14VICE_Flow";
            string desiredFlow = "SP14VICE_DesiredFlow";
            string pressure = "SP14VICE_Pressure";
            string voltage = "F13APA_Voltage";
            string currenttop = "F13APA_CurrentTOP";
            string currentbot = "F13APA_CurrentBOTTOM";
            string powertop = "F13APA_Power_Top";
            string powerbot = "F13APA_Power_Bot";
            switch (letter)                        //checks first value in string for command
            {
                case "C":                          //if connection
                    
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(connection, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "L":                          //if connection

                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(lockVal, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "FR":                          //if frequency

                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(frequency, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "VS":                          //if VFD State

                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(vfdState, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "PR":                          //if Pressure
                   
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(pressure, (float.Parse(value)));      
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "T":                          //if Temperature
                   
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(temp, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "F":                          //if Flow

                    try
                    {


                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(flow, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "DF":                          //if DesiredFlow

                    try
                    {

                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(desiredFlow, (float.Parse(value)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "V":                          //if voltage
                    string vtemp = voltage + outlet.ToString();
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(vtemp, ((((float.Parse(value))*120)/Math.Pow(2,23))/.6));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "IT":                            //if current TOP outlet
                    string currenttoptemp = currenttop + outlet.ToString();
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(currenttoptemp, ((((float.Parse(value))*15)/(Math.Pow(2,24)-1))/.6));
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "IB":                                  //if current BOT outlet
                    string currentbottemp = currentbot + outlet.ToString();
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(currentbottemp, ((((float.Parse(value)) * 15) / (Math.Pow(2, 24) - 1)) / .6));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "PT":                            //POWER TOP
                    string powertoptemp = powertop + outlet.ToString();
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(powertoptemp, 1-((float.Parse(value))/(Math.Pow(2,23)-1)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "PB":                                   //POWER BOT
                    string powerbottemp = powerbot + outlet.ToString();
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue(powerbottemp, 1-((float.Parse(value)) / (Math.Pow(2, 23) - 1)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;
                case "S":                         //set outlet Status
                    outlet = int.Parse(value);
                    break;
                case "STM":                         //set outlet Status
                    stream = int.Parse(value);
                    break;
                case "OO":                         //command from Web Service
                    try
                    {
                        myPI.connect_Server("ESMARTSERVER-PC");

                        if (myPI.check_connection())
                        {
                            //Setting the value of the point
                            myPI.setPiPointValue("F13APA_STATUS", int.Parse(value));
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    switch (value)                     //case for outlet switching from web service
                    {
                        case "00":
                            {
                                if (outlet == 1)
                                    PythonComm.sendTCP(EWO1, 9750, "<,RT=0,RB=0,TM="+stream.ToString()+",>");
                                else if (outlet == 2)
                                    PythonComm.sendTCP(EWO2, 9750, "<,RT=0,RB=0,TM=" + stream.ToString() + ",>");

                            }
                            break;
                        case "11":
                            {
                                if (outlet == 1)
                                    PythonComm.sendTCP(EWO1, 9750, "<,RT=1,RB=1,TM=" + stream.ToString() + ",>");
                                else if (outlet == 2)
                                    PythonComm.sendTCP(EWO2, 9750, "<,RT=1,RB=1,TM=" + stream.ToString() + ",>");
                                
                            }
                            break;
                        case "10":
                            {
                                if (outlet == 1)
                                    PythonComm.sendTCP(EWO1, 9750, "<,RT=1,RB=0,TM=" + stream.ToString() + ",>");
                                else if (outlet == 2)
                                    PythonComm.sendTCP(EWO2, 9750, "<,RT=1,RB=0,TM=" + stream.ToString() + ",>");
                            }
                            break;
                        case "01":
                            {
                                if (outlet == 1)
                                    PythonComm.sendTCP(EWO1, 9750, "<,RT=0,RB=1,TM=" + stream.ToString() + ",>");
                                else if (outlet == 2)
                                    PythonComm.sendTCP(EWO2, 9750, "<,RT=0,RB=1,TM=" + stream.ToString() + ",>");
                            }
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)               //test function for sending TCP messages
        {
            // string get2 = get;
            string ret = "noret";
            //int getint = Convert.ToInt32(get2);
            if (get == "")                                                           //if text box empty
                PythonComm.sendTCP("192.168.144.128", 9750, "Blank Text Box");
            //PythonComm.sendTCP("130.191.222.107", 12345, "Blank Text Box");     //send default message
            else
                PythonComm.sendTCP("192.168.144.128", 9750, get);
            //PythonComm.sendTCP("130.191.222.107", 12345, get);                   //else send written message

            Console.WriteLine(ret);
            //TextBox2.Text = ret;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                PythonComm.recTCP("192.168.144.128", 9750, "Rec Text");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            get = textBox1.Text;                      //get text for terminal debugging
            Validate();                        
        }

        public string getchar()              //more debug functions for sending TCP messages
        {
            string t = textBox1.Text;
            string lastChar = string.Empty;
            if (t.Length > 1)
                lastChar = t.Substring(t.Length - 1, 1);
            else
                lastChar = "Moar";

            oldLength = t.Length;
            return lastChar;
        }

        public bool newChar()
        {
            string t = textBox1.Text;
            if (t.Length == oldLength)
                return true;
            else
                return false;
        }


        private void button3_Click(object sender, EventArgs e)   //debug form for PI DB testing
        {
            Form1 newPiform = new Form1();                //new pi update test form
            newPiform.Show();
        }

        private void button4_Click(object sender, EventArgs e)   //debug for terminal testing
        {
            PythonComm.term("192.168.144.124", 9750, "Starting Terminal", 200);
        }


         private void textBox8_TextChanged(object sender, EventArgs e)       //terminal debugging
         {
             NetworkStream stream = connect.GetStream();
             StreamWriter sw = new StreamWriter(stream);
             string t = textBox8.Text;
             string lastChar = string.Empty;
             if (t.Length > 1)
                 lastChar = t.Substring(t.Length - 1, 1);
             else
                 lastChar = "Moar";

             oldLength = t.Length;
             //return lastChar;
             sw.Write(lastChar);

         }


         private void button10_Click(object sender, EventArgs e)     //start asynchronous server
         {
             String[] moon = new String[2];
             int i = AsynchTCP.moon(moon);
         }

    }
}
