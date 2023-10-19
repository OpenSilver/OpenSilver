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
        private AggregateCatalog _catalog = new AggregateCatalog();
        private Uri _uri;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/> 
        /// class using assemblies in the current XAP.
        /// </summary>
        public DeploymentCatalog()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/>
        /// class using the XAP file at the specified relative URI.
        /// </summary>
        /// <param name="uriRelative">The URI of the XAP file.</param>
		[OpenSilver.NotImplemented]
        public DeploymentCatalog(string uriRelative)
        {
            this._uri = new Uri(uriRelative, UriKind.Relative);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentCatalog"/>
        /// class using the XAP file at the specified URI.
        /// </summary>
        /// <param name="uri">The URI of the XAP file.</param>
		[OpenSilver.NotImplemented]
        public DeploymentCatalog(Uri uri)
        {
            this._uri = uri;
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
                this.ThrowIfDisposed();
                return this._catalog.Parts;
            }
        }

        /// <summary>
        /// Gets the URI for the XAP file.
        /// </summary>
        public Uri Uri
        {
            get
            {
                this.ThrowIfDisposed();
                return this._uri;
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
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted
        {
            add
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            remove
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Occurs when the download progress of the XAP file changes.
        /// </summary>
		[OpenSilver.NotImplemented]
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
        {
            add
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            remove
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Cancels the XAP file download in progress.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The method is called before <see cref="DeploymentCatalog.DownloadAsync"/>
        /// or after the <see cref="DeploymentCatalog.DownloadCompleted"/> event has occurred.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void CancelAsync()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Begins downloading the XAP file associated with the <see cref="DeploymentCatalog"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This method is called more than once, or after the <see cref="DeploymentCatalog.CancelAsync"/>
        /// method.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void DownloadAsync()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

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
            this.ThrowIfDisposed();
            return this._catalog.GetExports(definition);
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
                    if (!this._isDisposed)
                    {
                        AggregateCatalog catalog = null;
                        try
                        {
                            catalog = this._catalog;
                            this._catalog = null;
                            this._isDisposed = true;
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
        /// Raises the <see cref="DeploymentCatalog.Changed"/>
        /// event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="ComposablePartCatalogChangeEventArgs"/>
        /// object that contains the event data.
        /// </param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DeploymentCatalog.Changing"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="ComposablePartCatalogChangeEventArgs"/> object that contains the event data.
        /// </param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            if (this.Changing != null)
            {
                this.Changing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DeploymentCatalog.DownloadCompleted"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="AsyncCompletedEventArgs"/> object that contains the event data.
        /// </param>
		[OpenSilver.NotImplemented]
        protected virtual void OnDownloadCompleted(AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Raises the <see cref="DeploymentCatalog.DownloadProgressChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DownloadProgressChangedEventArgs"/> object that contains the event data.
        /// </param>
		[OpenSilver.NotImplemented]
        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

#region Internal Methods

        private void Initialize()
        {
            this.ThrowIfDisposed();

            List<AssemblyCatalog> catalogsList = new List<AssemblyCatalog>();
            List<ComposablePartDefinition> partDefinitionsList = new List<ComposablePartDefinition>();

            foreach (Assembly assembly in this.GetAssemblies())
            {
                AssemblyCatalog assemblyCatalog = new AssemblyCatalog(assembly);
                partDefinitionsList.AddRange(assemblyCatalog.Parts);
                catalogsList.Add(assemblyCatalog);
            }

            using (AtomicComposition atomicComposition = new AtomicComposition())
            {
                this.RaiseChangingEvent(partDefinitionsList, null, atomicComposition);
                foreach (AssemblyCatalog catalog in catalogsList)
                {
                    this._catalog.Catalogs.Add(catalog);
                }
                atomicComposition.Complete();
            }
            this.RaiseChangedEvent(partDefinitionsList, null, null);
        }

        private IEnumerable<Assembly> GetAssemblies()
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
            var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions);
            var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions);
            this.OnChanging(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
        }

        private void RaiseChangedEvent(
            IEnumerable<ComposablePartDefinition> addedDefinitions,
            IEnumerable<ComposablePartDefinition> removedDefinitions,
            AtomicComposition atomicComposition)
        {
            var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions);
            var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions);
            this.OnChanged(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
        }

        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new InvalidOperationException("Cannot access a disposed object.");
            }
        }

#endregion Internal Methods
    }
}