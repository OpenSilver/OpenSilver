
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

using System.Xml;

namespace System.Windows;

/// <summary>
/// Represents the resource key for the <see cref="DataTemplate"/> class.
/// </summary>
public class DataTemplateKey : TemplateKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataTemplateKey"/> class.
    /// </summary>
    public DataTemplateKey()
        : base(TemplateType.DataTemplate)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTemplateKey"/> class with the specified type.
    /// </summary>
    /// <param name="dataType">
    /// The type for which this template is designed. This is either a <see cref="Type"/> (to indicate that 
    /// the <see cref="DataTemplate"/> is used to display items of the given type), or a string (to indicate 
    /// that the <see cref="DataTemplate"/> is used to display <see cref="XmlNode"/> elements with the given 
    /// tag name).
    /// </param>
    public DataTemplateKey(object dataType)
        : base(TemplateType.DataTemplate, dataType)
    {
    }
}
