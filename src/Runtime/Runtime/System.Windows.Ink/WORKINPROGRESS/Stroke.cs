#if MIGRATION
using System.ComponentModel;
using System.Windows.Input;

//https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Ink/Stroke.cs
namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a single ink stroke.
    /// </summary>
    /// <remarks>A Stroke is the data object that is collected from a pointing device, such as a tablet pen or a mouse.
    /// The Stroke can be created and manipulated programmatically, and can be represented visually on an ink-enabled element,
    /// such as the InkCanvas. A Stroke contains information about both its position and appearance.
    /// The StylusPoints property is a collection of StylusPoint objects that specifies the position of the Stroke.
    /// The DrawingAttributes property specifies a stroke's appearance.</remarks>
    [OpenSilver.NotImplemented]
    public class Stroke : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// StylusPoints
        /// </summary>
        [OpenSilver.NotImplemented]
        public StylusPointCollection StylusPoints
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif
