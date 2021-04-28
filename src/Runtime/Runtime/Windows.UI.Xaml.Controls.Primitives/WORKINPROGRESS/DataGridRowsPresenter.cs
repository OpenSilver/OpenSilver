

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


#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Used within the template of a <see cref="T:System.Windows.Controls.DataGrid" /> to specify the location in the control's visual tree where the rows are to be added.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class DataGridRowsPresenter : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Console.WriteLine($"TODO {this} MeasureOverride");
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            Console.WriteLine($"TODO {this} ArrangeOverride");
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
#endif