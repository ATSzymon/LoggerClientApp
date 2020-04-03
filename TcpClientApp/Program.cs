using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TcpClientApp
{
    class Program
    {


        static void Main(string[] args)
        {
            //SHA1 sHA = SHA1.Create();
            //MD5 md5algo = MD5.Create();
            //string dupa = "chuuuj ci w dupe";
            //byte[] s = Encoding.UTF8.GetBytes(dupa);
            //byte [] converted = md5algo.ComputeHash(s);
            //byte[] sh = sHA.ComputeHash(s);

            //var lens = s.Length;
            //var lenc = converted.Length;
            //var shaa = sh.Length;

            //HashSet<string> vs = new HashSet<string>();
            //vs.Add(dupa);
            

            //Debugger.Break();

            Console.WriteLine("Hello World!");

            TCPClient client = new TCPClient();

            //client.Connect("127.0.0.1", 10001);

            client.StartClient("127.0.0.1", 10005);

            Console.ReadKey();
        }

        //static void Connect(String server, String message)
        //{
        //    try
        //    {

        //        Int32 port = 10001;
        //        TcpClient client = new TcpClient(server, port);

        //        // Translate the passed message into ASCII and store it as a Byte array.
        //        //Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

        //        // Get a client stream for reading and writing.
        //        //  Stream stream = client.GetStream();

        //        RecieveData(client);
                

                
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        Console.WriteLine("ArgumentNullException: {0}", e);
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("SocketException: {0}", e);
        //    }

        //    Console.WriteLine("\n Press Enter to continue...");
        //    Console.Read();
        //}

        //private static void RecieveData(TcpClient client)
        //{
        //    NetworkStream stream = client.GetStream();

        //    //// Send the message to the connected TcpServer. 
        //    //stream.Write(data, 0, data.Length);
        //    //Console.WriteLine("Sent: {0}", message);
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    // Buffer to store the response bytes.
        //    //Byte[] responseBuffer = new Byte[256];

        //    // String to store the response ASCII representation.
        //    String responseData = String.Empty;
        //    int bytesToRead;
        //    while (true)
        //    {
        //        //stream.Write(data, 0, data.Length);
        //        Byte[] responseBuffer = new Byte[client.ReceiveBufferSize];
        //        //Console.WriteLine("Sent: {0}", message);

        //        //// Read the first batch of the TcpServer response bytes.
        //        //Int32 bytes = stream.Read(responseBuffer, 0, responseBuffer.Length);

        //        ATDiagnosticsLogger.Log obj = new ATDiagnosticsLogger.Log();

        //        while (stream.DataAvailable && stream.CanRead) //bytesToRead = stream.Read(responseBuffer, 0, responseBuffer.Length)) != 0
        //        {
        //            try
        //            {

        //                obj = (ATDiagnosticsLogger.Log)binaryFormatter.Deserialize(stream);

        //                Console.WriteLine($"Received: {obj.ClassName}, {obj.LineNumber}, {obj.MethodName}, {obj.Message}");
        //            }
        //            catch (Exception ex)
        //            {
        //                //some shit
        //            }

        //        }
        //        //responseData = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytes);

        //        Thread.Sleep(20);
        //    }

            
        //}
    }
}
