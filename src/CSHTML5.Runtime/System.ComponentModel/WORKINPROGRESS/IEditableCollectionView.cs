#if WORKINPROGRESS

namespace System.ComponentModel
{
    public interface IEditableCollectionView
    {
        bool CanAddNew { get; }
        bool CanCancelEdit { get; }
        bool CanRemove { get; }
        object CurrentAddItem { get; }
        object CurrentEditItem { get; }
        bool IsAddingNew { get; }
        bool IsEditingItem { get; }
        NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }
        object AddNew();
        void CancelEdit();
        void CancelNew();
        void CommitEdit();
        void CommitNew();
        void EditItem(object item);
        void Remove(object item);
        void RemoveAt(int idx);
    }
}

#endif