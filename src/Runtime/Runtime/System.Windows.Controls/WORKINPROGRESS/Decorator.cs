#if MIGRATION
using System.Windows.Markup;

namespace System.Windows.Controls
{
    [ContentProperty("Child")]
    [OpenSilver.NotImplemented]
    public class Decorator : FrameworkElement, IAddChild
    {
        [OpenSilver.NotImplemented]
        public void AddChild(object value)
        {
            throw new NotImplementedException();
        }

        [OpenSilver.NotImplemented]
        public void AddText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
