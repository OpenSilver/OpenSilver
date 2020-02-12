#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial interface IDataObject
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

#endif
