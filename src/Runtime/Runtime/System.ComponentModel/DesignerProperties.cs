

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides attached properties that can be used to communicate with a designer.
    /// </summary>
    public static class DesignerProperties
    {
        /// <summary>
        /// Gets the value of the System.ComponentModel.DesignerProperties.IsInDesignMode
        /// attached property for the specified System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>
        /// The System.ComponentModel.DesignerProperties.IsInDesignMode property value
        /// for the element.
        /// </returns>
        public static bool GetIsInDesignMode(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("The element parameter cannot be null");
            return (bool)element.GetValue(IsInDesignModeProperty);
        }
        /// <summary>
        /// Sets the value of the System.ComponentModel.DesignerProperties.IsInDesignMode
        /// attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="value">The needed System.Boolean value.</param>
        public static void SetIsInDesignMode(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("The element parameter cannot be null");
            element.SetValueInternal(IsInDesignModeProperty, value);
        }
        /// <summary>
        /// Identifies the System.ComponentModel.DesignerProperties.IsInDesignMode attached
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsInDesignModeProperty =
            DependencyProperty.RegisterAttached("IsInDesignMode", typeof(bool), typeof(DesignerProperties), new PropertyMetadata(false));



        /// <summary>
        /// Gets a value that indicates whether the element is running in the context
        /// of a designer.
        /// </summary>
        public static bool IsInDesignTool { get; set; }

        ///// <summary>
        ///// Gets or sets a value that indicates whether all the metadata associated with
        ///// an assembly or just the xmlns namespace definitions should be refreshed in
        ///// the designer when an assembly is recompiled.
        ///// </summary>
        //public static bool RefreshOnlyXmlnsDefinitionsOnAssemblyReplace { get; set; }

    }
}