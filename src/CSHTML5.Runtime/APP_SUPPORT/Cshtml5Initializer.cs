using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetForHtml5
{
    public static class Cshtml5Initializer
    {
        public static void Initialize()
        {
            DotNetForHtml5.Core.INTERNAL_Simulator.JavaScriptExecutionHandler = new JavaScriptExecutionHandler();
#if MIGRATION            
            EmulatorWithoutJavascript.StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(typeof(System.Windows.Controls.Button).Assembly);
            //EmulatorWithoutJavascript.StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(typeof(System.Resources.ResourceManager).Assembly);
#else
            EmulatorWithoutJavascript.StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(typeof(Windows.UI.Xaml.Controls.Button).Assembly);
#endif
        }
    }
}
