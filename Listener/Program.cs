using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace Listener
{
    class Program
    {
        public static int Main(String[] args)
        {
            try 
            {
                bool conn = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

                if (conn) 
                {
                    StartServer();
                }
                else
                {
                    LogData("ListenerExceptionFolderMain", "Network Exceptions", DateTime.Now.ToString("yyyyMMdd_hhmmss"), "No Internet Connection Available");

                }
            }
            catch (Exception ex) 
            {
                LogData("ListenerExceptionFolderMain", "Code Exceptions", DateTime.Now.ToString("yyyyMMdd_hhmmss"), ex.ToString());

            }

            Console.ReadKey();
            return 0;
        }

        public static void StartServer()
        {
            Console.WriteLine("Listener Start Successfully .... \n");

            List<string> IPlist = new List<string>();
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
                    string IP = DeviceIpPort[1];
                    int multipleports = int.Parse(DeviceIpPort[2]);


                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    Console.WriteLine("Listener matching LAN IP : " + IP +"\n");

                    try

                    {                   
                        NetworkInterfaceType _type = NetworkInterfaceType.Ethernet;
                        Console.WriteLine("Listener Getting machine's LAN IPs .... \n");
                        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                        {

                                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                                {

                                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                    {

                                        if (ip.Address.ToString().StartsWith("192"))
                                        {
                                            IPlist.Add(ip.Address.ToString());
                                            
                                        }
                                        else 
                                        {
                                            ipv4 = string.Empty;
                                        }
                                       
                                    
                                }
                            }
                        }
                      if (IPlist.Count==0)
                        {

                            _type = NetworkInterfaceType.Wireless80211;
                            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                        {

                           
                                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                                {
                                        
                                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                    {

                                           
                                            if (ip.Address.ToString().StartsWith("192"))                                        
                                            {
                                                IPlist.Add(ip.Address.ToString());

                                            }
                                            
                                            else
                                            {
                                                ipv4 = ip.Address.ToString();

                                            }
                                    
                                }
                            }
                        }
                        }
                        Console.WriteLine("Your Machine Contains following IP Addresses...\n");

                        foreach (var ipfinal in IPlist)
                        {
                            Console.WriteLine(ipfinal + "\n");

                        }

                        if (IPlist.Contains(IP))
                        {
                            Console.WriteLine("\n IPV4 Address " + IP + " is matched Succsessfully with Machine's IP...\n");
                            ipv4 = IP;
                            // Console.WriteLine("\r IPV4 Address: " + ipv4 + "\r \n Port: " + multipleports);
                            
                        }

                        else
                        {
                            ipv4 = string.Empty;
                            Console.WriteLine(IP + " not matched with your Machine's IP...\n");
                        }

                    }

                    catch (Exception e)
                    {
                        LogData("ListenerExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss"), e.ToString());
                    }

                    if (ipv4.StartsWith("192"))
                    {
                        IPAddress ipAddress = IPAddress.Parse(ipv4);
                        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, multipleports);
                        string FolderName = DateTime.Now.ToString("yyyyMMdd");
                        try
                        {

                            // Create a Socket that will use Tcp protocol
                            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                            Console.WriteLine("Listener Connected with");
                            Console.WriteLine("\r IPV4 Address: " + ipv4 + "\r \n Port: " + multipleports+"\n");
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

                                    int ms = dt.Millisecond;
                                    string filename = dt.ToString();
                                    string finaldata = data;

                                    WriteFile("DestinationFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + ms + ".hl7", finaldata);
                                    WriteFile("DestinationFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + ms + ".txt", finaldata);


                                    LogData("ListenerSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "File:" + count + " Received" + Environment.NewLine);
                                    Console.WriteLine("Received Data : {0}", finaldata);



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

                                    LogData("ListenerSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd"), "Acknowledgment: " + count + " Sent" + Environment.NewLine);

                                    data = string.Empty;
                                    finaldata = string.Empty;

                                }

                                if (data.IndexOf("<EOF>") > -1)
                                {
                                    int finalcount = count - 1;
                                    LogData("ListenerSuccsessFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss"), "Total Files Received: " + finalcount + "\n \n Total Acknowledgment Sent: " + AckNo);
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
                            LogData("ListenerExceptionFolder", devicename + "_" + multipleports.ToString(), DateTime.Now.ToString("yyyyMMdd_hhmmss"), e.ToString());

                        }
                    }
                    else
                    {
                        Console.WriteLine("IP Address Not Matched with Machine's IP Addresses, Provide Correct IP Address... ");
                    }
                   
                }

            }
            else
            {
                LogData("ListenerExceptionFolderMain", "IP Exceptions", DateTime.Now.ToString("yyyyMMdd_hhmmss"), "IP address not provided");
                
            }

            Console.WriteLine("\n Press any key to continue...");

        }
    
    public static void LogData(string folderPath ,string joinfolder,string filename, string message) 
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
  
        
        public static void WriteFile(string folderPath ,string joinfolder,string filename, string message) 
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

            string FolderWithFile = Path.Combine(DestFolderPathFinal, Path.GetFileName(filename));

            bool fileExist = File.Exists(FolderWithFile);

            if (fileExist)
            {
                File.AppendAllText(FolderWithFile,message);

            }
            else
            {
                StreamWriter sw = new StreamWriter(FolderWithFile);
                sw.Write(message);
                sw.Flush();
                sw.Close();      
                sw.Dispose();   
            }
        }
    }
}
