
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
using System.Windows.Threading;
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
    /// Animates the value of an Object property along a set of KeyFrames over a
    /// specified Duration.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public sealed class ObjectAnimationUsingKeyFrames : AnimationTimeline
    {
        Dictionary<ObjectKeyFrame, DispatcherTimer> _keyFramesToObjectTimers = new Dictionary<ObjectKeyFrame,DispatcherTimer>();
        ///// <summary>
        ///// Initializes a new instance of the ObjectAnimationUsingKeyFrames class.
        ///// </summary>
        //public ObjectAnimationUsingKeyFrames();

        //// Returns:
        ////     True if the animation can be used for a dependent animation case. False if
        ////     the animation cannot be used for a dependent animation case.
        ///// <summary>
        ///// Gets or sets a value that declares whether animated properties that are considered
        ///// dependent animations should be permitted to use this animation declaration.
        ///// </summary>
        //public bool EnableDependentAnimation { get; set; }
        ///// <summary>
        ///// Identifies the EnableDependentAnimation dependency property.
        ///// </summary>
        //public static DependencyProperty EnableDependentAnimationProperty { get; }

        private ObjectKeyFrameCollection _keyFrames = new ObjectKeyFrameCollection();
        // Returns:
        //     The collection of ObjectKeyFrame objects that define the animation. The default
        //     is an empty collection.
        /// <summary>
        /// Gets the collection of ObjectKeyFrame objects that define the animation.
        /// </summary>
        public ObjectKeyFrameCollection KeyFrames { get { return _keyFrames; } }

        internal override void Apply(IterationParameters parameters, bool isLastLoop) //Note: visualStateGroupName is useless here. Its point is to allow the definition of queues in velocity so that we can stop the animations before they are finished
        {
            DependencyObject target;
            PropertyPath propertyPath;
            DependencyObject targetBeforePath;
            GetPropertyPathAndTargetBeforePath(parameters.Target, out targetBeforePath, out propertyPath);
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
                            parentElement.GetType().GetProperty("Item").SetValue(parentElement, clone, new object[]{ index });
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


            GetTargetElementAndPropertyInfo(parameters.Target, out target, out propertyPath);
            //DependencyObject lastElementBeforeProperty = propertyPath.INTERNAL_AccessPropertyContainer(target);
            DependencyProperty dp = GetProperty(target, propertyPath);
            _expectedAmountOfKeyFrameEnds = KeyFrames.Count;
            foreach (ObjectKeyFrame keyFrame in KeyFrames)
            {
                if (keyFrame.KeyTime.TimeSpan.Ticks == 0)
                {
                    ApplyKeyFrame(target, parameters, propertyPath, dp.PropertyType, keyFrame, null);
                }
                else
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = keyFrame.KeyTime.TimeSpan;
                    timer.Tick += (sender, args) =>
                    {
                        ApplyKeyFrame(target, parameters, propertyPath, dp.PropertyType, keyFrame, timer);
                    };
                    _keyFramesToObjectTimers.Add(keyFrame, timer);
                    timer.Start();
                }
            }
        }


        private void ApplyKeyFrame(DependencyObject target, IterationParameters parameters, PropertyPath propertyPath, Type propertyType, ObjectKeyFrame keyFrame, DispatcherTimer timer)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            _keyFramesToObjectTimers.Remove(keyFrame);
            object value = keyFrame.Value;
            if (value is string && propertyType != typeof(string))
            {
                if (propertyType.IsEnum)
                {
                    value = Enum.Parse(propertyType, (string)value);
                }
                else
                {
                    //we convert the value from the given string:
                    value = DotNetForHtml5.Core.TypeFromStringConverters.ConvertFromInvariantString(propertyType, (string)value);
                }
            }

            var castedValue = DynamicCast(value, propertyType); //Note: we put this line here because the Xaml could use a Color gotten from a StaticResource (which was therefore not converted to a SolidColorbrush by the compiler in the .g.cs file) and led to a wrong type set in a property (Color value in a property of type Brush).

            if (parameters.IsVisualStateChange)
            {
                propertyPath.INTERNAL_PropertySetVisualState(target, castedValue);
            }
            else
            {
                propertyPath.INTERNAL_PropertySetLocalValue(target, castedValue);
            }

            CheckTimeLineEndAndRaiseCompletedEvent(parameters);

            //----------------------------------------
            //todo:clone required ?
            //----------------------------------------
        }



        internal override void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false) //frameworkElement is for the animations requiring the use of GetCssEquivalent
        {
            base.Stop(frameworkElement, groupName, revertToFormerValue);


            foreach (ObjectKeyFrame keyFrame in _keyFramesToObjectTimers.Keys)
            {
                _keyFramesToObjectTimers[keyFrame].Stop();
            }
            _keyFramesToObjectTimers.Clear();
        }


        private int _expectedAmountOfKeyFrameEnds = 0;
        object thisLock = new object();
        internal void CheckTimeLineEndAndRaiseCompletedEvent(IterationParameters parameters)
        {
            bool raiseEvent = false;
            lock (thisLock)
            {
                --_expectedAmountOfKeyFrameEnds;
                if (_expectedAmountOfKeyFrameEnds <= 0)
                {
                    raiseEvent = true;
                }
            }
            if (raiseEvent)
            {
                OnIterationCompleted(parameters);
            }
        }


        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            base.IterateOnce(parameters, isLastLoop);
            Apply(parameters, isLastLoop);
        }
    }
}