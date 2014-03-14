//*******************************************************************************
//
//******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PISDK;
using System.Windows.Forms;

namespace VICE_Final //name of project
{
    public class PointVal
    {
        public string date;
        public string value;
    }

    public class OSIPI
    {
        PISDK.Server PI_Server;  // Creates instance of Pi Server

        //Connection to Pi Server function

        public void connect_Server(string PiServerName)
        {

            PISDK.PISDK SDK = new PISDK.PISDK();  //Creates new instace of PI SDK
            PI_Server = SDK.Servers[PiServerName]; // Assign PI server to local machine
            PI_Server.Open(PI_Server.DefaultUser); // Open connection through default user

        }




        // Verify if connection is established

        public Boolean check_connection() //the boolean returns a boolean value (0 or 1)
        {
            if (PI_Server != null)   // loop  to check connectivity

                return PI_Server.Connected; // return true if conencted
            else
                return false;              // return false if not connected
        }



        // if the check_connection function return false connect to the server//

        public void connectserver(string PiServerName)
        {

            if (PI_Server == null)   //if not connected
            {
                connect_Server("esmartserver-pc"); // connect to the esmart connection 
            }                                     // PIserverName = esmartserver-pc


        }



        ///////Disconnect from server function//////////////////

        public void disconnectserver()
        {
            try
            {
                PI_Server.Close(); // this closes the PI_Server
            }

            catch
            {
            }

        }


        ////////////////add data entry given a tag name////////////////////////////

        public void setPiPointValue(string tagName, object value)
        {
            PISDK.PIValue PiValue = new PISDK.PIValue(); // Create new instance of PI value
            PiValue.Value = value; // set the Pi Value

            PITimeServer.PITime piTime = new PITimeServer.PITime(); // create new instance of PITime
            PiValue.TimeStamp = piTime;  // set datetime


            //////////////////// add entry to server//////////////////////////
            PI_Server.PIPoints[tagName].Data.UpdateValue(PiValue, "*", PISDK.DataMergeConstants.dmInsertDuplicates, null);


        }


        // REMOVE PI POINT//
        //must have value before trying to delete a value from there.
        //catch error?

        public void deletePiPoint(string tagName)
        {
            //try
            //{
            if (PI_Server.PIPoints != null)
            {

                PI_Server.PIPoints.Remove(tagName);
            }
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show("Insert a tag that already EXISTS!");
            //}
        }
        


        

        //////////////////Create a tag////////////////////////
        public void createPiPoint(string tagName, int pointIndex)
        {
            PointTypeConstants pointType = PointTypeConstants.pttypFloat32; // default type of tag created
            switch (pointIndex)
            {
                case 0:
                    pointType = PointTypeConstants.pttypDigital;
                    break;
                case 1:
                    pointType = PointTypeConstants.pttypFloat16;
                    break;
                default:
                    break;
            }


            PI_Server.PIPoints.Add(tagName, "classic", pointType);
        }

        ///////Display tags in certain time frame///////////////////
        public List<PointVal> searchPiPoints(string tagName, string startTime, string endTime)
        {
            //Create new list
            List<PointVal> list = new List<PointVal>();

            try
            {
                //Get point from tagname
                PIPoint point = PI_Server.PIPoints[tagName];

                //Get recorded values for selected time
                // in order to display only those points in the time frame choosen by user
                PIValues values = point.Data.RecordedValues(startTime, endTime);

                //Walk through values list and save each value
                foreach (PIValue value in values)
                {
                    //Create new value object
                    PointVal val = new PointVal();

                    //Save date from values timestamp
                    val.date = value.TimeStamp.LocalDate.ToString();

                    //Save the value, but first check to see if value is a digital state or an actual value
                    // when a tag is first created it does not have a value and therefore it does not
                    //contain a value. Instead is  displays " NO DATA" which means it is in digital state
                    DigitalState state = value.Value as DigitalState;

                    if (state != null)
                    {
                        //Not a value, it's a digital state
                        // it turns it into a string in order to be saved and displayed
                        val.value = (value.Value as DigitalState).Name.ToString();
                    }
                    else
                    {
                        //Convert value to object, then object to string
                        object obj = value.Value;
                        val.value = obj.ToString();
                    }

                    //Add new value to list
                    list.Add(val);
                }
            }
            catch (Exception ex)
            {
            }

           return list;// return the tags recorded on the list
        }
    }
}


