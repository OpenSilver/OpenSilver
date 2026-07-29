
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

namespace System.Windows.Controls;

/// <summary>
/// Provides a resource key for an <see cref="ItemContainerTemplate"/> object.
/// </summary>
public class ItemContainerTemplateKey : TemplateKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContainerTemplateKey"/> class.
    /// </summary>
    public ItemContainerTemplateKey()
        : base(TemplateType.TableTemplate)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContainerTemplateKey"/> class with 
    /// the specified data type.
    /// </summary>
    /// <param name="dataType">
    /// The type for which this template is designed.
    /// </param>
    public ItemContainerTemplateKey(object dataType)
        : base(TemplateType.TableTemplate, dataType)
    {
    }
}