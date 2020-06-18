#if WORKINPROGRESS
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Provides data for the System.ComponentModel.Composition.Hosting.INotifyComposablePartCatalogChanged.Changed
    //     event.
    public partial class ComposablePartCatalogChangeEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs
        //     class with the specified changes.
        //
        // Parameters:
        //   addedDefinitions:
        //     The part definitions that were added to the catalog.
        //
        //   removedDefinitions:
        //     The part definitions that were removed from the catalog.
        //
        //   atomicComposition:
        //     The composition transaction to use, or null to disable transactional composition.
        public ComposablePartCatalogChangeEventArgs(IEnumerable<ComposablePartDefinition> addedDefinitions, IEnumerable<ComposablePartDefinition> removedDefinitions, AtomicComposition atomicComposition)
        {

        }

        //
        // Summary:
        //     Gets a collection of definitions added to the System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        //     in this change.
        //
        // Returns:
        //     A collection of definitions added to the catalog.
        public IEnumerable<ComposablePartDefinition> AddedDefinitions { get; private set; }
        //
        // Summary:
        //     Gets the composition transaction for this change.
        //
        // Returns:
        //     The composition transaction for this change.
        public AtomicComposition AtomicComposition { get; private set; }
        //
        // Summary:
        //     Gets a collection of definitions removed from the System.ComponentModel.Composition.Primitives.ComposablePartCatalog
        //     in this change.
        //
        // Returns:
        //     A collection of definitions removed from the catalog in this change.
        public IEnumerable<ComposablePartDefinition> RemovedDefinitions { get; private set; }
    }
}
#endif