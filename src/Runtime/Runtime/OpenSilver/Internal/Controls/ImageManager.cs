
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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal sealed class ImageManager
{
    private readonly JavaScriptCallback _loadHandler;
    private readonly JavaScriptCallback _errorHandler;

    private ImageManager()
    {
        _loadHandler = JavaScriptCallback.Create(ProcessLoadEvent);
        _errorHandler = JavaScriptCallback.Create(ProcessErrorEvent);
        string sLoadHandler = Interop.GetVariableStringForJS(_loadHandler);
        string sErrorHandler = Interop.GetVariableStringForJS(_errorHandler);
        Interop.ExecuteJavaScriptVoid($"document.createImageManager({sLoadHandler}, {sErrorHandler})");
    }

    public static ImageManager Instance { get; } = new();

    public void CreateImage(string id, string imgId, string parentId) =>
        Interop.ExecuteJavaScriptVoidAsync($"document.imgManager.create('{id}','{imgId}','{parentId}')");

    public Size GetNaturalSize(Image image)
    {
        Debug.Assert(image is not null && image.ImageDiv is not null);

        string sElement = Interop.GetVariableStringForJS(image.ImageDiv);
        double width = Interop.ExecuteJavaScriptDouble($"document.imgManager.getNaturalWidth({sElement})");
        double height = Interop.ExecuteJavaScriptDouble($"document.imgManager.getNaturalHeight({sElement})");
        return new Size(width, height);
    }

    private static void ProcessLoadEvent(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is Image image)
        {
            image.OnLoadNative();
        }
    }

    private static void ProcessErrorEvent(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is Image image)
        {
            image.OnErrorNative();
        }
    }
}
