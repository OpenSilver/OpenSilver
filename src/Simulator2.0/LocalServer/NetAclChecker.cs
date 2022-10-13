/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{

    // Credits: http://stackoverflow.com/questions/2583347/c-sharp-httplistener-without-using-netsh-to-register-a-uri

    internal static class NetAclChecker
    {
        internal static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        internal static void AddAddress(string address, string domain, string user)
        {
            //var sb = new StringBuilder();

            //string args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);
            string args = string.Format(@"http add urlacl url={0} user=everyone", address, domain, user);

            Process p = new Process();

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = false;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.UseShellExecute = true;
            psi.RedirectStandardError = false;
            psi.RedirectStandardOutput = false;

            p.StartInfo = psi;
            //p.OutputDataReceived += (sender, a) => sb.AppendLine(a.Data);
            //p.ErrorDataReceived += (sender, a) => sb.AppendLine(a.Data);

            Process.Start(psi).WaitForExit();

            //MessageBox.Show(sb.ToString());
        }
    }
}
*/