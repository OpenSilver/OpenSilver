#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

internal interface IPanel
{
    bool IsItemsHost { get; }
}
