#if WORKINPROGRESS
using System;
using System.Linq;

namespace System.ComponentModel.Composition.Primitives
{
    public abstract partial class ComposablePartCatalog : IDisposable
    {
        public abstract IQueryable<ComposablePartDefinition> Parts { get; }
        public void Dispose()
        {

        }
    }
}

#endif