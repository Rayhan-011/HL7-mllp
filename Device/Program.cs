using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Linq;

namespace Client
{
    class Program
    {
        public static int Main(String[] args)
        {
            try 
            {
                StartClient();
            }
            catch (Exception ex)
            {
                LogData("DeviceExceptionFolderMain", "Code Exception", DateTime.Now.ToString("yyyyMMdd_hhmmss"), ex.ToString());
    
            }

            Console.ReadKey();
            return 0;
        }

        public static void StartClient()
        {
           
            byte[] bytes = new byte[1048576];

            int filesent = 0;
            int ackRec = 0;
           
          
           
            string[] repositoryUrls = ConfigurationManager.AppSettings.AllKeys
                           .Where(key => key.StartsWith("DeviceIP"))
                           .Select(key => ConfigurationManager.AppSettings[key])
                           .ToArray();
            if (repositoryUrls.Length!=0)
            {
                foreach (var appsettings in repositoryUrls)
                {

                    string[] DeviceIpPort = appsettings.Split(':');
                    string devicename = DeviceIpPort[0];
                    string IP = DeviceIpPort[1];
                    int multipleports = int.Parse(DeviceIpPort[2]);
                    string FolderName = DateTime.Now.ToString("yyyyMMdd");
                    try
                    {

                        // Connect to a Remote server
                        // Get Host IP Address that is used to establish a connection
                        // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                        // If a host has multiple addresses, you will get a list of addresses

                        IPAddress ipAddress = IPAddress.Parse(IP);
                        IPEndPoint remoteEP = new IPEndPoint(ipAddress, multipleports);

                        // Create a TCP/IP  socket.
                        Socket sender = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                        // Connect to Remote EndPoint
                        sender.Connect(remoteEP);


                        Console.WriteLine("Socket connected to {0}",
                          sender.RemoteEndPoint.ToString());
                        string SourceFolderPath = System.Configuration.ConfigurationSettings.AppSettings["SourceFolderPath"];

                        foreach (var srcPath in Directory.GetFiles(SourceFolderPath))
                        {
                            string message = string.Empty;             
                            string SourceFolderPathFinalWithFile = Path.Combine(SourceFolderPath, Path.GetFileName(srcPath));
                            StreamReader sr = new StreamReader(SourceFolderPathFinalWithFile);
                            string line = sr.ReadLine();
                            message += line;

                            //Read Data from file
                            while (line != null)
                            {

                                line = sr.ReadLine();
                                message += line;

                            }
                            sr.DiscardBufferedData();
                            sr.Dispose();
                            sr.Close();


                            // Connect the socket to the remote endpoint. Catch any errors.
                            try
                            {

                                // Encode the data string into a byte array.

                                byte[] msg2 = Encoding.ASCII.GetBytes(message);

                                // Send the data through the socket.
                                int bytesSent = sender.Send(msg2);
                                if (bytesSent != null)
                                {
                                    filesent = filesent + 1;
                                    LogData("DeviceSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "File: " + filesent + " Sent" + Environment.NewLine);

                                }
                                else
                                {
                                    filesent = filesent;
                                }
                             
                                // Receive the response from the remote device.
                                int bytesRec = sender.Receive(bytes);
                                if (bytesRec != null)
                                {
                                    ackRec = ackRec + 1;
                                    LogData("DeviceSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "Acknowledgment: " + ackRec + " Received" + Environment.NewLine);

                                }
                                else { ackRec = ackRec; }
                                Console.WriteLine("Echoed test = {0}",
                                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                             

                            }
                            catch (ArgumentNullException ane)
                            {
                                LogData("DeviceExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "ArgumentNullException: { 0} " + ane.ToString() + Environment.NewLine);



                                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                            }
                            catch (SocketException se)
                            {
                                LogData("DeviceExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "SocketException : {0}" + se.ToString() + Environment.NewLine);
                                
                            }
                            catch (Exception e)
                            {
                                LogData("DeviceExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "Unexpected exception : {0}" + e.ToString());

                            }
                        }
                        //yyyyMMdd_hhmmss

                        LogData("DeviceSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss"), "Total File Sent: " + filesent + "\n Total Acknowledgment Recived: " + ackRec + Environment.NewLine);

                        // Encode the data string into a byte array.

                        byte[] msg3 = Encoding.ASCII.GetBytes("<EOF>");
                        
                        // Send the data through the socket.
                        int bytesSent2 = sender.Send(msg3);
                       
                    }
                    catch (Exception e)
                    {
                        LogData("DeviceExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "Unexpected exception : {0}" + e.ToString());

                    }

                   

                    filesent = 0;
                    ackRec = 0;

                }
            }
            else 
            {
                LogData("DeviceExceptionFolderMain", "IP Exceptions", DateTime.Now.ToString("yyyyMMdd"), "Unexpected exception : {0}" + "IP address not provided");

            }
           

              Console.WriteLine("\n Press any key to Exit...");

          Console.ReadKey();
        }


        public static void LogData(string folderPath, string joinfolder, string filename, string message)
        {

            string Folder = System.Configuration.ConfigurationSettings.AppSettings[folderPath];
            string FolderNameDate = DateTime.Now.ToString("yyyyMMdd");
            string DestFolderPath = Path.Combine(Folder, joinfolder);

            if (!Directory.Exists(DestFolderPath))
            {
                Directory.CreateDirectory(DestFolderPath);


            }
            string DestFolderPathFinal = Path.Combine(DestFolderPath, FolderNameDate);
            if (!Directory.Exists(DestFolderPathFinal))
            {
                Directory.CreateDirectory(DestFolderPathFinal);


            }

            string FolderWithFile = Path.Combine(DestFolderPathFinal, Path.GetFileName(filename + ".txt"));

            bool fileExist = File.Exists(FolderWithFile);

            if (fileExist)
            {
                File.AppendAllText(FolderWithFile, "Log Generated on: " + DateTime.Now + "\n" + message);

            }
            else
            {
                StreamWriter sw = new StreamWriter(FolderWithFile);
                sw.Write("Log Generated on: " + DateTime.Now + "\n" + message);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            Console.WriteLine("\n" + message);

        }

    }
}
