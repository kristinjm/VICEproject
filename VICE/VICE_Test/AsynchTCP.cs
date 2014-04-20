using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Linq;
using System.Web;

// State object for reading/ sending client data asynchronously 
namespace VICE_Final
{

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchTCP
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static OSIPI _myPI;
        public static DataMGT _IpTest;
   

        public AsynchTCP()
        {
            //osisoft-pi.blogspot.com/2012/01/retrieve-point-value-changes-from-pi.html
        }

        /// <summary>
        /// Connect to devices and listen for incoming data
        /// </summary>
        public static void StartListening()
        {
                       
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.

            IPAddress ipAddress = IPAddress.Parse("192.168.144.111");
            int port = 9750;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                //listener.Connect(localEndPoint);
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    if (!listener.Poll(300, SelectMode.SelectRead))
                    {
                        Console.WriteLine("Ding Ding Ding");
                    }
                          

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }


        /// <summary>
        /// Verify connection and begin recieving
        /// </summary>
        /// <param name="ar"></param>
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            Console.WriteLine("In acceptCallback...");
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        /// <summary>
        /// Read incoming data and send ACK
        /// </summary>
        /// <param name="ar"></param>
        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            _IpTest = new DataMGT();
            _myPI = new OSIPI();

            Console.WriteLine("In ReadCallBack...");

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                Console.WriteLine("Bytes>0...");
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                string msg1 = "<ACK>";
                string msg2 = "<,RT=0,RB=0,TM=1,>";
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 

                    DateTime time = DateTime.Now;              // Use current time
                    string format = "MMM ddd d HH:mm yyyy";    // Use this format
                    Console.WriteLine(time.ToString(format));
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    // Echo the data back to the client
                    try
                    {
                        _IpTest.gotString(content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("IPTest Error: {0} \n", ex.Message);
                    }


                    Send(handler, msg1);
                    //Send(handler, content);


                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        /// <summary>
        /// Send a string to a handler
        /// </summary>
        /// <param name="handler">Handler to send to</param>
        /// <param name="data">Desired string to send</param>
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        /// <summary>
        /// Complete sending data to remote device and close socket
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /*
         * ****************************************************************************************************************************************
         */


        /// <summary>
        /// Listen for changes in the OSI points and send changes to Rasp PI (VFD State, Freq, Lock, and Desired Flow Rate)
        /// </summary>
        public static void osiListen() 
        {

            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse("192.168.144.111");
            int port = 9700;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                //listener.Connect(localEndPoint);
                listener.Bind(localEndPoint);
                listener.Listen(100);

            
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    if (!listener.Poll(300, SelectMode.SelectRead))
                    {
                        Console.WriteLine("Dong Dong Dong");
                    }


                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for Rasp PI connection...");
                    listener.BeginAccept(new AsyncCallback(osiAcceptCallback), listener);


                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                    Console.WriteLine("Rasp PI connected!!!!");
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        /// <summary>
        /// Verify connection and begin listening to osi
        /// </summary>
        /// <param name="ar"></param>
        public static void osiAcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            Console.WriteLine("In osiAcceptCallback...");
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginSend(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(osiReadCallback), state);
        }

        /// <summary>
        /// Monitor PI points for changes
        /// </summary>
        /// <param name="ar"></param>
        public static void osiReadCallback(IAsyncResult ar)
        {
   
            Console.WriteLine("In osiReadCallBack...");

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            /****Search current VFD State and Frequency PI values*****/

            //initailize OSI PI and server
            OSIPI myPI = new OSIPI();
            myPI.connect_Server("ESMARTSERVER-PC");

            //Create PI values and Lists for each
            PointVal savedVfdState = null;
            PointVal currentVfdState = null;
            PointVal savedDesiredFlow = null;
            PointVal currentDesiredFlow = null;
            List<PointVal> vfdStateList;
            List<PointVal> desiredFlowList;
            bool firstRun = false;

            if (myPI.check_connection())
            {

                //put VFD state, frequency, lock, and desired floe rate in a saved PI points
                vfdStateList = myPI.searchPiPoints("SP14VICE_VfdState", DateTime.Now.ToString(), DateTime.Now.ToString());
                savedVfdState = currentVfdState = vfdStateList.ElementAt(0);

                desiredFlowList = myPI.searchPiPoints("SP14VICE_DesiredFlow", DateTime.Now.ToString(), DateTime.Now.ToString());
                savedDesiredFlow = currentDesiredFlow = desiredFlowList.ElementAt(0);

            }


            while (true)
            {

                //get current OSI PI value for VFD, frequency, lock, and desire flow rate
                if (myPI.check_connection())
                {
                    vfdStateList = myPI.searchPiPoints("SP14VICE_VfdState", DateTime.Now.ToString(), DateTime.Now.ToString());
                    currentVfdState = vfdStateList.ElementAt(0);

                    desiredFlowList = myPI.searchPiPoints("SP14VICE_DesiredFlow", DateTime.Now.ToString(), DateTime.Now.ToString());
                    currentDesiredFlow = desiredFlowList.ElementAt(0);
                }

                //check for first run to send initail DesiredFlow value
                if (firstRun == false)
                {
                    //Send to Rasp PI to initiate function to change flow via frequency
                    savedDesiredFlow = currentDesiredFlow;
                    osiSend(handler, "<,DF=" + savedDesiredFlow.value.ToString() + ",EOF,>");
                    firstRun = true;
                }

                //check if the OSI server value has changed for VfdState
                if (currentVfdState.value != savedVfdState.value)
                {

                    //send current value to Rasp PI
                    savedVfdState = currentVfdState;
                    osiSend(handler, "<,S=" + savedVfdState.value.ToString() + ",EOF,>");
                }


                //check if the OSI server value has changed for desired flow rate
                if (currentDesiredFlow.value != savedDesiredFlow.value)
                {

                    //send current value to Rasp PI
                    savedDesiredFlow = currentDesiredFlow;
                    osiSend(handler, "<,DF=" + savedDesiredFlow.value.ToString() + ",EOF,>");

                }

               
                Thread.Sleep(5000); //sleep 5 sec then update

            }
        }

        /// <summary>
        /// Send a string of new points to a handler
        /// </summary>
        /// <param name="handler">Handler to send to</param>
        /// <param name="data">Desired string to send</param>
        private static void osiSend(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(osiSendCallback), handler);
        }

        /// <summary>
        /// Complete sending data to remote device, keep socket open
        /// </summary>
        /// <param name="ar"></param>
        private static void osiSendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

         /*
         * ****************************************************************************************************************************************
         */

        /// <summary>
        /// Initiates two threads to run the applications 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int moon(String[] args)
        {
            Thread listenMonitor = new Thread(StartListening); //thread to recieve Rasp PI/ALPACA data
            listenMonitor.Start(); //start listening
            Thread osiMonitor = new Thread(osiListen);  //thread to monitor OSI PI state
            osiMonitor.Start();
            return 0;
        }

        
    }

}