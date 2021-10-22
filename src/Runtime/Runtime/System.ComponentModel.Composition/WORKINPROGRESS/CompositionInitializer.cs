using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    //
    // Summary:
    //     Provides static access to methods for parts to satisfy imports.
	[OpenSilver.NotImplemented]
    public static class CompositionInitializer
    {
        //
        // Summary:
        //     Fills the imports of the specified attributed part.
        //
        // Parameters:
        //   attributedPart:
        //     The attributed part to fill the imports of.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     attributedPart is null.
        //
        //   T:System.ArgumentException:
        //     attributedPart contains exports.
        //
        //   T:System.ComponentModel.Composition.ChangeRejectedException:
        //     One or more of the imports of attributedPart could not be satisfied.
        //
        //   T:System.ComponentModel.Composition.CompositionException:
        //     One or more of the imports of attributedPart caused a composition error.
		[OpenSilver.NotImplemented]
        public static void SatisfyImports(object attributedPart)
        {

        }
        //
        // Summary:
        //     Fills the imports of the specified part.
        //
        // Parameters:
        //   part:
        //     The part to fill the imports of.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     attributedPart is null.
        //
        //   T:System.ArgumentException:
        //     attributedPart contains exports.
        //
        //   T:System.ComponentModel.Composition.ChangeRejectedException:
        //     One or more of the imports of attributedPart could not be satisfied.
        //
        //   T:System.ComponentModel.Composition.CompositionException:
        //     One or more of the imports of attributedPart caused a composition error.
		[OpenSilver.NotImplemented]
        public static void SatisfyImports(ComposablePart part)
        {

        }
    }
}
