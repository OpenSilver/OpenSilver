
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
#if MIGRATION
using System.Windows.Controls;
#else 
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Animates the value of a Double property along a set of key frames.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public sealed class DoubleAnimationUsingKeyFrames : DoubleAnimation
    {
        ///// <summary>
        ///// Initializes a new instance of the DoubleAnimationUsingKeyFrames class.
        ///// </summary>
        //public DoubleAnimationUsingKeyFrames();

        ///// <summary>
        ///// Gets or sets a value that declares whether animated properties that are considered
        ///// dependent animations should be permitted to use this animation declaration.
        ///// </summary>
        //public bool EnableDependentAnimation
        //{
        //    get { return (bool)GetValue(EnableDependentAnimationProperty); }
        //    set { SetValue(EnableDependentAnimationProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the EnableDependentAnimation dependency property.
        ///// </summary>
        //public static readonly DependencyProperty EnableDependentAnimationProperty =
        //    DependencyProperty.Register("EnableDependentAnimation", typeof(bool), typeof(DoubleAnimationUsingKeyFrames), new PropertyMetadata(true));



#if WORKINPROGRESS
        private DoubleKeyFrameCollection _keyFrames = new DoubleKeyFrameCollection();
        //     The collection of DoubleKeyFrame objects that define the animation. The default
        //     is an empty collection.
        /// <summary>
        /// Gets the collection of DoubleKeyFrame objects that define the animation.
        /// </summary>
        public DoubleKeyFrameCollection KeyFrames
        {
            get { return _keyFrames; }
            set { _keyFrames = value; }
        }
#endif

        //internal override void Apply(FrameworkElement frameworkElement, bool useTransitions)
        //{
        //    DependencyObject target;
        //    PropertyPath propertyPath;
        //    Control frameworkElementAsControl = frameworkElement as Control;
        //    if (frameworkElementAsControl != null)
        //    {
        //        GetTargetElementAndPropertyInfo(frameworkElement, out target, out propertyPath);
        //        //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
        //        Type lastElementType = target.GetType();
        //        PropertyInfo propertyInfo = lastElementType.GetProperty(propertyPath.INTERNAL_DependencyPropertyName);
        //        object value = null;
        //        KeyTime currentKeyTime = new KeyTime();
        //        foreach (DoubleKeyFrame keyFrame in KeyFrames) //todo: replace what is inside of this foreach with a code that "starts" the keyframes (uses DispatcherTimer to apply the values at the right times)
        //        {
        //            if (keyFrame.KeyTime.TimeSpan >= currentKeyTime.TimeSpan)
        //            {
        //                value = keyFrame.Value;
        //                currentKeyTime = keyFrame.KeyTime;
        //            }
        //        }
        //        if (value is string && propertyInfo.PropertyType != typeof(string))
        //        {
        //            if (propertyInfo.PropertyType.IsEnum)
        //            {
        //                value = Enum.Parse(propertyInfo.PropertyType, (string)value);
        //            }
        //            else
        //            {
        //                //we convert the value from the given string:
        //                value = DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(propertyInfo.PropertyType, (string)value);
        //            }
        //        }

        //        propertyPath.INTERNAL_PropertySetVisualState(target, value);
        //        //propertyInfo.SetValue(target, value);
        //    }
        //}



    }
}
