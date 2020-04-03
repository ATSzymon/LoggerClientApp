using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using ATDiagnosticsLogger;

namespace TcpClientApp
{

    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class TCPClient
    {

        public TCPClient()
        {
            frame.Frame_OK += Frame_Frame_OK;

            logsQueue = new ConcurrentQueue<string>();
        }

        private void Frame_Frame_OK(object o, Frame_Comm.FOKEventArg e)
        {
            MessageDTO msgDTO = new MessageDTO();
            byte[] recObj = frame.Get_Input_Frame();

            int raw_size;

            raw_size = Marshal.SizeOf(msgDTO);

            IntPtr ptr = Marshal.AllocHGlobal(raw_size);

            Marshal.Copy(recObj, 0, ptr, raw_size);

            msgDTO = (MessageDTO)Marshal.PtrToStructure(ptr, msgDTO.GetType());

            Log log = new Log();

            log.Message = Encoding.UTF8.GetString(msgDTO.Message).Replace("\0", "");
            log.Level = BitConverter.ToInt32(msgDTO.Level, 0);
            log.MethodName = Encoding.ASCII.GetString(msgDTO.MethodName).Replace("\0", "");
            log.ClassName = Encoding.ASCII.GetString(msgDTO.ClassName).Replace("\0", "");
            log.LineNumber = BitConverter.ToUInt32(msgDTO.LineNumber, 0);
            log.timeStamp = DateTime.FromBinary(BitConverter.ToInt64(msgDTO.timeStamp, 0));


            switch (log.Level)
            {
                case (int)Levels.Debug:
                    SimonJConsole.WriteLineDebug($"Debug: {log.timeStamp}, {log.ClassName}/{log.MethodName}, Line: {log.LineNumber}, M: {log.Message}");
                    break;
                case (int)Levels.Info:
                    SimonJConsole.WriteLineInfo($"Info: {log.timeStamp}, {log.ClassName}/{log.MethodName}, Line: {log.LineNumber}, M: {log.Message}");
                    break;
                case (int)Levels.Warning:
                    SimonJConsole.WriteLineWarn($"Warn: {log.timeStamp}, {log.ClassName}/{log.MethodName}, Line: {log.LineNumber}, M: {log.Message}");
                    break;
                case (int)Levels.Error:
                    SimonJConsole.WriteLineErr($"Error: {log.timeStamp}, {log.ClassName}/{log.MethodName}, Line: {log.LineNumber}, M: {log.Message}");
                    break;
            }



            Marshal.FreeHGlobal(ptr);
        }

        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        private static String response = String.Empty;

        private Frame_Comm frame = new Frame_Comm();
        private ConcurrentQueue<string> logsQueue;

        //MessageDTO recMsg = new MessageDTO();
        //byte[] response1 = new byte[sizeof(recMsg)];

        public void Connect(String server, int port)
        {
            try
            {

                TcpClient client = new TcpClient(server, port);
                RecieveData(client);



            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);

            }


            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        private static void RecieveData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            //// Send the message to the connected TcpServer. 
            //stream.Write(data, 0, data.Length);
            //Console.WriteLine("Sent: {0}", message);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            // Buffer to store the response bytes.
            //Byte[] responseBuffer = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;
            int bytesToRead;
            while (true)
            {
                //stream.Write(data, 0, data.Length);
                Byte[] responseBuffer = new Byte[client.ReceiveBufferSize];
                //Console.WriteLine("Sent: {0}", message);

                //// Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(responseBuffer, 0, responseBuffer.Length);

                ATDiagnosticsLogger.Log obj = new ATDiagnosticsLogger.Log();

                while (stream.DataAvailable && stream.CanRead) //bytesToRead = stream.Read(responseBuffer, 0, responseBuffer.Length)) != 0
                {
                    try
                    {

                        obj = (ATDiagnosticsLogger.Log)binaryFormatter.Deserialize(stream);

                        switch (obj.Level)
                        {
                            case Levels.Debug:
                                SimonJConsole.WriteLineDebug($"Debug: {obj.timeStamp}, {obj.ClassName}/{obj.MethodName}, Line: {obj.LineNumber}, M: {obj.Message}");
                                break;
                            case Levels.Info:
                                SimonJConsole.WriteLineInfo($"Info: {obj.timeStamp}, {obj.ClassName}/{obj.MethodName}, Line: {obj.LineNumber}, M: {obj.Message}");
                                break;
                            case Levels.Warning:
                                SimonJConsole.WriteLineWarn($"Warn: {obj.timeStamp}, {obj.ClassName}/{obj.MethodName}, Line: {obj.LineNumber}, M: {obj.Message}");
                                break;
                            case Levels.Error:
                                SimonJConsole.WriteLineErr($"Error: {obj.timeStamp}, {obj.ClassName}/{obj.MethodName}, Line: {obj.LineNumber}, M: {obj.Message}");
                                break;
                        }


                        //Console.WriteLine($"Received: {obj.ClassName}, {obj.LineNumber}, {obj.MethodName}, {obj.Message}");
                    }
                    catch (Exception ex)
                    {
                        //some shit
                    }

                }
                //responseData = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytes);

                Thread.Sleep(100);
            }


        }


        public void StartClient(string ip, int port)
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Parse(ip), port);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.BeginConnect(remoteIP, new AsyncCallback(ConnectionCallback), client);
                connectDone.WaitOne();

                while (true)
                {
                    SendToServer(client, null);
                    sendDone.WaitOne();

                    Thread.Sleep(1);

                    if (client.Available != 0)
                    {
                        Receive(client);
                        receiveDone.WaitOne();
                    }

                    Thread.Sleep(1);

                }

            }
            catch
            {

            }
        }

        private void Receive(Socket client)
        {
            try
            {


                StateObject clientState = new StateObject();
                clientState.workSocket = client;

                client.BeginReceive(clientState.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), clientState);
            }
            catch
            {
                //Unable to read!
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {


                StateObject clientState = (StateObject)ar.AsyncState;
                Socket client = clientState.workSocket;

                int bytesToRead = client.EndReceive(ar);

                if (client.Available > 0)
                {
                    //! Deserialize data!
                    //clientState.sb.Append(Encoding.ASCII.GetString(clientState.buffer, 0, bytesToRead));
                    frame.Decode_data(clientState.buffer, bytesToRead);
                    bytesToRead = 0;

                    logsQueue.Enqueue("chuj Ci w dupe");

                    // Get the rest of the data.  
                    client.BeginReceive(clientState.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), clientState);
                }
                else
                {
                    //if (clientState.buffer.Length > 0)
                    //{
                    //    //frame.Decode_data(clientState.buffer, bytesToRead);

                    //}

                    receiveDone.Set();
                }
            }
            catch
            {
                //? nie udało się odczytać danych!!
            }
        }

        private void SendToServer(Socket clientHandler, object? p)
        {
            //create data from server 
            //if (p != null)
            //{
                if (logsQueue.Count != 0)
                {

                    if (logsQueue.Count > 0) //send till the last msg
                    {


                        logsQueue.TryDequeue(out string msg);
                        if (msg != null)
                        {
                            int len = 0;


                            byte[] bytesToSend = Encoding.ASCII.GetBytes(msg);
                            try
                            {

                                clientHandler.BeginSend(bytesToSend, 0, bytesToSend.Length, 0, new AsyncCallback(SendCallback), clientHandler);
                            }
                            catch
                            {
                                clientHandler.Shutdown(SocketShutdown.Both);
                                clientHandler.Close();
                            }


                        }

                    }
                //}
                //sendDone.Set();
            }
            sendDone.Set();
        }
        private void ConnectionCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;

            //establish connection
            client.EndConnect(ar);

            connectDone.Set();
            Console.WriteLine("Poczyem się jak sam skurwysyn");
        }


        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket clientHandler = (Socket)ar.AsyncState;

                //dokończ wysyłanie do clienta
                int bytesSent = clientHandler.EndSend(ar);
                sendDone.Set();

            }
            catch
            {
                Debug.WriteLine(" Error in sending message to Server");
            }
        }
    }
}
