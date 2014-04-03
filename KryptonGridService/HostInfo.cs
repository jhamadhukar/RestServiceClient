using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KryptonGridService
{
    public class HostInfo1
    {
        private string _key;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

    }
}
