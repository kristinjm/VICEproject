using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;

namespace AlpacaFinal
{
    public class PythonComm
    {

        public DataMGT IpTest;

        /*<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>
       Send a message over TCP/IP given the IP, PORT number, and message to be sent
       <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>*/
        public void sendTCP(string IP, int PORT, string message)
        {
            try
            {
                TcpClient connection = new TcpClient(IP, PORT);     //Create new connection to IP and PORT
                NetworkStream stream = connection.GetStream();      //Create a stream to pass the message
                StreamWriter sw = new StreamWriter(stream);         //Create a steam writer to write to

                sw.Write(message);      //Write the message to the writer
                Thread.Sleep(200);
                sw.Close();             //Close the stream writer since the message is writen so
                stream.Close();         //Close the stream 
                connection.Close();     //Close the connection
            }

            catch(Exception e)
            {
                Console.Write("{0} Exception", e);
            }
        }

        public TcpClient connekt(string IP, int PORT, string message)
        {
          
                TcpClient connection = new TcpClient(IP, PORT);     //Create new connection to IP and PORT
                //NetworkStream stream = connection.GetStream();      //Create a stream to pass the message
               // StreamWriter sw = new StreamWriter(stream);         //Create a steam writer to write to

                //sw.Write(message);      //Write the message to the writer
                //sw.Close();             //Close the stream writer since the message is writen so
                //stream.Close();         //Close the stream 
                //connection.Close();     //Close the connection
                return connection;

            
        }

        public string recTCP(string IP, int PORT, string message)
        {
            TcpClient connection = new TcpClient(IP, PORT);     //Create new connection to IP and PORT
            NetworkStream stream = connection.GetStream();      //Create a stream to pass the message
            //StreamReader sr = new StreamReader(stream);         //Create a steam writer to write to
            /*char[] buf;
            buf = new char[1024];
            //buf[1] = 'q';
            //while (sr.Read(buf, 0, 1024) <3)
            //{
            //}//Write the message to the writer
            sr.Read(buf, 0, 1024);
            string got = new string(buf);*/

            string Output = string.Empty;
            byte[] bReads = new byte[1024];                //new byte array to hold data
            int ReadAmount = 0;
            Thread.Sleep(200);                            //sleep to let data on network before checking
            while (stream.DataAvailable)                  //check for available data
            {
                ReadAmount = stream.Read(bReads, 0, bReads.Length);                               //read data into array
                Output += string.Format("{0}", Encoding.UTF8.GetString(bReads, 0, ReadAmount));   //append string
            }



            // sr.Close();             //Close the stream writer since the message is writen so
            stream.Close();         //Close the stream 
            connection.Close();     //Close the connection
            //return got;
            return Output;             //return UTF encoded string
        }

        public string xbee(string IP, int PORT, string message)
        {
            try
            {
                TcpClient connection = new TcpClient(IP, PORT);     //Create new connection to IP and PORT


                NetworkStream streamO = connection.GetStream();      //Create a stream to pass the message
                //NetworkStream streamI = connection.GetStream();      //Create a stream to pass the message
                StreamWriter sw = new StreamWriter(streamO);         //Create a steam writer to write to
                string Output = string.Empty;
                byte[] bReads = new byte[1024];                //new byte array to hold data
                int ReadAmount = 0;

                //sw.Write(message);

                Thread.Sleep(1000);                            //sleep to let data on network before checking

                while (streamO.DataAvailable)                  //check for available data
                {
                    ReadAmount = streamO.Read(bReads, 0, bReads.Length);                               //read data into array
                    Output += string.Format("{0}", Encoding.UTF8.GetString(bReads, 0, ReadAmount));   //append string
                }
                //sw.Write("Data Received");
                sw.Close();
                streamO.Close();
                //streamI.Close();
                connection.Close();
            return Output;
            }

            catch
            {
                //exception
            }

            return " ";
           
        }


        public void term(string IP, int PORT, string message, int limit)
        {
            IpTest = new DataMGT();
            TcpClient connection = new TcpClient(IP, PORT);     //Create new connection to IP and PORT
            NetworkStream stream = connection.GetStream();      //Create a stream to pass the message
            StreamWriter sw = new StreamWriter(stream);         //Create a steam writer to write to

            sw.Write(message);      //Write the message to the writer
            int i = 1;
                
            
                if (IpTest.newChar())
                {
                }
                else
                {
                    sw.Write(IpTest.getchar());
                    i++;
                }
            




            sw.Close();             //Close the stream writer since the message is writen so
            stream.Close();         //Close the stream 
            connection.Close();     //Close the connection
        }

            public static void lisn(string IP, int PORT)
            {
                TcpListener server = null;
                try
                {
                    // Set the TcpListener on port 13000.
                    //Int32 port = 13000;
                    System.Net.IPAddress localAddr = System.Net.IPAddress.Parse("192.168.1.9");//192.168.144.111

                    // TcpListener server = new TcpListener(port);
                    server = new TcpListener(localAddr,PORT);

                    // Start listening for client requests.
                    server.Start();

                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String data = null;

                    // Enter the listening loop. 
                    while (true)
                    {
                        Console.Write("Waiting for a connection... ");

                        // Perform a blocking call to accept requests. 
                        // You could also user server.AcceptSocket() here.
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client. 
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }

                        // Shutdown and end connection
                        client.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    // Stop listening for new clients.
                    server.Stop();
                }


                Console.WriteLine("\nHit enter to continue...");
                Console.Read();
            }
        
   
    
    
    }
}