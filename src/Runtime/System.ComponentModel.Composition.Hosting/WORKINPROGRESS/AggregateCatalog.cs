#if WORKINPROGRESS
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class AggregateCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        //
        // Summary:
        //     Occurs when the contents of the System.ComponentModel.Composition.Hosting.AggregateCatalog
        //     object have changed.
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        //
        // Summary:
        //     Occurs when the contents of the System.ComponentModel.Composition.Hosting.AggregateCatalog
        //     object is changing.
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        public ICollection<ComposablePartCatalog> Catalogs { get; private set; }

        public override IQueryable<ComposablePartDefinition> Parts 
        { 
            get { return null; } 
        }
    }
}

#endif