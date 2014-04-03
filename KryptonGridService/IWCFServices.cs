using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace KryptonGridService
{
    [ServiceContract(Name = "KryptonGridService")]
    public interface IWCFServices
    {
        [OperationContract]
        [WebInvoke(Method="POST", ResponseFormat=WebMessageFormat.Xml, 
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/ConfigureHostFile")]
        string ConfigureHostFile(RequestData hostInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml,
          RequestFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/InitializeSeleniumDriver")]
        string InitializeSeleniumDriver(IntializeSeleniumGridDriveRequestData data);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml,
          RequestFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/RestoreHostFile")]
        string RestoreHostFile();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/StopNode")]
        void StopNode();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/KillProcessByName")]
        void KillProcessByName(ProcessInfo ProcessInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/StartExecutableByName")]
        void StartExecutableByName(Executable executableDetails);
        
        [OperationContract]
        [WebGet(UriTemplate = "/GetMachineName")]
        string GetMachineName();
    }
}