#if WORKINPROGRESS
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CompositionContainer : ExportProvider, ICompositionService, IDisposable
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     class.
        public CompositionContainer()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     class with the specified export providers.
        //
        // Parameters:
        //   providers:
        //     An array of System.ComponentModel.Composition.Hosting.ExportProvider objects
        //     that provide the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     access to System.ComponentModel.Composition.Primitives.Export objects, or null
        //     to set System.ComponentModel.Composition.Hosting.CompositionContainer.Providers
        //     to an empty System.Collections.ObjectModel.ReadOnlyCollection`1.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     providers contains an element that is null.
        public CompositionContainer(params ExportProvider[] providers)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     class with the specified catalog and export providers.
        //
        // Parameters:
        //   catalog:
        //     A catalog that provides System.ComponentModel.Composition.Primitives.Export objects
        //     to the System.ComponentModel.Composition.Hosting.CompositionContainer.
        //
        //   providers:
        //     An array of System.ComponentModel.Composition.Hosting.ExportProvider objects
        //     that provide the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     access to System.ComponentModel.Composition.Primitives.Export objects, or null
        //     to set System.ComponentModel.Composition.Hosting.CompositionContainer.Providers
        //     to an empty System.Collections.ObjectModel.ReadOnlyCollection`1.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     providers contains an element that is null.
        public CompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     class with the specified catalog, thread-safe mode, and export providers.
        //
        // Parameters:
        //   catalog:
        //     A catalog that provides System.ComponentModel.Composition.Primitives.Export objects
        //     to the System.ComponentModel.Composition.Hosting.CompositionContainer.
        //
        //   isThreadSafe:
        //     true if this System.ComponentModel.Composition.Hosting.CompositionContainer object
        //     must be thread-safe; otherwise, false.
        //
        //   providers:
        //     An array of System.ComponentModel.Composition.Hosting.ExportProvider objects
        //     that provide the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     access to System.ComponentModel.Composition.Primitives.Export objects, or null
        //     to set the System.ComponentModel.Composition.Hosting.CompositionContainer.Providers
        //     property to an empty System.Collections.ObjectModel.ReadOnlyCollection`1.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     providers contains a null element.
        public CompositionContainer(ComposablePartCatalog catalog, bool isThreadSafe, params ExportProvider[] providers)
        {

        }

        public void Dispose()
        {

        }

        public void SatisfyImportsOnce(ComposablePart part)
        {

        }
    }
}

#endif