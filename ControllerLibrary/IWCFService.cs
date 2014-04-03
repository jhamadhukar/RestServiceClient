using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ControllerLibrary
{
    [ServiceContract(Name = "KryptonServiceContract")]


    public interface IWCFServices
    {
        [OperationContract(IsOneWay = true)]
        void SetupGridInfo(string _hostInfo);
    }
}
