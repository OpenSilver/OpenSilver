using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler.Common
{
    public interface IAssemblyInspectorOnOtherDomain
    {
        object InitializeLifetimeService();

        void LoadAssembly(string path);

        string GetAssemblyName();
        
        string GetAssemblyFullName();

        string FindApplicationClassFullName();
    }
}
