#if WORKINPROGRESS
namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Specifies when and how a part will be instantiated.
    public enum CreationPolicy
    {
        //
        // Summary:
        //     Specifies that the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     will use the most appropriate System.ComponentModel.Composition.CreationPolicy
        //     for the part given the current context. This is the default System.ComponentModel.Composition.CreationPolicy.
        //     Be default, System.ComponentModel.Composition.Hosting.CompositionContainer will
        //     use System.ComponentModel.Composition.CreationPolicy.Shared, unless the System.ComponentModel.Composition.Primitives.ComposablePart
        //     or importer requests System.ComponentModel.Composition.CreationPolicy.NonShared.
        Any = 0,
        //
        // Summary:
        //     Specifies that a single shared instance of the associated System.ComponentModel.Composition.Primitives.ComposablePart
        //     will be created by the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     and shared by all requestors.
        Shared = 1,
        //
        // Summary:
        //     Specifies that a new non-shared instance of the associated System.ComponentModel.Composition.Primitives.ComposablePart
        //     will be created by the System.ComponentModel.Composition.Hosting.CompositionContainer
        //     for every requestor.
        NonShared = 2
    }
}
#endif