using System;
using System.Collections.ObjectModel;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
    public class StylusPointCollection : Collection<StylusPoint>
    {
        /// <summary>
        /// Adds a collection of System.Windows.Input.StylusPoint objects to the collection.
        /// </summary>
        /// <param name="stylusPoints">The collection of System.Windows.Input.StylusPoint objects to add to the collection.</param>
        [OpenSilver.NotImplemented]
        public void Add(StylusPointCollection stylusPoints)
        {
            throw new NotImplementedException();
        }
    }
}
