#if WORKINPROGRESS
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Provides methods to satisfy imports on an existing part instance.
    public partial interface ICompositionService
    {
        //
        // Summary:
        //     Composes the specified part, with recomposition and validation disabled.
        //
        // Parameters:
        //   part:
        //     The part to compose.
        void SatisfyImportsOnce(ComposablePart part);
    }
}
#endif