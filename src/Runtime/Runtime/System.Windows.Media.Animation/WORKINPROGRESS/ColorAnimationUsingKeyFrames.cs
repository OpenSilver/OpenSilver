

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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// This class is used to animate a Color property value along a set
    /// of key frames.
    /// </summary>
    [ContentProperty("KeyFrames")]
    [OpenSilver.NotImplemented]
    public partial class ColorAnimationUsingKeyFrames : AnimationTimeline
    {
        private ColorKeyFrameCollection _keyFrames;
        // Summary:
        //     Gets the collection of System.Windows.Media.Animation.ColorKeyFrame objects
        //     that define the animation.
        //
        // Returns:
        //     The collection of System.Windows.Media.Animation.ColorKeyFrame objects that
        //     define the animation. The default is an empty collection.
        [OpenSilver.NotImplemented]
        public ColorKeyFrameCollection KeyFrames
        {
            get
            {
                if (_keyFrames == null)
                {
                    _keyFrames = new ColorKeyFrameCollection();
                }
                return _keyFrames;
            }
        }


        internal override void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters.Target, out targetBeforePath, out propertyPath, parameters.IsTargetParentTheTarget);
            DependencyObject parentElement = targetBeforePath; //this will be the parent of the clonable element (if any).
            foreach (Tuple<DependencyObject, DependencyProperty, int?> element in GoThroughElementsToAccessProperty(propertyPath, targetBeforePath))
            {
                DependencyObject depObject = element.Item1;
                DependencyProperty depProp = element.Item2;
                int? index = element.Item3;
                if (depObject is ICloneOnAnimation)
                {
                    if (!((ICloneOnAnimation)depObject).IsAlreadyAClone())
                    {
                        object clone = ((ICloneOnAnimation)depObject).Clone();
                        if (index != null)
                        {
#if BRIDGE
                            parentElement.GetType().GetProperty("Item").SetValue(parentElement, clone, new object[] { index });
#else
                            //JSIL does not support SetValue(object, object, object[])
#endif
                        }
                        else
                        {
                            parentElement.SetValue(depProp, clone);
                        }
                    }
                    break;
                }
                else
                {
                    parentElement = depObject;
                }
            }

            GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath, parameters.IsTargetParentTheTarget);

            _propertyContainer = target;
            _targetProperty = propertyPath;
            _propDp = GetProperty(_propertyContainer, _targetProperty);
            _target = Storyboard.GetTarget(this);
            _targetName = Storyboard.GetTargetName(this);
        }
    }
}
