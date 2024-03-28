
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

using System.Windows.Shapes;

namespace OpenSilver.Internal.Media;

internal interface ISvgBrush
{
    string GetBrush(Shape shape);

    void RenderBrush();

    void DestroyBrush(Shape shape);
}
