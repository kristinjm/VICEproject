using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Linq;
using System.Web;

// State object for reading client data asynchronously
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

        public static void StartListening()
        {
                       
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.

            IPAddress ipAddress = IPAddress.Parse("192.168.144.144");
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

        public static void osiListen() 
        {

            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse("192.168.144.144");
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

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    if (!listener.Poll(300, SelectMode.SelectRead))
                    {
                        Console.WriteLine("Dong Dong Dong");
                    }


                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for Rasp PI connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);


                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            /****Search current VFD State PI value*****/
            OSIPI myPI = new OSIPI();
            myPI.connect_Server("ESMARTSERVER-PC");
            PointVal stateVal = null;
            PointVal currentVal = null;
          
            if (myPI.check_connection())
            {

                //put VFD state in a saved PI point
                List<PointVal> vfdState = myPI.searchPiPoints("SP14VICE_VfdState", DateTime.Now.ToString(), DateTime.Now.ToString());
                stateVal = vfdState.ElementAt(0);

            }


            while(true)
            {

                //get current OSI PI value for VFD
                if (myPI.check_connection())
                {
                    List<PointVal> vfdCurrentState = myPI.searchPiPoints("SP14VICE_VfdState", DateTime.Now.ToString(), DateTime.Now.ToString());
                    currentVal = vfdCurrentState.ElementAt(0);
                }

                //check if the OSI server value has changed
                if(currentVal != stateVal)
                {
                    //send current value to Rasp PI
                    stateVal = currentVal;
                    Send(listener, "<,S=" + stateVal.ToString() + ",EOF,>");

                }

                Thread.Sleep(5000);

            }
        }

        
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
                        Console.WriteLine("IPTest Error: {0} \n",ex.Message);
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

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

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


        public static int moon(String[] args)
        {
            //StartListening(); //thread to recieve Rasp PI/ALPACA data
            //Thread listenMonitor = new Thread(new ThreadStart(StartListening));  //thread to recieve Rasp PI/ALPACA data
            Thread listenMonitor = new Thread(StartListening); //thread to recieve Rasp PI/ALPACA data
            listenMonitor.Start(); //start listening
            Thread osiMonitor = new Thread(osiListen);  //thread to monitor OSI PI state
            osiMonitor.Start();
            return 0;
        }
    }

}