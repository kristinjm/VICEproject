using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PISDK;

namespace PI_Inserter
{
    public class OSIPI
    {
        PISDK.Server piServer;                                      //Create instance of PI Server                                    

        //<<<<<<<<<<Connect to PI Server>>>>>>>>>>
        public void connectToServer(string piServerName)
        {
            PISDK.PISDK SDK = new PISDK.PISDK();                    // Create new instance of PI SDK
            piServer = SDK.Servers[piServerName];                   // Assign PI Server to local machine
            piServer.Open(piServer.DefaultUser);                    // Open connection through default user
        }

        //<<<<<<<<<<Check if connection established>>>>>>>>>>
        public Boolean isConnectedToServer()
        {
            if (piServer != null) return piServer.Connected;        // Return true if connected
            else return false;                                      // Return false is not connected
        }

        //<<<<<<<<<< Connect if not already connected >>>>>>>>>>
        public void connectIfNotConnected(string piServerName)
        {
            if (piServer == null)
            {
                connectToServer("esmartserver-pc");
            }
        }

        //<<<<<<<<<<Disconnect from the server>>>>>>>>>>
        public void dissconnectFromServer()
        {
            try
            {
                piServer.Close();                                       // Close the connection
            }
            catch
            {
            }
        }

        //<<<<<<<<<<Add data entry given a tag name >>>>>>>>>>
        public void setPiPointValue(string tagName, object value)
        {
            PISDK.PIValue piValue = new PISDK.PIValue();            // Create new instance of PIValue
            piValue.Value = value;                                  // Set the Pi Value

            PITimeServer.PITime piTime = new PITimeServer.PITime(); // Create new instance of PITime
            piValue.TimeStamp = piTime;                             //Set the datetime   

            //Add entry to server
            piServer.PIPoints[tagName].Data.UpdateValue(piValue, "*", PISDK.DataMergeConstants.dmInsertDuplicates, null);
        }

        //<<<<<<<<<<Retrieve current value from existing PI Point>>>>>>>>>>
        public object getCurrentValue(String tagName)
        {
            //Retrive the latest value given the tage name
            PIValues val = piServer.PIPoints[tagName].Data.RecordedValuesByCount("*", 1, DirectionConstants.dReverse);
            return val[1].Value;
        }

        //<<<<<<<<<<Retrieve all the values from existing PI Point>>>>>>>>>>
        //NOTE: does not work
        public List<string> getValues(String tagName, DateTime startTime, DateTime endTime, TimeSpan span)
        {
            List<string> Svalues = new List<string>();
            List<PIValue> values = new List<PIValue>();         // Create List instance
            PIPoint point = piServer.PIPoints[tagName];         // Create point instance
            DateTime tempTime = startTime;                      // Set a temp start time for while loop

            while (tempTime < endTime)                          // Loop until endtime is reached
            {
                //Add data to values and increase the tempTime by the timespan
                values.Add(point.Data.ArcValue(tempTime, RetrievalTypeConstants.rtAtOrBefore));
                tempTime += span;
            }

            //Transfer to String list
            foreach (PIValue value in values)
            {
                if (value.ValueAttributes.GetType().IsCOMObject)
                {
                    Svalues.Add((value.Value as DigitalState).Name.ToString());
                }
                else
                {
                    Svalues.Add((value.Value).ToString());
                }
            }
            return Svalues;
        }

        //<<<<<<<<<<Remove an existing Pi Point>>>>>>>>>>
        public void deletePiPoint(string tagName)
        {
            piServer.PIPoints.Remove(tagName);
        }
    }
}
