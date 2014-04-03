using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO.Compression;
using System.IO;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;

namespace ControllerLibrary
{
    public class ServiceAgent
    {
        private string _ip="";
        public ServiceAgent(string Ip)
        {
            _ip = Ip;
        }

        public string ConfigureHostFile(string hostFileLocation)
        {
            StreamReader reader =new StreamReader(hostFileLocation);
            string hostFileInfo = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            string sXml = "<RequestData xmlns=\"http://uri.org\"><details>" + hostFileInfo + "</details></RequestData>";
            string url = string.Format("http://{0}:8001/KryptonGridService/ConfigureHostFile", _ip); //System.Web.HttpUtility.UrlEncode(hostFileInfo));
            RestClient rest = new RestClient(url, HttpVerb.POST, sXml);
            string strRes = rest.MakeRequest("", true);
            return strRes;
        }

        public string InitializeSeleniumDriver(string browser, string browserInfo, string hostIp)
        {
            //string sXml = "<IntializeSeleniumGridDriveRequestData xmlns=\"http://uri.org\"><browser>" + browser + "</browser><browserInfo>" + browserInfo + "</browserInfo><hostIp>" + hostIp + "</hostIp></IntializeSeleniumGridDriveRequestData>";
            StringBuilder sXml = new StringBuilder();
            
            sXml.Append("<IntializeSeleniumGridDriveRequestData xmlns=\"http://uri.org\">");	
	        sXml.Append("<hostIp>hostip</hostIp>");
	        sXml.Append("<Browsers>");
	        sXml.Append("<Browser>");
	        sXml.Append("<browser>browser</browser>");
	        sXml.Append("<browserInfo>browserInfo</browserInfo>");
	        sXml.Append("</Browser>");
	        sXml.Append("<Browser>");
	        sXml.Append("	<browser>browser</browser>");
	        sXml.Append("	<browserInfo>browserInfo</browserInfo>");
	        sXml.Append("</Browser>");
	        sXml.Append("</Browsers>");
            sXml.Append("</IntializeSeleniumGridDriveRequestData>");



          


            string url = string.Format("http://{0}:8001/KryptonGridService/InitializeSeleniumDriver", _ip);
            RestClient rest = new RestClient(url, HttpVerb.POST, sXml.ToString());
           
            string returnStr = rest.MakeRequest("", true);

            bool status = CheckNodeRegistration(hostIp);
            Console.WriteLine(status);
            return status.ToString();
        }

        public string RestoreHostFile()
        {
            string url = string.Format("http://{0}:8001/KryptonGridService/RestoreHostFile", _ip);
            RestClient rest = new RestClient(url, HttpVerb.POST, "<resotre></resotre>");
            return rest.MakeRequest();
        }

        public string StopNode()
        {
            string url = string.Format("http://{0}:8001/KryptonGridService/StopNode", _ip);
            RestClient rest = new RestClient(url, HttpVerb.POST, "<StopNode></StopNode>");
            return rest.MakeRequest();
        }
        public string KillProcessByName(string processName)
        {
            string url = string.Format("http://{0}:8001/KryptonGridService/KillProcessByName", _ip);
            RestClient rest = new RestClient(url, HttpVerb.POST, "<ProcessInfo xmlns=\"http://uri.org\"><ProcessName>" + processName + "</ProcessName></ProcessInfo>");
            return rest.MakeRequest();
        }
        public bool CheckNodeRegistration(string hostIp)
        {
            string appURL = string.Format(@"http://{0}:4444/grid/console", hostIp);
            HttpWebRequest wrWebRequest = WebRequest.Create(appURL) as HttpWebRequest;
            HttpWebResponse hwrWebResponse = (HttpWebResponse)wrWebRequest.GetResponse();

            StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream());
            string strResponseData = srResponseReader.ReadToEnd();

            srResponseReader.Close();

            Regex expression = new Regex(string.Format("http://({0}:8002)", _ip), RegexOptions.None);
            MatchCollection m = expression.Matches(strResponseData);
            if (m.Count >= 1)
                return true;
            else
                return false;
        }

        public void StartExecutableByName(string path, string fileNameToBeUploaded)
        {
            string fileContent =Convert.ToBase64String(File.ReadAllBytes(fileNameToBeUploaded));
            string sXml = "<Executable xmlns=\"http://uri.org\"><fileNameToBeUploaded>" + Path.GetFileName(fileNameToBeUploaded) + "</fileNameToBeUploaded><fileToBeUploaded>" + fileContent + "</fileToBeUploaded><path>" + path + "</path></Executable>";
            //string sXml = "<Executable xmlns=\"http://uri.org\"><fileNameToBeUploaded>" + Path.GetFileName(fileNameToBeUploaded) + "</fileNameToBeUploaded><path>" + path + "</path><fileToBeUploaded></fileToBeUploaded></Executable>";
            string url = string.Format("http://{0}:8001/KryptonGridService/StartExecutableByName", _ip);
            RestClient rest = new RestClient(url, HttpVerb.POST, sXml);
            rest.MakeRequest("", true);
        }

        public string GetMachineName()
        {
            string url = string.Format("http://{0}:8001/KryptonGridService/GetMachineName", _ip);
            RestClient rest = new RestClient(url, HttpVerb.GET);
            return rest.MakeRequest();
        }
    }
}