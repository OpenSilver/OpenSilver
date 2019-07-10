
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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    /// <summary>
    /// This class is used to animate a Color property value along a set
    /// of key frames.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public class ColorAnimationUsingKeyFrames : AnimationTimeline
    {
        private ColorKeyFrameCollection _keyFrames;
        // Summary:
        //     Gets the collection of System.Windows.Media.Animation.ColorKeyFrame objects
        //     that define the animation.
        //
        // Returns:
        //     The collection of System.Windows.Media.Animation.ColorKeyFrame objects that
        //     define the animation. The default is an empty collection.
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
#endif
}
