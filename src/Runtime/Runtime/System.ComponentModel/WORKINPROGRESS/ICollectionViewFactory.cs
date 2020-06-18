#if WORKINPROGRESS
namespace System.ComponentModel
{
    //
    // Summary:
    //     Defines a method that enables a collection to provide a custom view for specialized
    //     sorting, filtering, grouping, and currency.
    public partial interface ICollectionViewFactory
    {
        //
        // Summary:
        //     Returns a custom view for specialized sorting, filtering, grouping, and currency.
        //
        // Returns:
        //     A custom view for specialized sorting, filtering, grouping, and currency.
        ICollectionView CreateView();
    }
}
#endif