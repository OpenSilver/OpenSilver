#if WORKINPROGRESS

using System.Windows;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
	public partial class CollectionViewSource : DependencyObject, ISupportInitialize
	{
		/// <summary>
		/// Identifies the System.Windows.Data.CollectionViewSource.Source dependency property.
		/// </summary>
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), null);
		
		/// <summary>
		/// public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(CollectionViewSource), null);
		/// </summary>
		public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("ViewProperty", typeof(ICollectionView), typeof(CollectionViewSource), new PropertyMetadata());
		
		
		/// <summary>
		/// Initializes a new instance of the System.Windows.Data.CollectionViewSource class.
		/// </summary>
		public CollectionViewSource()
		{
		}
		
		/// <summary>
		/// Gets or sets the cultural information for any operations of the view that might differ by culture, such as sorting.
		/// </summary>
		/// <returns>
		/// The culture to use during view operations.
		/// </returns>
		public CultureInfo Culture { get; set; }
		
		/// <summary>
		/// Gets a collection of System.ComponentModel.GroupDescription objects that describe how items in the collection are grouped in the view.
		/// </summary>
		/// <returns>
		/// A collection of System.ComponentModel.GroupDescription objects that describe how items in the collection are grouped in the view.
		/// </returns>
		public ObservableCollection<GroupDescription> GroupDescriptions { get; }
		
		/// <summary>
		/// Gets a collection of System.ComponentModel.SortDescription objects that describe how the items in the collection are sorted in the view.
		/// </summary>
		/// <returns>
		/// A collection of System.ComponentModel.SortDescription objects that describe how the items in the collection are sorted in the view.
		/// </returns>
		public SortDescriptionCollection SortDescriptions { get; }
		
		/// <summary>
		/// Gets or sets the collection object from which to create this view.
		/// </summary>
		/// <returns>
		/// The collection object from which to create this view. The default is null.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// The specified value when setting this property is not null or an System.Collections.IEnumerable implementation.
		/// -or-
		/// The specified value when setting this property is an System.ComponentModel.ICollectionView implementation.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// The specified value implements System.ComponentModel.ICollectionViewFactory but
		/// its System.ComponentModel.ICollectionViewFactory.CreateView method returns an
		/// System.ComponentModel.ICollectionView with one or more of the following inconsistencies:
		/// System.ComponentModel.ICollectionView.CanFilter is false but System.ComponentModel.ICollectionView.Filter is not null.
		/// System.ComponentModel.ICollectionView.CanSort is false but System.ComponentModel.ICollectionView.SortDescriptions is not empty.
		/// System.ComponentModel.ICollectionView.CanGroup is false but System.ComponentModel.ICollectionView.GroupDescriptions is not empty.
		/// </exception>
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
		
		//
		// Summary:
		//     Provides filtering logic.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     When adding a handler to this event, the System.Windows.Data.CollectionViewSource.View
		//     property value has a System.ComponentModel.ICollectionView.CanFilter property
		//     value of false.
		public event FilterEventHandler Filter;
		
		/// <summary>
		/// Gets the view object that is currently associated with this instance of System.Windows.Data.CollectionViewSource.
		/// </summary>
		/// <returns>
		/// The view object that is currently associated with this instance of System.Windows.Data.CollectionViewSource.
		/// </returns>
		public ICollectionView View
		{
			get
			{
				return (ICollectionView)this.GetValue(CollectionViewSource.ViewProperty);
			}
		}

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
		}
	}
}

#endif