

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


#if WORKINPROGRESS

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    //
    // Summary:
    //     The XAML proxy of a collection view class.
    public class CollectionViewSource : DependencyObject, ISupportInitialize
    {
        //
        // Summary:
        //     Identifies the System.Windows.Data.CollectionViewSource.Source dependency property.
        public static readonly DependencyProperty SourceProperty;
        //
        // Summary:
        //     Identifies the System.Windows.Data.CollectionViewSource.View dependency property.
        public static readonly DependencyProperty ViewProperty;

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Data.CollectionViewSource class.
        public CollectionViewSource()
        {

        }

        //
        // Summary:
        //     Gets or sets the cultural information for any operations of the view that might
        //     differ by culture, such as sorting.
        //
        // Returns:
        //     The culture to use during view operations.
        public CultureInfo Culture { get; set; }
        //
        // Summary:
        //     Gets a collection of System.ComponentModel.GroupDescription objects that describe
        //     how items in the collection are grouped in the view.
        //
        // Returns:
        //     A collection of System.ComponentModel.GroupDescription objects that describe
        //     how items in the collection are grouped in the view.
        public ObservableCollection<GroupDescription> GroupDescriptions { get; }
        //
        // Summary:
        //     Gets a collection of System.ComponentModel.SortDescription objects that describe
        //     how the items in the collection are sorted in the view.
        //
        // Returns:
        //     A collection of System.ComponentModel.SortDescription objects that describe how
        //     the items in the collection are sorted in the view.
        public SortDescriptionCollection SortDescriptions { get; }
        //
        // Summary:
        //     Gets or sets the collection object from which to create this view.
        //
        // Returns:
        //     The collection object from which to create this view. The default is null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The specified value when setting this property is not null or an System.Collections.IEnumerable
        //     implementation.-or-The specified value when setting this property is an System.ComponentModel.ICollectionView
        //     implementation.
        //
        //   T:System.InvalidOperationException:
        //     The specified value implements System.ComponentModel.ICollectionViewFactory but
        //     its System.ComponentModel.ICollectionViewFactory.CreateView method returns an
        //     System.ComponentModel.ICollectionView with one or more of the following inconsistencies:System.ComponentModel.ICollectionView.CanFilter
        //     is false but System.ComponentModel.ICollectionView.Filter is not null.System.ComponentModel.ICollectionView.CanSort
        //     is false but System.ComponentModel.ICollectionView.SortDescriptions is not empty.System.ComponentModel.ICollectionView.CanGroup
        //     is false but System.ComponentModel.ICollectionView.GroupDescriptions is not empty.
        public object Source { get; set; }
        //
        // Summary:
        //     Gets the view object that is currently associated with this instance of System.Windows.Data.CollectionViewSource.
        //
        // Returns:
        //     The view object that is currently associated with this instance of System.Windows.Data.CollectionViewSource.
        public ICollectionView View { get; }

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

        //
        // Summary:
        //     Enters a defer cycle that you can use to merge changes to the view and delay
        //     automatic refresh.
        //
        // Returns:
        //     An System.IDisposable object that you can use to dispose of the calling object.
        public IDisposable DeferRefresh()
        {
            return null;
        }
        //
        // Summary:
        //     Invoked when the collection view type changes.
        //
        // Parameters:
        //   oldCollectionViewType:
        //     The old collection view type.
        //
        //   newCollectionViewType:
        //     The new collection view type.
        protected virtual void OnCollectionViewTypeChanged(Type oldCollectionViewType, Type newCollectionViewType)
        {

        }
        //
        // Summary:
        //     Invoked when the System.Windows.Data.CollectionViewSource.Source property changes.
        //
        // Parameters:
        //   oldSource:
        //     The old value of the System.Windows.Data.CollectionViewSource.Source property.
        //
        //   newSource:
        //     The new value of the System.Windows.Data.CollectionViewSource.Source property.
        protected virtual void OnSourceChanged(object oldSource, object newSource)
        {

        }

        void ISupportInitialize.BeginInit()
        {
            throw new NotImplementedException();
        }

        void ISupportInitialize.EndInit()
        {
            throw new NotImplementedException();
        }
    }
}

#endif