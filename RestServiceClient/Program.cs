using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControllerLibrary;
using System.IO;
using System.Xml;

namespace RestServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
             ServiceAgent service = new ServiceAgent("10.101.21.235");
            //string str = service.ConfigureHostFile(@"C:\Windows\System32\drivers\etc\hosts");

            string str = service.InitializeSeleniumDriver("Chrome", "maxInstance=1", "10.101.22.74");
             //string str1 = service.StopNode();
             //string str = service.RestoreHostFile();
             //service.KillProcessByName("cmd");
            //service.StartExecutableByName("command.bat", @"C:\Users\jha.madhukar\Desktop\TwitterErrorImage1.jpg");
             //string str = service.GetMachineName();
             //XmlDocument xmlDoc = new XmlDocument();
             //xmlDoc.LoadXml(str);
             //Console.WriteLine(xmlDoc.InnerText);
             Console.WriteLine("Press any key to stop execution...");
            Console.ReadLine();
        }
        public static void CallBackFunction(string completionFlag, string returnAddress)
        {

        }
    }
}