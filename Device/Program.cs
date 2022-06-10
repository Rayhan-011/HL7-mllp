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
            StartClient();
            return 0;
        }

        public static void StartClient()
        {
            // byte[] bytes = new byte[1024];
            byte[] bytes = new byte[1048576];

            int filesent = 0;
            int ackRec = 0;
            string SourceFolderPath = System.Configuration.ConfigurationSettings.AppSettings["SourceFolderPath"];

          
           
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


                        //IPHostEntry host = Dns.GetHostEntry("localhost");
                       // IPHostEntry host2 = Dns.GetHostEntry(IP);

                        IPAddress ipAddress = IPAddress.Parse(IP);
                        IPEndPoint remoteEP = new IPEndPoint(ipAddress, multipleports);

                        // Create a TCP/IP  socket.
                        Socket sender = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                        // Connect to Remote EndPoint
                        sender.Connect(remoteEP);


                        Console.WriteLine("Socket connected to {0}",
                          sender.RemoteEndPoint.ToString());

                        foreach (var srcPath in Directory.GetFiles(SourceFolderPath))
                        {
                            string message = string.Empty;
                            string SourceFolderPathFinalWithFile = Path.Combine(SourceFolderPath, Path.GetFileName(srcPath));
                            StreamReader sr = new StreamReader(SourceFolderPathFinalWithFile);
                            string line = sr.ReadLine();
                            message += line;




                            while (line != null)
                            {

                                line = sr.ReadLine();
                                message += line;

                            }
                            sr.DiscardBufferedData();
                            sr.Dispose();
                            sr.Close();


                            //  }




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
                                }
                                else
                                {
                                    filesent = filesent;
                                }
                                // bytesSent = sender.Send(msg2);    

                                // Receive the response from the remote device.
                                int bytesRec = sender.Receive(bytes);
                                if (bytesRec != null)
                                {
                                    ackRec = ackRec + 1;
                                }
                                else { ackRec = ackRec; }
                                Console.WriteLine("Echoed test = {0}",
                                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                                //  Release the socket.
                                //  sender.Shutdown(SocketShutdown.Both);
                                //     sender.Close();

                            }
                            catch (ArgumentNullException ane)
                            {
                                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceSuccsessFolder"];

                                string ExceptionFolderNewFile2 = Path.Combine(ExceptionFolder, devicename + "_" + multipleports.ToString());

                                if (!Directory.Exists(ExceptionFolderNewFile2))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile2);


                                }
                                string ExceptionFolderNewFile = Path.Combine(ExceptionFolderNewFile2, FolderName);
                                if (!Directory.Exists(ExceptionFolderNewFile))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile);


                                }

                                bool fileExist2 = File.Exists(ExceptionFolderNewFile);

                                if (fileExist2)
                                {
                                    File.AppendAllText(ExceptionFolderNewFile, "Log Generated on: " + DateTime.Now + "\n ArgumentNullException : {0}" + ane.ToString() + Environment.NewLine);

                                }
                                else
                                {
                                    StreamWriter sw6 = new StreamWriter(ExceptionFolderNewFile);
                                    sw6.Write("Log Generated on: " + DateTime.Now + "\n ArgumentNullException : {0}" + ane.ToString() + Environment.NewLine);
                                    sw6.Flush();
                                    sw6.Close();
                                }

                                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                            }
                            catch (SocketException se)
                            {
                                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceSuccsessFolder"];

                                string ExceptionFolderNewFile2 = Path.Combine(ExceptionFolder, devicename + "_" + multipleports.ToString());

                                if (!Directory.Exists(ExceptionFolderNewFile2))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile2);


                                }
                                string ExceptionFolderNewFile = Path.Combine(ExceptionFolderNewFile2, FolderName);
                                if (!Directory.Exists(ExceptionFolderNewFile))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile);


                                }
                                bool fileExist2 = File.Exists(ExceptionFolderNewFile);

                                if (fileExist2)
                                {
                                    File.AppendAllText(ExceptionFolderNewFile, "Log Generated on: " + DateTime.Now + "\n SocketException : {0}" + se.ToString() + Environment.NewLine);

                                }
                                else
                                {
                                    StreamWriter sw6 = new StreamWriter(ExceptionFolderNewFile);
                                    sw6.Write("Log Generated on: " + DateTime.Now + "\n SocketException : {0}" + se.ToString() + Environment.NewLine);
                                    sw6.Flush();
                                    sw6.Close();
                                }



                                Console.WriteLine("SocketException : {0}", se.ToString());
                            }
                            catch (Exception e)
                            {
                                // string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceExceptionFolder"];
                                // string ExceptionFolderNewFile = Path.Combine(ExceptionFolder, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"));


                                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceSuccsessFolder"];

                                string ExceptionFolderNewFile2 = Path.Combine(ExceptionFolder, devicename + "_" + multipleports.ToString());

                                if (!Directory.Exists(ExceptionFolderNewFile2))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile2);


                                }
                                string ExceptionFolderNewFile = Path.Combine(ExceptionFolderNewFile2, FolderName);
                                if (!Directory.Exists(ExceptionFolderNewFile))
                                {
                                    Directory.CreateDirectory(ExceptionFolderNewFile);


                                }


                                StreamWriter sw4 = new StreamWriter(ExceptionFolderNewFile);
                                sw4.Write("Log Generated on: " + DateTime.Now + "\n Unexpected exception : {0}" + e.ToString());
                                sw4.Close();
                                Console.WriteLine(e.ToString());
                                Console.WriteLine(e.ToString());
                                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                            }
                        }


                        // Encode the data string into a byte array.

                        byte[] msg3 = Encoding.ASCII.GetBytes("<EOF>");

                        // Send the data through the socket.
                        int bytesSent2 = sender.Send(msg3);
                    }
                    catch (Exception e)
                    {
                        string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceExceptionFolder"];
                        string ExceptionFolderNewFile = Path.Combine(ExceptionFolder, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"));

                        bool fileExist2 = File.Exists(ExceptionFolderNewFile);

                        if (fileExist2)
                        {
                            File.AppendAllText(ExceptionFolderNewFile, "Log Generated on: " + DateTime.Now + "\n" + e.ToString() + Environment.NewLine);

                        }
                        else
                        {
                            StreamWriter sw6 = new StreamWriter(ExceptionFolderNewFile);
                            sw6.Write("Log Generated on: " + DateTime.Now + "\n" + e.ToString() + Environment.NewLine);
                            sw6.Flush();
                            sw6.Close();
                        }

                    }
                    string LogFilesSuccsess = System.Configuration.ConfigurationSettings.AppSettings["DeviceSuccsessFolder"];

                    string DestFolderPathSuccsess = Path.Combine(LogFilesSuccsess, devicename + "_" + multipleports.ToString());

                    if (!Directory.Exists(DestFolderPathSuccsess))
                    {
                        Directory.CreateDirectory(DestFolderPathSuccsess);


                    }
                    string LofFiles = Path.Combine(DestFolderPathSuccsess, FolderName);
                    if (!Directory.Exists(LofFiles))
                    {
                        Directory.CreateDirectory(LofFiles);


                    }


                    string LofFilesNewFile = Path.Combine(LofFiles, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"));

                    bool fileExist = File.Exists(LofFilesNewFile);


                    if (fileExist)
                    {
                        File.AppendAllText(LofFilesNewFile, "Log Generated on: " + DateTime.Now + "\n Total File Sent: " + filesent + "\n Total Acknowledgment Recived: " + ackRec + Environment.NewLine);

                    }
                    else
                    {
                        StreamWriter sw6 = new StreamWriter(LofFilesNewFile);
                        sw6.Write("Log Generated on: " + DateTime.Now + "\n Total File Sent: " + filesent + "\n Total Acknowledgment Recived: " + ackRec + Environment.NewLine);
                        sw6.Flush();
                        sw6.Close();
                    }

                    Console.WriteLine("\n Total File Sent: " + filesent);
                    Console.WriteLine("\n Total Acknowledgment Recived: " + ackRec);
                    filesent = 0;
                    ackRec = 0;

                }
            }
            else 
            {
                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["DeviceExceptionFolder"];
                //  string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];
                string Main = "IP Exceptions";
                string FolderName = DateTime.Now.ToString("yyyyMMdd");
                string DestFolderPathFinal3 = Path.Combine(ExceptionFolder, Main);

                if (!Directory.Exists(DestFolderPathFinal3))
                {
                    Directory.CreateDirectory(DestFolderPathFinal3);


                }
                string DestFolderPathFinal = Path.Combine(DestFolderPathFinal3, FolderName);
                if (!Directory.Exists(DestFolderPathFinal))
                {
                    Directory.CreateDirectory(DestFolderPathFinal);


                }

                string ExceptionFolderNewFile = Path.Combine(DestFolderPathFinal, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"));


                StreamWriter sw = new StreamWriter(ExceptionFolderNewFile);
                sw.Write("Log Generated on: " + DateTime.Now + "\n" + "IP address not provided");
                sw.Flush();
                sw.Close();
                Console.WriteLine("\n IP address not provided...");
            }
           

              Console.WriteLine("\n Press any key to Exit...");

          Console.ReadKey();
        }
}
}
