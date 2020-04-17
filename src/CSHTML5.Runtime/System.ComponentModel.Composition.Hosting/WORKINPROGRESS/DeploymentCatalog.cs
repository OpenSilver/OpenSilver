using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Net;

#if WORKINPROGRESS
namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Discovers attributed parts in a XAP file, and provides methods for asynchronously
    //     downloading XAP files.
    public class DeploymentCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     class using assemblies in the current XAP.
        public DeploymentCatalog()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     class using the XAP file at the specified relative URI.
        //
        // Parameters:
        //   uriRelative:
        //     The URI of the XAP file.
        public DeploymentCatalog(string uriRelative)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     class using the XAP file at the specified URI.
        //
        // Parameters:
        //   uri:
        //     The URI of the XAP file.
        public DeploymentCatalog(Uri uri)
        {

        }

        //
        // Summary:
        //     Gets all the parts contained in the catalog.
        //
        // Returns:
        //     A query enumerating all the parts contained in the catalog.
        public override IQueryable<ComposablePartDefinition> Parts { get; }
        //
        // Summary:
        //     Gets the URI for the XAP file.
        //
        // Returns:
        //     The URI for the XAP file.
        public Uri Uri { get; }

        //
        // Summary:
        //     Occurs when the contents of the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     have changed.
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        //
        // Summary:
        //     Occurs when the contents of the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     are changing.
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
        //
        // Summary:
        //     Occurs when the XAP file has finished downloading, or there has been an error.
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;
        //
        // Summary:
        //     Occurs when the download progress of the XAP file changes.
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        //
        // Summary:
        //     Cancels the XAP file download in progress.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The method is called before System.ComponentModel.Composition.Hosting.DeploymentCatalog.DownloadAsync
        //     or after the System.ComponentModel.Composition.Hosting.DeploymentCatalog.DownloadCompleted
        //     event has occurred.
        public void CancelAsync()
        {

        }
        //
        // Summary:
        //     Begins downloading the XAP file associated with the System.ComponentModel.Composition.Hosting.DeploymentCatalog.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     This method is called more than once, or after the System.ComponentModel.Composition.Hosting.DeploymentCatalog.CancelAsync
        //     method.
        public void DownloadAsync()
        {

        }
        //
        // Summary:
        //     Gets the export definitions that match the constraint expressed by the specified
        //     definition.
        //
        // Parameters:
        //   definition:
        //     The conditions of the System.ComponentModel.Composition.Primitives.ExportDefinition
        //     objects to be returned.
        //
        // Returns:
        //     A collection of System.Tuple`2 objects containing the System.ComponentModel.Composition.Primitives.ExportDefinition
        //     objects and their associated System.ComponentModel.Composition.Primitives.ComposablePartDefinition
        //     objects for objects that match the constraint specified by definition.
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return null;
        }
        //
        // Summary:
        //     Releases the unmanaged resources used by the System.ComponentModel.Composition.Hosting.DeploymentCatalog
        //     and optionally releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only unmanaged
        //     resources.
        protected override void Dispose(bool disposing)
        {

        }
        //
        // Summary:
        //     Raises the System.ComponentModel.Composition.Hosting.DeploymentCatalog.Changed
        //     event.
        //
        // Parameters:
        //   e:
        //     A System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs
        //     object that contains the event data.
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {

        }
        //
        // Summary:
        //     Raises the System.ComponentModel.Composition.Hosting.DeploymentCatalog.Changing
        //     event.
        //
        // Parameters:
        //   e:
        //     A System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs
        //     object that contains the event data.
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {

        }
        //
        // Summary:
        //     Raises the System.ComponentModel.Composition.Hosting.DeploymentCatalog.DownloadCompleted
        //     event.
        //
        // Parameters:
        //   e:
        //     A System.ComponentModel.AsyncCompletedEventArgs object that contains the event
        //     data.
        protected virtual void OnDownloadCompleted(AsyncCompletedEventArgs e)
        {

        }
        //
        // Summary:
        //     Raises the System.ComponentModel.Composition.Hosting.DeploymentCatalog.DownloadProgressChanged
        //     event.
        //
        // Parameters:
        //   e:
        //     A System.Net.DownloadProgressChangedEventArgs object that contains the event
        //     data.
        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {

        }
    }
}
#endif