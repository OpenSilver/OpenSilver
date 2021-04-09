#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows
{
    public enum InstallState
    {
        NotInstalled = 0,
        Installing = 1,
        Installed = 2,
        InstallFailed = 3
    }

    public partial class Application
    {
        public event EventHandler InstallStateChanged;
        public InstallState InstallState { get; }
        public bool Install()
        {
            return true;
        }

        public void RegisterName(string name, object scopedElement)
        {

        }
    }

    public partial class VisualStateGroup : DependencyObject
    {
        public VisualStateGroup() { }
        public event EventHandler<VisualStateChangedEventArgs> CurrentStateChanged;
    }

    public partial class Window
    {
        public string Title { get; set; }
        public bool Install()
        {
            return true;
        }
    }
}
#endif