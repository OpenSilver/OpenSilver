
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

using System.ComponentModel.Composition.Primitives;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    /// Provides static methods to control the container used by <see cref="CompositionInitializer" />.
    /// </summary>
    public static class CompositionHost
    {
        private static CompositionContainer _container;
        private static readonly object _lockObject = new();

        /// <summary>
        /// Sets <see cref="CompositionInitializer" /> to use the specified container.
        /// </summary>
        /// <param name="container">
        /// The container to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This method has already been called.
        /// </exception>
        public static void Initialize(CompositionContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (TryGetOrCreateContainer(() => container, out CompositionContainer globalContainer))
            {
                throw new InvalidOperationException(Resources.Error_ContainerAlreadyInitialized);
            }
        }

        /// <summary>
        /// Sets <see cref="CompositionInitializer" /> to use a new container initialized 
        /// with the specified catalogs.
        /// </summary>
        /// <param name="catalogs">
        /// The catalogs to load into the new container.
        /// </param>
        /// <returns>
        /// The new container.
        /// </returns>
        public static CompositionContainer Initialize(params ComposablePartCatalog[] catalogs)
        {
            var catalog = new AggregateCatalog(catalogs);
            var container = new CompositionContainer(catalog);
            
            try
            {
                Initialize(container);
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

        internal static bool TryGetOrCreateContainer(Func<CompositionContainer> createContainer, out CompositionContainer globalContainer)
        {
            bool flag = true;

            if (_container == null)
            {
                CompositionContainer compositionContainer = createContainer();
                lock (_lockObject)
                {
                    if (_container == null)
                    {
                        Thread.MemoryBarrier();
                        _container = compositionContainer;
                        flag = false;
                    }
                }
            }

            globalContainer = _container;

            return flag;
        }
    }
}
