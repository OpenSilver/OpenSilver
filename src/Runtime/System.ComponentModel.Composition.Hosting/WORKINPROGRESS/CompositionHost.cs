using System.ComponentModel.Composition.Primitives;

#if WORKINPROGRESS
namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Provides static methods to control the container used by System.ComponentModel.Composition.CompositionInitializer.
    public static class CompositionHost
    {
        //
        // Summary:
        //     Sets System.ComponentModel.Composition.CompositionInitializer to use the specified
        //     container.
        //
        // Parameters:
        //   container:
        //     The container to use.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     container is null.
        //
        //   T:System.InvalidOperationException:
        //     This method has already been called.
        public static void Initialize(CompositionContainer container)
        {

        }
        //
        // Summary:
        //     Sets System.ComponentModel.Composition.CompositionInitializer to use a new container
        //     initialized with the specified catalogs.
        //
        // Parameters:
        //   catalogs:
        //     The catalogs to load into the new container.
        //
        // Returns:
        //     The new container.
        public static CompositionContainer Initialize(params ComposablePartCatalog[] catalogs)
        {
            return null;
        }
    }
}
#endif