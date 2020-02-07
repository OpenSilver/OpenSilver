using System.IO;
using System.Reflection;
using System.Security;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public sealed class AssemblyPart : DependencyObject
    {
        //
        // Summary:
        //     Identifies the System.Windows.AssemblyPart.Source dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.AssemblyPart.Source dependency property.
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(AssemblyPart), null);

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.AssemblyPart class.
        public AssemblyPart()
        {

        }

        //
        // Summary:
        //     Gets the System.Uri that identifies an assembly as an assembly part.
        //
        // Returns:
        //     A System.String that is the assembly, which is identified as an assembly part.
        public string Source
        {
            get { return (string)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        //
        // Summary:
        //     Converts a System.IO.Stream to an System.Reflection.Assembly that is subsequently
        //     loaded into the current application domain.
        //
        // Parameters:
        //   assemblyStream:
        //     The System.IO.Stream to load into the current application domain.
        //
        // Returns:
        //     The System.Reflection.Assembly that is subsequently loaded into the current application
        //     domain.
        [SecuritySafeCritical]
        public Assembly Load(Stream assemblyStream)
        {
            return null;
        }
    }
}
#endif
