

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using DotNetBrowser;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal class BrowserResultConverter
    {
        public static object CastFromJsValue(object obj)
        {
            if (!(obj is JSValue res))
                return null;

            if (res.IsNull())// || res.IsUndefined()) //Note: res.IsUndefined has been specifically added but for some reason, it makes the Simulator freeze in CSHTML5 so we'll remove it for now. Keeping this note here so we konw.
                return null;

            if (res.IsString())
                return res.AsString().Value;
            if (res.IsBool())
                return res.AsBoolean().Value;
            if (res.IsNumber())
                return res.AsNumber().Value;
            if (int.TryParse(res.ToString(), out var resInt))
                return resInt;

            return res;
        }
    }
}
