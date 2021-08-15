﻿

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


// Source: https://github.com/Microsoft/UWPCommunityToolkit/tree/master/Microsoft.Toolkit.Uwp.UI.Controls/GridSplitter


// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal partial class GridSplitterGripper : ContentControl
    {
        // Symbol GripperBarVertical in Segoe MDL2 Assets
#if CSHTML5_NOT_SUPPORTED
        private const string GripperBarVertical = "\xE784";
#else
        private const string GripperBarVertical = "";
#endif

        // Symbol GripperBarHorizontal in Segoe MDL2 Assets
#if CSHTML5_NOT_SUPPORTED
        private const string GripperBarHorizontal = "\xE76F";
#else
        private const string GripperBarHorizontal = "";
#endif
        private const string GripperDisplayFont = "Segoe MDL2 Assets";
        private readonly TextBlock _gripperDisplay;

        public GridSplitter.GridResizeDirection ResizeDirection { get; private set; }

        internal Brush GripperForeground
        {
            set
            {
                if (_gripperDisplay == null)
                {
                    return;
                }

                _gripperDisplay.Foreground = value;
            }
        }

        internal GridSplitterGripper(GridSplitter.GridResizeDirection gridSplitterDirection)
        {
            ResizeDirection = gridSplitterDirection;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
#if CSHTML5_NOT_SUPPORTED
            IsTabStop = true;
            UseSystemFocusVisuals = true;
            IsFocusEngagementEnabled = true;
#endif
        }

        internal GridSplitterGripper(
            GridSplitter.GridResizeDirection gridSplitterDirection,
            Brush gripForeground)
            : this(gridSplitterDirection)
        {
            _gripperDisplay = new TextBlock
            {
                FontFamily = new FontFamily(GripperDisplayFont),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = gripForeground
            };

            if (gridSplitterDirection == GridSplitter.GridResizeDirection.Columns)
            {
                _gripperDisplay.Text = GripperBarVertical;
            }
            else if (gridSplitterDirection == GridSplitter.GridResizeDirection.Rows)
            {
                _gripperDisplay.Text = GripperBarHorizontal;
            }

            Content = _gripperDisplay;
        }

        internal GridSplitterGripper(UIElement content, GridSplitter.GridResizeDirection gridSplitterDirection)
            : this(gridSplitterDirection)
        {
            Content = content;
        }
    }
}
