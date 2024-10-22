using Microsoft.AspNetCore.Components.WebView.Maui;

namespace OpenSilver.Maui;

public interface IWebViewManagerService : IDisposable
{
    Task InitializeWebViewAsync(IBlazorWebView blazorWebView, Action startAppAction);
}