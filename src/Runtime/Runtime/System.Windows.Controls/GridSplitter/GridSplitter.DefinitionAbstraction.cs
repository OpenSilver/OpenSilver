// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// Pretends to be the base class for RowDefinition and ClassDefinition
        /// types so that objects of either type can be treated as one.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal class DefinitionAbstraction
        {
            /// <summary>
            /// Creates an instance of the DefinitionAbstraction class based on
            /// the given row or column definition.
            /// </summary>
            /// <param name="definition">
            /// RowDefinition or ColumnDefinition instance.
            /// </param>
            public DefinitionAbstraction(DependencyObject definition)
            {
                this.AsRowDefinition = definition as RowDefinition;
                if (this.AsRowDefinition == null)
                {
                    this.AsColumnDefinition = definition as ColumnDefinition;
                    Debug.Assert(this.AsColumnDefinition != null, "AsColumnDefinition should not be null!");
                }
            }

            /// <summary>
            /// Gets the stored definition cast as a row definition.
            /// </summary>
            /// <value>Null if not a RowDefinition.</value>
            public RowDefinition AsRowDefinition { get; private set; }

            /// <summary>
            /// Gets the stored definition cast as a column definition.
            /// </summary>
            /// <value>Null if not a ColumnDefinition.</value>
            public ColumnDefinition AsColumnDefinition { get; private set; }

            /// <summary>
            /// Gets the MaxHeight/MaxWidth for the row/column.
            /// </summary>
            public double MaxSize
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.MaxHeight;
                    }
                    return this.AsColumnDefinition.MaxWidth;
                }
            }

            /// <summary>
            /// Gets the MinHeight/MinWidth for the row/column.
            /// </summary>
            public double MinSize
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.MinHeight;
                    }
                    return this.AsColumnDefinition.MinWidth;
                }
            }

            /// <summary>
            /// Gets the Height/Width for the row/column.
            /// </summary>
            public GridLength Size
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.Height;
                    }
                    return this.AsColumnDefinition.Width;
                }
            }
        }
    }
}