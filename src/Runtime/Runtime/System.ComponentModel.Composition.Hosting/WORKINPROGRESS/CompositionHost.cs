

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

// UGiacobbi2000104 First implementation

using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>Provides static methods to control the container used by <see cref="T:System.ComponentModel.Composition.CompositionInitializer" />.</summary>
    public static class CompositionHost
    {
        internal static CompositionContainer _container = (CompositionContainer)null;
        
        // Better to add some thread safety here
        private static object _lockObject = new object();

        /// <summary>Sets <see cref="T:System.ComponentModel.Composition.CompositionInitializer" /> to use the specified container.</summary>
        /// <param name="container">The container to use.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="container" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">This method has already been called.</exception>
        public static void Initialize(CompositionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            CompositionContainer globalContainer = (CompositionContainer)null;
            
            if (CompositionHost.TryGetOrCreateContainer((Func<CompositionContainer>)(() => container), out globalContainer))
                //TODO: Localize this string
                throw new InvalidOperationException("Global container already initialized.");
        }

        /// <summary>Sets <see cref="T:System.ComponentModel.Composition.CompositionInitializer" /> to use a new container initialized with the specified catalogs.</summary>
        /// <returns>The new container.</returns>
        /// <param name="catalogs">The catalogs to load into the new container.</param>
        public static CompositionContainer Initialize(params ComposablePartCatalog[] catalogs)
        {
            AggregateCatalog catalog = new AggregateCatalog(catalogs);
            // ExportProvider exportProvider = new ExportProvider();
            var container = new CompositionContainer(catalog, null /*exportProvider*/);

            //CompositionContainer container = new CompositionContainer((ComposablePartCatalog)catalog, new ExportProvider[0]);
            try
            {
                CompositionHost.Initialize(container);
            }
            catch
            {
                container.Dispose();
                catalog.Catalogs.Clear();
                catalog.Dispose();
                throw;
            }
            return container;
        }

        #region Implementation

        internal static bool TryGetOrCreateContainer(Func<CompositionContainer> createContainer, out CompositionContainer globalContainer)
        {
            bool container = true;

            if (CompositionHost._container == null)
            {
                CompositionContainer compositionContainer = createContainer();
                lock (CompositionHost._lockObject)
                {
                    if (CompositionHost._container == null)
                    {
                        Thread.MemoryBarrier();
                        CompositionHost._container = compositionContainer;
                        container = false;
                    }
                }
            }

            globalContainer = CompositionHost._container;

            return container;
        }

        #endregion
    }
}
