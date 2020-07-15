using System;
using System.Windows;
using System.Windows.Controls;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    public class ControlForWrappingLaterXamlExceptions : ContentControl
    {
        // This class will catch exceptions that happen AFTER the dynamically-loaded XAML has been added to the Visual Tree (otherwise it would result in an infinite UnhandledException loop).

        //--------------------------
        // Credits: https://social.msdn.microsoft.com/Forums/windowsserver/en-US/132170ca-59f5-43cf-9edc-1e5d7bf7ee57/how-do-i-gracefully-handle-xamlparseexceptions?forum=wpf
        //--------------------------

        public event EventHandler ExceptionOccurred;

        protected override Size MeasureOverride(Size availableSize)
        {
            try
            {
                var size = base.MeasureOverride(availableSize);

                // The following line prevents the error "Layout measurement override of element '...' should not return PositiveInfinity as its DesiredSize, even if infinity is passed in as available size":
                if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
                    size = new Size();

                return size;
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                this.Content = null;
                if (ExceptionOccurred != null)
                    ExceptionOccurred(this, new EventArgs());
            }

            // The following line prevents the error "Layout measurement override of element '...' should not return PositiveInfinity as its DesiredSize, even if infinity is passed in as available size":
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
                availableSize = new Size();

            return availableSize;
        }
    }
}
