
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

namespace System.Windows
{
    public interface IDataObject
    {
#region Methods
        object GetData(string @format);
        object GetData(Type @format);
        object GetData(string @format, bool @autoConvert);
        bool GetDataPresent(string @format);
        bool GetDataPresent(Type @format);
        bool GetDataPresent(string @format, bool @autoConvert);
        string[] GetFormats();
        string[] GetFormats(bool @autoConvert);
        void SetData(object @data);
        void SetData(string @format, object @data);
        void SetData(Type @format, object @data);
        void SetData(string @format, object @data, bool @autoConvert);
#endregion

    }
}
