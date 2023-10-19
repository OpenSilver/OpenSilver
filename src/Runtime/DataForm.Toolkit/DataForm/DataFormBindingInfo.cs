//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    /// Stores information about a Binding, including the BindingExpression, BindingTarget and associated Element.
    /// </summary>
    internal class DataFormBindingInfo
    {
        /// <summary>
        /// Creates a new BindingInfo.
        /// </summary>
        public DataFormBindingInfo()
        {
        }

        /// <summary>
        /// Creates a new BindingInfo with the specified BindingExpression, BindingTarget and Element.
        /// </summary>
        /// <param name="bindingExpression">The BindingExpression.</param>
        /// <param name="bindingTarget">The BindingTarget.</param>
        /// <param name="element">The Element.</param>
        public DataFormBindingInfo(BindingExpression bindingExpression, DependencyProperty bindingTarget, FrameworkElement element)
        {
            this.BindingExpression = bindingExpression;
            this.BindingTarget = bindingTarget;
            this.Element = element;
        }

        /// <summary>
        /// Gets or sets the BindingExpression.
        /// </summary>
        public BindingExpression BindingExpression { get; set; }

        /// <summary>
        /// Gets or sets the BindingTarget.
        /// </summary>
        public DependencyProperty BindingTarget { get; set; }

        /// <summary>
        /// Gets or sets the Element.
        /// </summary>
        public FrameworkElement Element { get; set; }
    }
}
