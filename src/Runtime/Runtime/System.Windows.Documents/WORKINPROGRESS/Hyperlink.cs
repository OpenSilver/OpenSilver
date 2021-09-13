#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
	public sealed partial class Hyperlink
	{
        [OpenSilver.NotImplemented]
		public string TargetName { get; set; }
	}
}
