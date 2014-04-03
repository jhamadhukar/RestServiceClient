using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace KryptonGridServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
           try
            {
                Process thisProc = Process.GetCurrentProcess();
                if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                {
                    return;
                }

                ServiceHost host = new ServiceHost(typeof(KryptonGridService.WCFServices));

                host.Open();
                foreach (ServiceEndpoint se in host.Description.Endpoints)
                {
                    Console.WriteLine("Address : {0}, Binding : {1}, Contract : {2}", se.Address, se.Binding, se.Contract.ContractType);
                }
                Console.WriteLine("               ");
                Console.WriteLine("               ");
                Console.WriteLine("               ");
                Console.WriteLine("Service Started");
                Console.ReadLine();

                host.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
