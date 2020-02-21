#if WORKINPROGRESS
using System.Windows;
using System.ComponentModel;
using System;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public partial class CollectionViewSource : DependencyObject, ISupportInitialize
	{
		public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("ViewProperty", typeof(ICollectionView), typeof(CollectionViewSource), new PropertyMetadata());
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), null);
		public ICollectionView View
		{
			get
			{
				return (ICollectionView)this.GetValue(CollectionViewSource.ViewProperty);
			}
		}

		public object Source
		{
			get
			{
				return this.GetValue(SourceProperty);
			}

			set
			{
				this.SetValue(SourceProperty, value);
			}
		}

		public CollectionViewSource()
		{
		}

		public void BeginInit()
		{
		}

		public void EndInit()
		{
		}
	}
}
#endif