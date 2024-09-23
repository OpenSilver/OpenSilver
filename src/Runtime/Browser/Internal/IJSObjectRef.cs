
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using OpenSilver.Internal;

namespace System.Windows.Browser.Internal;

internal interface IJSObjectRef : IJavaScriptConvertible, IDisposable { }

internal sealed class WindowRef : IJSObjectRef
{
    private const string Window = "window";

    public WindowRef() { }

    public string ToJavaScriptString() => Window;

    public void Dispose() { }
}

internal sealed class DocumentRef : IJSObjectRef
{
    private const string Document = "document";

    public DocumentRef() { }

    public string ToJavaScriptString() => Document;

    public void Dispose() { }
}

internal sealed class JSObjectRef : IJSObjectRef
{
    private readonly string _jsRef;

    public JSObjectRef(string jsRef)
    {
        _jsRef = jsRef;
    }

    public string ToJavaScriptString() => $"document.browserService.getObject('{_jsRef}')";

    public void Dispose()
    {
        ScriptObject.UnregisterScriptObject(_jsRef);
        OpenSilver.Interop.ExecuteJavaScriptVoid($"document.browserService.releaseObject('{_jsRef}');");
    }
}
