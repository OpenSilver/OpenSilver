
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not implemented yet
    public partial class HierarchicalDataTemplate : DataTemplate
    {
        /// <summary>
        /// Gets or sets the binding that is used to generate content for the next sublevel in the data hierarchy.
        /// </summary>
        public Binding ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets the System.Windows.DataTemplate to apply to the System.Windows.Controls.ItemsControl.ItemTemplate property 
        /// on a generated System.Windows.Controls.HeaderedItemsControl, such as a System.Windows.Controls.TreeViewItem, to indicate
        /// how to display items in the next sublevel in the data hierarchy.
        /// </summary>
        public DataTemplate ItemTemplate { set; get; }
    }
    #endregion
#endif
}
