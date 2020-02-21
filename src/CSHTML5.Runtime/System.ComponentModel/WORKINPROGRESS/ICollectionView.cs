#if WORKINPROGRESS
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System;
using System.Globalization;

namespace System.ComponentModel
{
    public partial interface ICollectionView : IEnumerable, INotifyCollectionChanged
    {
        bool CanFilter { get; }
        bool CanGroup { get; }
        bool CanSort { get; }
        CultureInfo Culture { get; set; }
        object CurrentItem { get; }
        int CurrentPosition { get; }
        Predicate<object> Filter { get; set; }
        ObservableCollection<GroupDescription> GroupDescriptions { get; }
        ReadOnlyObservableCollection<object> Groups { get; }
        bool IsCurrentAfterLast { get; }
        bool IsCurrentBeforeFirst { get; }
        bool IsEmpty { get; }
        SortDescriptionCollection SortDescriptions { get; }
        IEnumerable SourceCollection { get; }
        bool Contains(object item);
        IDisposable DeferRefresh();
        bool MoveCurrentTo(object item);
        bool MoveCurrentToFirst();
        bool MoveCurrentToLast();
        bool MoveCurrentToNext();
        bool MoveCurrentToPosition(int position);
        bool MoveCurrentToPrevious();
        void Refresh();
        event EventHandler CurrentChanged;
        event CurrentChangingEventHandler CurrentChanging;
    }
}

#endif