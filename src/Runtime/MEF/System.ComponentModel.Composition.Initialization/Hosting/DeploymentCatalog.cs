
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

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    /// Discovers attributed parts in a XAP file, and provides methods for asynchronously
    /// downloading XAP files.
    /// </summary>
    public class DeploymentCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly Uri _uri;
        private AggregateCatalog _catalog = new();
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/> 
        /// class using assemblies in the current XAP.
        /// </summary>
        public DeploymentCatalog()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/>
        /// class using the XAP file at the specified relative URI.
        /// </summary>
        /// <param name="uriRelative">The URI of the XAP file.</param>
		[OpenSilver.NotImplemented]
        public DeploymentCatalog(string uriRelative)
        {
            _uri = new Uri(uriRelative, UriKind.Relative);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/>
        /// class using the XAP file at the specified URI.
        /// </summary>
        /// <param name="uri">The URI of the XAP file.</param>
		[OpenSilver.NotImplemented]
        public DeploymentCatalog(Uri uri)
        {
            _uri = uri;
        }

        /// <summary>
        /// Gets all the parts contained in the catalog.
        /// </summary>
        /// <returns>
        /// A query enumerating all the parts contained in the catalog.
        /// </returns>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                ThrowIfDisposed();
                return _catalog.Parts;
            }
        }

        /// <summary>
        /// Gets the URI for the XAP file.
        /// </summary>
        public Uri Uri
        {
            get
            {
                ThrowIfDisposed();
                return _uri;
            }
        }

        /// <summary>
        /// Occurs when the contents of the <see cref="DeploymentCatalog"/>
        /// have changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Occurs when the contents of the <see cref="DeploymentCatalog"/>
        /// are changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        /// <summary>
        /// Occurs when the XAP file has finished downloading, or there has been an error.
        /// </summary>
		[OpenSilver.NotImplemented]
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;

        /// <summary>
        /// Occurs when the download progress of the XAP file changes.
        /// </summary>
		[OpenSilver.NotImplemented]
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Cancels the XAP file download in progress.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The method is called before <see cref="DownloadAsync"/> or after the 
        /// <see cref="DownloadCompleted"/> event has occurred.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void CancelAsync() => throw new NotImplementedException();

        /// <summary>
        /// Begins downloading the XAP file associated with the <see cref="DeploymentCatalog"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This method is called more than once, or after the <see cref="CancelAsync"/>
        /// method.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void DownloadAsync() => throw new NotImplementedException();

        /// <summary>
        /// Gets the export definitions that match the constraint expressed by the specified
        /// definition.
        /// </summary>
        /// <param name="definition">
        /// The conditions of the <see cref="ExportDefinition"/> objects to be returned.
        /// </param>
        /// <returns>
        /// A collection of <see cref="Tuple{T1, T2}"/> objects containing the <see cref="ExportDefinition"/>
        /// objects and their associated <see cref="ComposablePartDefinition"/>
        /// objects for objects that match the constraint specified by definition.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            ThrowIfDisposed();
            return _catalog.GetExports(definition);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DeploymentCatalog"/>
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!_isDisposed)
                    {
                        AggregateCatalog catalog = null;
                        try
                        {
                            catalog = _catalog;
                            _catalog = null;
                            _isDisposed = true;
                        }
                        finally
                        {
                            if (catalog != null)
                            {
                                catalog.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }            
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="ComposablePartCatalogChangeEventArgs"/> object that 
        /// contains the event data.
        /// </param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e) => Changed?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="ComposablePartCatalogChangeEventArgs"/> object that contains the event data.
        /// </param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e) => Changing?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="DownloadCompleted"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="AsyncCompletedEventArgs"/> object that contains the event data.
        /// </param>
        protected virtual void OnDownloadCompleted(AsyncCompletedEventArgs e) => DownloadCompleted?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="DownloadProgressChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DownloadProgressChangedEventArgs"/> object that contains the event data.
        /// </param>
        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e) => DownloadProgressChanged?.Invoke(this, e);

        private void Initialize()
        {
            ThrowIfDisposed();

            var catalogsList = new List<AssemblyCatalog>();
            var partDefinitionsList = new List<ComposablePartDefinition>();

            foreach (Assembly assembly in GetAssemblies())
            {
                var assemblyCatalog = new AssemblyCatalog(assembly);
                partDefinitionsList.AddRange(assemblyCatalog.Parts);
                catalogsList.Add(assemblyCatalog);
            }

            using (var atomicComposition = new AtomicComposition())
            {
                RaiseChangingEvent(partDefinitionsList, null, atomicComposition);
                foreach (AssemblyCatalog catalog in catalogsList)
                {
                    _catalog.Catalogs.Add(catalog);
                }
                atomicComposition.Complete();
            }
            RaiseChangedEvent(partDefinitionsList, null, null);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            Assembly applicationAssembly = Application.Current.GetType().Assembly;
            yield return applicationAssembly;
            AssemblyName[] referencedAssemblies = applicationAssembly.GetReferencedAssemblies();
            Assembly assemblyRef = null;
            for (int i = 0; i < referencedAssemblies.Length; ++i)
            {
                bool flag = false;
                try
                {
                    assemblyRef = Assembly.Load(referencedAssemblies[i]);
                }
                catch
                {
                    flag = true;
                }

                if (!flag)
                {
                    yield return assemblyRef;
                }
            }
        }

        private void RaiseChangingEvent(
            IEnumerable<ComposablePartDefinition> addedDefinitions,
            IEnumerable<ComposablePartDefinition> removedDefinitions,
            AtomicComposition atomicComposition)
        {
            var added = addedDefinitions ?? Enumerable.Empty<ComposablePartDefinition>();
            var removed = removedDefinitions ?? Enumerable.Empty<ComposablePartDefinition>();
            OnChanging(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
        }

        private void RaiseChangedEvent(
            IEnumerable<ComposablePartDefinition> addedDefinitions,
            IEnumerable<ComposablePartDefinition> removedDefinitions,
            AtomicComposition atomicComposition)
        {
            var added = addedDefinitions ?? Enumerable.Empty<ComposablePartDefinition>();
            var removed = removedDefinitions ?? Enumerable.Empty<ComposablePartDefinition>();
            OnChanged(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
    }
}