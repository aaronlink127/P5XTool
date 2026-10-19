using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib
{
    public partial class MonoScript : Object
    {
        public string m_Name = "";
        public string m_ClassName = "";
        public string m_Namespace = "";
        public string m_AssemblyName = "";
        public string FullClassName
        {
            get
            {
                if (string.IsNullOrEmpty(m_Namespace))
                    return m_ClassName;
                return m_Namespace + "." + m_ClassName;
            }
        }
    }
}
