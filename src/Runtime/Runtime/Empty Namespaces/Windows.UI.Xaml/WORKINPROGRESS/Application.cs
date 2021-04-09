#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
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
}
#endif