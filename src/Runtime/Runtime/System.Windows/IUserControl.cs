#if MIGRATION
namespace System.Windows;
#else
namespace Windows.UI.Xaml;
#endif

public interface IUserControl : IControl
{
}
