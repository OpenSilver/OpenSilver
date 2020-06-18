#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public partial class Binding
	{
		public Binding(Binding original)
		{
			original?.CopyTo(this);
		}
		
		/// <summary>
		/// Gets or sets a value that indicates whether the binding ignores any System.ComponentModel.ICollectionView
		/// settings on the data source.
		/// </summary>
		public bool BindsDirectlyToSource { get; set; }

		public bool ValidatesOnNotifyDataErrors { get; set; }
	}
}

#endif