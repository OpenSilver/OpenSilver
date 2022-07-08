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
        string GetAllText();
        void SetText(int start, int length, string text);
        void SelectAll();
        object GetPropertyValue(DependencyProperty prop, int start, int length);
        void SetPropertyValue(DependencyProperty prop, object value, int start, int length);
        string GetContents();
        void Clear();
        void InsertText(string text);
    }
}
