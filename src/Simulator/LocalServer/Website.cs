
// Credits: http://stackoverflow.com/questions/4772092/starting-and-stopping-iis-express-programmatically


using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using IISVersionManagerLibrary;
using Microsoft.Web.Administration;
using System.Runtime.InteropServices;

public class Website
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    /// <summary>
    /// Win32 API Constants for ShowWindowAsync()
    /// </summary>
    private const int SW_HIDE = 0;
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;
    private const int SW_SHOWNOACTIVATE = 4;
    private const int SW_RESTORE = 9;
    private const int SW_SHOWDEFAULT = 10;


    private const string DefaultAppPool = "Clr4IntegratedAppPool";
    private const string DefaultIISVersion = "8.0";
    private const string DefaultIP = "localhost";

    private static readonly Random Random = new Random();
    private readonly IIISExpressProcessUtility _iis;
    private readonly string _name;
    private readonly string _path;
    private readonly string _ip;
    private readonly int _port;
    private readonly string _appPool;
    private readonly string _iisPath;
    private readonly string _iisArguments;
    private readonly string _iisConfigPath;
    private uint _iisHandle;

    private Website(string path, string name, string ip, int port, string appPool, string iisVersion)
    {
        _path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path));
        _name = name;
        _ip = ip;
        _port = port;
        _appPool = appPool;
        _iis = (IIISExpressProcessUtility)new IISVersionManager()
            .GetVersionObject(iisVersion, IIS_PRODUCT_TYPE.IIS_PRODUCT_EXPRESS)
            .GetPropertyValue("expressProcessHelper");
        var commandLine = _iis.ConstructCommandLine(name, "", appPool, "");
        var commandLineParts = new Regex("\\\"(.*?)\\\" (.*)").Match(commandLine);
        _iisPath = commandLineParts.Groups[1].Value;
        _iisArguments = commandLineParts.Groups[2].Value;
        _iisConfigPath = new Regex("\\/config:\\\"(.*?)\\\"").Match(commandLine).Groups[1].Value;
        Url = string.Format("http://{0}:{1}/", _ip, _port);
    }

    public static Website Create(string path,
        string name = null, int? port = null,
        string appPool = DefaultAppPool,
        string iisVersion = DefaultIISVersion,
        string ip = DefaultIP)
    {
        return new Website(path,
            name ?? Guid.NewGuid().ToString("N"),
            ip,
            port ?? Random.Next(30000, 40000),
            appPool, iisVersion);
    }

    public string Url { get; private set; }
    public int Port { get { return _port; } }

    public void Start()
    {
        using (var manager = new ServerManager(_iisConfigPath))
        {
            manager.Sites.Add(_name, "http", string.Format("*:{0}:{1}", _port, _ip), _path);
            manager.CommitChanges();
        }
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = _iisPath,
            Arguments = _iisArguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal
        });
        var startTime = DateTime.Now;
        do
        {
            try
            {
                _iisHandle = _iis.GetRunningProcessForSite(_name, "", _appPool, "");
            }
            catch { }
            if (_iisHandle != 0) break;
            if ((DateTime.Now - startTime).Seconds >= 10)
                throw new TimeoutException("Timeout starting IIS Express.");
        } while (true);
        System.Threading.Thread.Sleep(300);
        ShowWindow(process.MainWindowHandle, SW_SHOWMINIMIZED);
    }

    public void Stop()
    {
        try
        {
            _iis.StopProcess(_iisHandle);
        }
        finally
        {
            using (var manager = new ServerManager(_iisConfigPath))
            {
                var site = manager.Sites[_name];
                manager.Sites.Remove(site);
                manager.CommitChanges();
            }
        }
    }
}