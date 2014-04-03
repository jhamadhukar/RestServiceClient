using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using System.Configuration;

using Microsoft.Win32;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Runtime.Serialization.Formatters.Binary;

namespace KryptonGridService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFServices : IWCFServices
    {
        string exitMessage = "";
        //int iCount = 0;
        //bool exit = false;

        public void SetupGridInfo(string _hostInfo)
        {

        }

        private void SaveProcessId(int processId)
        {
            IFormatter binaryFormatter = new BinaryFormatter();
            Stream stream = new FileStream("PID", FileMode.Create, FileAccess.Write, FileShare.None);
            binaryFormatter.Serialize(stream, processId);
            stream.Close();

        }
        private int GetProcessId()
        {
            IFormatter binaryFormatter = new BinaryFormatter();
            FileStream readStream = new FileStream("PID", FileMode.Open);
            int readData = (int)binaryFormatter.Deserialize(readStream);
            readStream.Close();
            return readData;
        }

        private string ExecuteCommand(string command)
        {
            try
            {
                FileStream fs1 = new FileStream("command.bat", FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);
                writer.Write(command);
                writer.Close();
                fs1.Close();
                fs1.Dispose();

                ProcessStartInfo processStartInfo = new ProcessStartInfo("command.bat");
                processStartInfo.CreateNoWindow = false;
                
                Process _process = new Process();
                _process.StartInfo = processStartInfo;
                //_process.StartInfo.RedirectStandardOutput = true;
                //_process.StartInfo.RedirectStandardError = true;
                //_process.OutputDataReceived += new DataReceivedEventHandler(_process_OutputDataReceived);
                //_process.ErrorDataReceived += new DataReceivedEventHandler(_process_ErrorDataReceived);
                _process.StartInfo.UseShellExecute = false;
             
                _process.Start();
                //_process.BeginOutputReadLine();
                //string error = _process.StandardError.ReadToEnd();
                //exitMessage = error;
                SaveProcessId(_process.Id);
                _process.Close();

                return exitMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }

            //if (exitCode != 0)
            //    exitMessage = "Command executed successfully ";
            //else
            //    exitMessage = "Command exited with exit code " + exitCode;

        }

        void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

        }

        void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //if (e.Data != null)
            //{
            //    if (!e.Data.ToLower().Contains("registering") && iCount <= 30)
            //    {
            //        exitMessage += e.Data;
            //        exit = false;
            //    }
            //    else
            //        exit = true;
            //}
            //else
            //    exit = true;
            //iCount++;
        }

        private string FindByDisplayName(RegistryKey parentKey, string name)
        {
            string[] nameList = parentKey.GetSubKeyNames();
            for (int i = 0; i < nameList.Length; i++)
            {
                RegistryKey regKey = parentKey.OpenSubKey(nameList[i]);
                try
                {
                    if (regKey.GetValue("DisplayName").ToString().ToLower().Contains(name.ToLower()))
                    {
                        return regKey.GetValue("InstallLocation").ToString();
                    }
                }
                catch { }
            }
            return "";
        }

        public string ConfigureHostFile(RequestData _hostInfo)
        {
            try
            {
                //Console.WriteLine(_hostInfo.details);
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string hostFileLocation = "" + winDir + @"\System32\drivers\etc\hosts";
                File.Copy(hostFileLocation, "Data\\BackupHost\\hosts", true);

                FileStream fs1 = new FileStream(hostFileLocation, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);
                _hostInfo.details = _hostInfo.details.Replace("\n", "\r\n");
                writer.Write(_hostInfo.details);
                writer.Close();
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
            return "Successfully updated";
        }

        public string InitializeSeleniumDriver(IntializeSeleniumGridDriveRequestData data)
        {
            string exitMessage = "";
            //string browser = data.browser, browserInfo = data.browserInfo, hostIp = data.hostIp;
            string hostIp = data.hostIp;
            string browser = data.Browsers[0].browser;
            string browserInfo = data.Browsers[0].browserInfo;

            Console.WriteLine("Browsers: " + browser + " browserInfo: " + browserInfo + " hostIp: " + hostIp);
            try
            {
                string browserParam = "", browserDriverPath = "";

                string jarCommandLineArguments = @"java -jar """ + Path.GetFullPath("Data/selenium-server-standalone-2.37.0.jar") + "\" -role node -port 8002 ";

                if (!string.IsNullOrEmpty(hostIp))
                {
                    browserParam = string.Format(" -hub http://{0}:4444/grid/register  ", hostIp);
                    browserDriverPath = "";
                }

                jarCommandLineArguments += browserDriverPath + browserParam;
                browserDriverPath = "";
                browserParam = "";


                if (!string.IsNullOrEmpty(browser) && !string.IsNullOrEmpty(browserInfo))
                {
                    if (browser.ToLower() == "chrome")
                    {
                        browserParam = string.Format(" -browser \"browserName={0}, {1}\" ", browser, browserInfo);
                        browserDriverPath = string.Format(" -Dwebdriver.chrome.driver=\"{0}\" ", Path.GetFullPath("Data/chromedriver.exe"));
                    }
                    jarCommandLineArguments += browserDriverPath + browserParam;
                    browserDriverPath = "";
                    browserParam = "";

                    if (browser.ToLower() == "firefox")
                    {
                        RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                        string location = FindByDisplayName(regKey, "firefox");

                        browserParam = string.Format(" -browser \"browserName={0}, {1}\" ", browser, browserInfo);
                        browserDriverPath = string.Format(" -Dwebdriver.firefox.driver=\"{0}\" ", location);
                    }
                    jarCommandLineArguments += browserDriverPath + browserParam;
                    browserDriverPath = "";
                    browserParam = "";

                    if (browser.ToLower() == "ie")
                    {
                        browserParam = string.Format(" -browser \"browserName={0}, {1}\" ", browser, browserInfo);
                        browserDriverPath = string.Format(" -Dwebdriver.ie.driver=\"{0}\" ", Path.GetFullPath("Data/IEDriverServer.exe"));
                    }
                    jarCommandLineArguments += browserDriverPath + browserParam;
                    browserDriverPath = "";
                    browserParam = "";
                }
                exitMessage = ExecuteCommand(jarCommandLineArguments);
                Console.WriteLine(exitMessage);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            //if (!exit)
            //    Thread.Sleep(1000);

            return exitMessage;
        }

        public string RestoreHostFile()
        {
            try
            {
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string hostFileLocation = "" + winDir + @"\System32\drivers\etc\hosts";
                if (File.Exists("Data\\BackupHost\\hosts"))
                    File.Copy("Data\\BackupHost\\hosts", hostFileLocation, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return "Successfully restored";
        }

        public void StopNode()
        {
            try
            {
                int processId = GetProcessId();
                Process process = Process.GetProcessById(processId);
                
                process.Kill();
                process.Close();
                Console.WriteLine("Process killed");
                //File.Delete("PID");
                // killProcess(processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void KillProcessByName(ProcessInfo ProcessInfo)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(ProcessInfo.ProcessName))
                {
                    process.Kill();
                }
                Console.WriteLine("Killed process by name");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void StartExecutableByName(Executable executableDetails)
        {
            try
            {
                string tempPath = ByteArrayToFile(executableDetails.fileNameToBeUploaded, Convert.FromBase64String(executableDetails.fileToBeUploaded));

                Process.Start(executableDetails.path, tempPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string GetMachineName()
        {
           return  Environment.MachineName;
        }

        private string ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), _FileName);
                System.IO.FileStream _FileStream = new System.IO.FileStream(tempPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                _FileStream.Close();
                return tempPath;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            return string.Empty;
        }
    }

    [DataContract(Namespace = "http://uri.org")]
    public class ProcessInfo
    {
        [DataMember]
        public string ProcessName { get; set; }
    }
    [DataContract(Namespace = "http://uri.org")]
    public class RequestData
    {
        [DataMember]
        public string details { get; set; }
    }
    //[DataContract(Namespace = "http://uri.org")]
    //public class IntializeSeleniumGridDriveRequestData
    //{
    //    [DataMember]
    //    public string browser{get; set;}
    //    [DataMember]
    //    public string browserInfo{get; set;}
    //    [DataMember]
    //    public string hostIp { get; set; }
    //}
    [DataContract(Namespace = "http://uri.org")]
    public class Executable
    {
        [DataMember]
        public string path { get; set; }
        [DataMember]
        public string fileNameToBeUploaded { get; set; }
        [DataMember]
        public string fileToBeUploaded { get; set; }
    }


     [DataContract(Namespace = "http://uri.org")]
    public class IntializeSeleniumGridDriveRequestData
    {

        private string hostIpField;

        private IntializeSeleniumGridDriveRequestDataBrowser[] browsersField;
 
        [DataMember]
        public string hostIp
        {
            get
            {
                return this.hostIpField;
            }
            set
            {
                this.hostIpField = value;
            }
        }

       [DataMember]
       [System.Xml.Serialization.XmlArrayItemAttribute("Browser", IsNullable = false)]
        public IntializeSeleniumGridDriveRequestDataBrowser[] Browsers
        {
            get
            {
                return this.browsersField;
            }
            set
            {
                this.browsersField = value;
            }
        }
    }

    [DataContract]
    public class IntializeSeleniumGridDriveRequestDataBrowser
    {

        private string browserField;

        private string browserInfoField;

        [DataMember]
        public string browser
        {
            get
            {
                return this.browserField;
            }
            set
            {
                this.browserField = value;
            }
        }

        [DataMember]
        public string browserInfo
        {
            get
            {
                return this.browserInfoField;
            }
            set
            {
                this.browserInfoField = value;
            }
        }
    }








}