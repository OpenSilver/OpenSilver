

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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    public enum FrameworkPropertyMetadataOptions
    {
        None = 0,
        AffectsMeasure = 0x1,
        AffectsArrange = 0x2,
        AffectsParentMeasure = 0x4,
        AffectsParentArrange = 0x8,
        AffectsRender = 0x10,
    }

    public class FrameworkPropertyMetadata : PropertyMetadata
    {
        public bool AffectsArrange { get; set; }
        public bool AffectsMeasure { get; set; }
        public bool AffectsParentArrange { get; set; }
        public bool AffectsParentMeasure { get; set; }
        public bool AffectsRender { get; set; }

        public FrameworkPropertyMetadata() :
            this(null, FrameworkPropertyMetadataOptions.None, null, null)
        {
            
        }

        public FrameworkPropertyMetadata(object defaultValue) :
            this(defaultValue, FrameworkPropertyMetadataOptions.None, null, null)
        {
            
        }

        public FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions flags) :
            this(null, flags, null, null)
        {
            
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags) :
            this(defaultValue, flags, null, null)
        {
            
        }
        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) :
            this(defaultValue, flags, propertyChangedCallback, null)
        {

        }

        public FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback) :
            this(null, flags, propertyChangedCallback, null)
        {

        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) :
            base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            this.AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
            this.AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            this.AffectsParentArrange = (flags & FrameworkPropertyMetadataOptions.AffectsParentArrange) != 0;
            this.AffectsParentMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsParentMeasure) != 0;
            this.AffectsRender = (flags & FrameworkPropertyMetadataOptions.AffectsRender) != 0;
        }
    }
#endif
}
