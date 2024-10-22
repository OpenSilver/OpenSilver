using Microsoft.AspNetCore.Components.WebView.Maui;

namespace OpenSilver.Maui;

public partial class WebViewManagerService : IWebViewManagerService
{
    public partial Task InitializeWebViewAsync(IBlazorWebView blazorWebView, Action startAppAction);

    protected SingleThreadTaskScheduler? Scheduler;

    #region IDisposable implementation

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Scheduler?.Dispose();
            }

            _disposed = true;
        }
    }

    #endregion IDisposable implementation
}