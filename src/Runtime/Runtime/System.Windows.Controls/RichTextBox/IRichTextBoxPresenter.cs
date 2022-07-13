#if MIGRATION
using System.Windows.Documents;

namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal interface IRichTextBoxPresenter
    {
        TextSelection Selection { get; }
        void Init();
        string GetText(int start, int length);
        string GetText();
        void SetText(int start, int length, string text);
        void SetText(string text);
        void SelectAll();
        object GetPropertyValue(DependencyProperty prop, int start, int length);
        void SetPropertyValue(DependencyProperty prop, object value, int start, int length);
        string GetContents();
        string GetContents(int start, int length);
        void Clear();
        void InsertText(string text);
        void SetReadOnly(bool value);
        void SetEnable(bool value);
    }
}
