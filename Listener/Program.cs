using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Listener
{
    class Program
    {
        public static int Main(String[] args)
        {
            try 
            {
                StartServer();
            }
            catch (Exception ex) 
            {
                string FolderName = DateTime.Now.ToString("yyyyMMdd");
                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["ListenerExceptionFolder"];
                //  string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];

                string DestFolderPathFinal3 = Path.Combine(ExceptionFolder);

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
                sw.Write("Log Generated on: " + DateTime.Now + "\n" + ex.ToString());
                sw.Flush();
                sw.Close();

                Console.WriteLine("Exception: " +ex.ToString());
            }
            
            return 0;
        }

        public static void StartServer()
        {
            Console.WriteLine("Listener is Ready");

            string[] repositoryUrls = ConfigurationManager.AppSettings.AllKeys
                             .Where(key => key.StartsWith("DeviceIP"))
                             .Select(key => ConfigurationManager.AppSettings[key])
                             .ToArray();

            if (repositoryUrls.Length != 0)
            {
                string ipv4 = string.Empty;
                foreach (var appsettings in repositoryUrls)
                {

                    string[] DeviceIpPort = appsettings.Split(':');
                    string devicename = DeviceIpPort[0];
                    
                    int multipleports = int.Parse(DeviceIpPort[1]);


                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    try
                    {
                        foreach (var ip in host.AddressList)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipv4 = ip.ToString();
                                if (ipv4.StartsWith("192")) 
                                {
                                    Console.WriteLine("\r IPV4 Address: " + ipv4 + "\r \n Port: " + multipleports);
                                }
                                
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];

                        string DestFolderPathFinal3 = Path.Combine(DestFolderPathFinal2, devicename + "_" + multipleports.ToString());
                        string FolderName2 = DateTime.Now.ToString("yyyyMMdd");
                        if (!Directory.Exists(DestFolderPathFinal3))
                        {
                            Directory.CreateDirectory(DestFolderPathFinal3);


                        }
                        string DestFolderPathFinal = Path.Combine(DestFolderPathFinal3, FolderName2);
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


                    IPAddress ipAddress = IPAddress.Parse(ipv4); ;
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, multipleports);
                    string FolderName = DateTime.Now.ToString("yyyyMMdd");
                    try
                    {

                        // Create a Socket that will use Tcp protocol
                        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        // A Socket must be associated with an endpoint using the Bind method
                        listener.Bind(localEndPoint);

                        // Specify how many requests a Socket can listen before it gives Server busy response.
                        // We will listen 10 requests at a time
                        listener.Listen(15);

                        Console.WriteLine("Waiting for a connection...");
                        Socket handler;
                        handler = listener.Accept();

                        // Incoming data from the client.
                        string data = null;
                        byte[] bytes = null;


                        string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];

                        string DestFolderPathFinal3 = Path.Combine(DestFolderPathFinal2, devicename + "_" + multipleports.ToString());

                        if (!Directory.Exists(DestFolderPathFinal3))
                        {
                            Directory.CreateDirectory(DestFolderPathFinal3);


                        }
                        string DestFolderPathFinal = Path.Combine(DestFolderPathFinal3, FolderName);
                        if (!Directory.Exists(DestFolderPathFinal))
                        {
                            Directory.CreateDirectory(DestFolderPathFinal);


                        }

                        int count = 0;
                        int AckNo = 0;
                        while (true)
                        {



                            bytes = null;
                            bytes = new byte[1048576];
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data != "")
                            {
                                count = count + 1;

                            }
                            else
                            {
                                count = count;
                            }

                            List<string> Elements = new List<string>();
                            // Elements.Add("MSH|");
                            Elements.Add("PID|");
                            Elements.Add("NTE|");
                            Elements.Add("PV1|");
                            Elements.Add("OBR|");
                            Elements.Add("OCR|");
                            Elements.Add("SPM|");
                            Elements.Add("MSA|");
                            Elements.Add("SAC|");
                            Elements.Add("ERR|");
                            Elements.Add("OBX|");
                            Elements.Add("ZER|");
                            Elements.Add("QPD|");
                            Elements.Add("ORC|");
                            Elements.Add("RCP|");
                            Elements.Add("QAK|");
                            Elements.Add("QID|");
                            Elements.Add("EQU|");
                            Elements.Add("INV|");
                            Elements.Add("NDS|");

                            List<int> foundIndexes = new List<int>();

                            foreach (var ind in Elements)
                            {
                                if (data.Contains(ind))
                                {

                                    for (int i = data.IndexOf(ind); i > -1; i = data.IndexOf(ind, i + 1))
                                    {
                                        // for loop end when i=-1 ('a' not found)
                                        foundIndexes.Add(i);



                                    }

                                    if (foundIndexes.Count > 1)
                                    {
                                        string str1 = "/x0D";
                                        data = data.Insert(data.IndexOf(ind), str1);

                                        for (int i = 1; i < foundIndexes.Count; i++)

                                        {
                                            int strlength = str1.Length;
                                            int finalpos = foundIndexes[i] + (strlength * i);
                                            data = data.Insert(finalpos, str1);

                                        }
                                    }

                                    else
                                    {
                                        foreach (var listindex in foundIndexes)
                                        {

                                            string str1 = "/x0D";
                                            data = data.Insert(data.IndexOf(ind), str1);


                                        }

                                    }

                                    foundIndexes.Clear();
                                }

                                //  data += @"<EOF>";
                            }

                            data = data.Replace("/x0D", Environment.NewLine);




                            if (!data.Contains("<EOF>"))
                            {

                                bool res;
                                char c = data[0];
                                int index = data.IndexOf(c);
                                res = char.IsLetterOrDigit(data[index]);

                                if (!res)
                                {
                                    if (data.Length > 0)
                                    {
                                        data = data.Remove(0, 1);
                                    }

                                }


                                int index2 = data.Length - 1;
                                char c2 = data[index2];
                                res = char.IsLetterOrDigit(c2);
                                if (!res)
                                {
                                    data = data.Remove(index2);
                                }


                                DateTime dt = DateTime.Now;
                                // Random rndnum = new Random();
                                //  int num = rndnum.Next(0, 50);
                                int ms = dt.Millisecond;
                                string filename = dt.ToString();
                                string DestFolderPathFinalWithNewFile = Path.Combine(DestFolderPathFinal, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + ms + ".hl7"));
                                string DestFolderPathFinalWithNewFileTxt = Path.Combine(DestFolderPathFinal, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + ms + ".txt"));


                                StreamWriter sw2 = new StreamWriter(DestFolderPathFinalWithNewFile);
                                StreamWriter swt = new StreamWriter(DestFolderPathFinalWithNewFileTxt);
                                string finaldata = data;
                                //     finaldata = finaldata.Replace("<EOF>", Environment.NewLine);
                                sw2.WriteLine(finaldata);
                                swt.WriteLine(finaldata);

                                swt.Flush();
                                sw2.Flush();

                                sw2.Dispose();
                                swt.Dispose();

                                sw2.Close();
                                swt.Close();

                                Console.WriteLine("Text received : {0}", finaldata);
                                string LogFiles = System.Configuration.ConfigurationSettings.AppSettings["ListenerSuccsessFolder"];

                                string DestFolderPathFinal33 = Path.Combine(LogFiles, devicename + "_" + multipleports.ToString());

                                if (!Directory.Exists(DestFolderPathFinal33))
                                {
                                    Directory.CreateDirectory(DestFolderPathFinal33);


                                }
                                string DestFolderPathFinal26 = Path.Combine(DestFolderPathFinal33, FolderName);
                                if (!Directory.Exists(DestFolderPathFinal26))
                                {
                                    Directory.CreateDirectory(DestFolderPathFinal26);


                                }

                                string LogFilesNewFile = Path.Combine(DestFolderPathFinal26, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd") + ".txt"));
                                bool fileExist = File.Exists(LogFilesNewFile);

                                if (fileExist)
                                {
                                    File.AppendAllText(LogFilesNewFile, "Log Generated on: " + DateTime.Now + "\n File:" + count + " Recived" + Environment.NewLine);

                                }
                                else
                                {
                                    StreamWriter sw = new StreamWriter(LogFilesNewFile);
                                    sw.Write("Log Generated on: " + DateTime.Now + "\n File:" + count + " Recived" + Environment.NewLine);
                                    sw.Flush();
                                    sw.Close();
                                }

                                byte[] msg = Encoding.ASCII.GetBytes(finaldata);
                                if (msg != null)
                                {
                                    AckNo = AckNo + 1;
                                }
                                else
                                {
                                    AckNo = AckNo;
                                }

                                handler.Send(msg);

                                LogFilesNewFile = Path.Combine(DestFolderPathFinal26, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd") + ".txt"));
                                fileExist = File.Exists(LogFilesNewFile);

                                if (fileExist)
                                {
                                    File.AppendAllText(LogFilesNewFile, "Log Generated on: " + DateTime.Now + "\n Acknowledgment: " + count + " Sent" + Environment.NewLine);
                                    //Console.WriteLine("File exists.");
                                }
                                else
                                {
                                    StreamWriter sw = new StreamWriter(LogFilesNewFile);
                                    sw.Write("Log Generated on: " + DateTime.Now + "\n Acknowledgment: " + count + " Sent" + Environment.NewLine);
                                    sw.Flush();
                                    sw.Close();
                                }

                                data = string.Empty;
                                finaldata = string.Empty;

                                //  Console.WriteLine("File" + count + "Recived");

                            }

                            if (data.IndexOf("<EOF>") > -1)
                            {
                                int finalcount = count - 1;

                                Console.WriteLine("\n Total Files Recived: " + finalcount);
                                Console.WriteLine("\n Total Acknowledgment Sent: " + AckNo);


                                string LofFiles = System.Configuration.ConfigurationSettings.AppSettings["ListenerSuccsessFolder"];

                                //string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["ListenerExceptionFolder"];
                                //  string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];

                                string DestFolderPathFinal33 = Path.Combine(LofFiles, devicename + "_" + multipleports.ToString());

                                if (!Directory.Exists(DestFolderPathFinal33))
                                {
                                    Directory.CreateDirectory(DestFolderPathFinal33);


                                }
                                string DestFolderPathFinal26 = Path.Combine(DestFolderPathFinal33, FolderName);
                                if (!Directory.Exists(DestFolderPathFinal26))
                                {
                                    Directory.CreateDirectory(DestFolderPathFinal26);


                                }

                                string LofFilesNewFile = Path.Combine(DestFolderPathFinal26, Path.GetFileName(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"));


                                StreamWriter sw = new StreamWriter(LofFilesNewFile);
                                sw.Write("Log Generated on: " + DateTime.Now + "\n Total Files Recived: " + finalcount + "\n Total Acknowledgment Sent: " + AckNo);
                                sw.Flush();
                                sw.Close();
                                break;

                                finalcount = 0;
                                AckNo = 0;
                            }

                        }





                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();

                    }
                    catch (Exception e)
                    {
                        string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["ListenerExceptionFolder"];
                        //  string DestFolderPathFinal2 = System.Configuration.ConfigurationSettings.AppSettings["DestinationFolder"];

                        string DestFolderPathFinal3 = Path.Combine(ExceptionFolder, devicename + "_" + multipleports.ToString());

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
                        sw.Write("Log Generated on: " + DateTime.Now + "\n" + e.ToString());
                        sw.Flush();
                        sw.Close();
                        Console.WriteLine(e.ToString());

                    }
                }

            }
            else
            {
                string ExceptionFolder = System.Configuration.ConfigurationSettings.AppSettings["ListenerExceptionFolder"];
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




            Console.WriteLine("\n Press any key to continue...");

            Console.ReadKey();
        }
    }
}
