// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a <see cref="T:System.Windows.Controls.DataGrid" /> column that hosts template-specified 
    /// content in its cells.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridTemplateColumn : DataGridColumn
    {
        #region Constants

        #endregion Constants

        #region Data

        private DataTemplate _cellTemplate;
        private DataTemplate _cellEditingTemplate;

        #endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.DataGridTemplateColumn" /> class. 
        /// </summary>
        public DataGridTemplateColumn()
        {
        }

        #region Dependency Properties
        // 











































































        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the template that is used to display the contents of a cell that is in editing mode.
        /// </summary>
        public DataTemplate CellEditingTemplate
        {
            get
            {
                return this._cellEditingTemplate;
            }
            set
            {
                if (this._cellEditingTemplate != value)
                {
                    this.RemoveEditingElement();
                    this._cellEditingTemplate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the template that is used to display the contents of a cell that is not in editing mode. 
        /// </summary>
        public DataTemplate CellTemplate
        {
            get
            {
                return this._cellTemplate;
            }
            set
            {
                if (this._cellTemplate != value)
                {
                    if (this._cellEditingTemplate == null)
                    {
                        this.RemoveEditingElement();
                    }
                    this._cellTemplate = value;
                }
            }
        }

        internal bool HasDistinctTemplates
        {
            get
            {
                return this.CellTemplate != this.CellEditingTemplate;
            }
        }

        #endregion Properties

        #region Methods

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            editingElement = GenerateEditingElement(null, null);
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:System.Windows.Controls.DataGridTemplateColumn.CellEditingTemplate" /> that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <returns>A new editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <exception cref="T:System.TypeInitializationException">
        /// The <see cref="P:System.Windows.Controls.DataGridTemplateColumn.CellEditingTemplate" /> is null.
        /// </exception>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            if (this.CellEditingTemplate != null)
            {
                return this.CellEditingTemplate.LoadContent() as FrameworkElement;
            }
            if (this.CellTemplate != null)
            {
                return this.CellTemplate.LoadContent() as FrameworkElement;
            }
            if (DesignerProperties.IsInDesignTool)
            {
                return null;
            }
            else
            {
                throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
            }
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:System.Windows.Controls.DataGridTemplateColumn.CellTemplate" /> that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.
        /// </summary>
        /// <returns>A new, read-only element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding" /> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <exception cref="T:System.TypeInitializationException">
        /// The <see cref="P:System.Windows.Controls.DataGridTemplateColumn.CellTemplate" /> is null.
        /// </exception>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (this.CellTemplate != null)
            {
                return this.CellTemplate.LoadContent() as FrameworkElement;
            }
            if (this.CellEditingTemplate != null)
            {
                return this.CellEditingTemplate.LoadContent() as FrameworkElement;
            }
            if (DesignerProperties.IsInDesignTool)
            {
                return null;
            }
            else
            {
                throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
            }
        }

        /// <summary>
        /// Called when a cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="editingEventArgs">Information about the user gesture that is causing a cell to enter editing mode.</param>
        /// <returns>null in all cases.</returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return null;
        }

        #endregion Methods
    }
}
