

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal interface IDefinitionBase
    {
        double MinLength { get; }
        double MaxLength { get; }
        GridLength Length { get; }

        double GetUserMaxSize();
        double GetUserMinSize();
        GridUnitType GetUserSizeType();
        double GetUserSizeValue();
        void UpdateEffectiveMinSize(double newValue);
        void SetEffectiveUnitType(GridUnitType type);
        GridUnitType GetEffectiveUnitType();
        void SetEffectiveMinSize(double value);
        double GetEffectiveMinSize();
        void SetMeasureArrangeSize(double value);
        double GetMeasureArrangeSize();
        void SetSizeCache(double value);
        double GetSizeCache();
        double GetPreferredSize();
        double GetFinalOffset();
        void SetFinalOffset(double value);
    }
}
#endif