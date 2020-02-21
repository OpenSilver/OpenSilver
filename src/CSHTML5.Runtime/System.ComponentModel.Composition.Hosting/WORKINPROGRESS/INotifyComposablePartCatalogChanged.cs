#if WORKINPROGRESS
namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Provides notifications when a System.ComponentModel.Composition.Primitives.ComposablePartCatalog
    //     changes.
    public partial interface INotifyComposablePartCatalogChanged
    {
        //
        // Summary:
        //     Occurs when a System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        //     has changed.
        event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        //
        // Summary:
        //     Occurs when a System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        //     is changing.
        event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
    }
}

#endif