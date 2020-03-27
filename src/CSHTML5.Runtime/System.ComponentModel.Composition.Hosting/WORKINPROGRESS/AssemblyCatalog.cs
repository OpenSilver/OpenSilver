#if WORKINPROGRESS
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Composition.Hosting
{
    //
    // Summary:
    //     Discovers attributed parts in a managed code assembly.
    public partial class AssemblyCatalog : ComposablePartCatalog, ICompositionElement
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.Composition.Hosting.AssemblyCatalog
        //     class with the specified assembly.
        //
        // Parameters:
        //   assembly:
        //     The assembly that contains the attributed System.Type objects to add to the System.ComponentModel.Composition.Hosting.AssemblyCatalog
        //     object.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     assembly is null.-or-assembly was loaded in the reflection-only context.
        public AssemblyCatalog(Assembly assembly)
        {

        }

        //
        // Summary:
        //     Gets the assembly whose attributed types are contained in the assembly catalog.
        //
        // Returns:
        //     The assembly whose attributed System.Type objects are contained in the System.ComponentModel.Composition.Hosting.AssemblyCatalog.
        public Assembly Assembly { get; }
        //
        // Summary:
        //     Gets the part definitions that are contained in the assembly catalog.
        //
        // Returns:
        //     The System.ComponentModel.Composition.Primitives.ComposablePartDefinition objects
        //     contained in the System.ComponentModel.Composition.Hosting.AssemblyCatalog.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The System.ComponentModel.Composition.Hosting.AssemblyCatalog object has been
        //     disposed of.
        public override IQueryable<ComposablePartDefinition> Parts { get; }

        string ICompositionElement.DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        ICompositionElement ICompositionElement.Origin
        {
            get { throw new NotImplementedException(); }
        }

        //
        // Summary:
        //     Gets a string representation of the assembly catalog.
        //
        // Returns:
        //     A representation of the assembly catalog.
        public override string ToString()
        {
            return null;
        }
    }
}
#endif