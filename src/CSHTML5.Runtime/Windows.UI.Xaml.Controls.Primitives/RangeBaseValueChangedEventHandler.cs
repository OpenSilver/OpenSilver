

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


#if MIGRATION
// ----> See the class "System.Windows.RoutedPropertyChangedEventHandler"
#else
namespace Windows.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents the method that will handle a ValueChanged event.
    /// </summary>
    public delegate void RangeBaseValueChangedEventHandler(object sender, RangeBaseValueChangedEventArgs e);
}
#endif