using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Provides static methods to control the container used by System.ComponentModel.Composition.CompositionInitializer.
	[OpenSilver.NotImplemented]
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
		[OpenSilver.NotImplemented]
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
		[OpenSilver.NotImplemented]
        public static CompositionContainer Initialize(params ComposablePartCatalog[] catalogs)
        {
            return null;
        }
    }
}
