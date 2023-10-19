
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

using System.Collections;
using System.Collections.Generic;
using CSHTML5.Internal;
using OpenSilver.Internal.Data;
using OpenSilver.Internal.Media.Animation;

namespace System.Windows.Media.Animation
{
    public abstract class AnimationTimeline : Timeline
    {
        private List<JavaScriptCallback> _velocityCallbacks;
        internal PropertyPath _targetProperty;
        internal DependencyObject _propertyContainer;
        internal DependencyProperty _propDp;
        internal bool _isInitialized = false;
        internal bool _cancelledAnimation = false;
        /// <summary>
        /// This variable is here to let us know when the animation has been cancelled before it ended and so it should not apply the final value to the C# when receiving the callback from Velocity.
        ///  An example where it is useful is with a Control with a "MouseOver" VisualState that sets the opacity of an element to 1 though a DoubleAnimation (with a duration of 0) and we move the cursor in and out of the Control very quickly.
        /// </summary>
        internal bool _isUnapplied = true;
        protected override Duration GetNaturalDurationCore()
        {
            return new TimeSpan(0, 0, 1);
        }

        internal static bool IsZeroDuration(Duration duration)
            => duration.HasTimeSpan && duration.TimeSpan == TimeSpan.Zero;

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            Initialize(parameters);

            base.IterateOnce(parameters, isLastLoop);

            Apply(parameters, isLastLoop);
        }

        // todo: find a way to not have to do this.
        private void SetInitialAnimationValue()
        {
            object initialAnimationValue = _propertyContainer.GetValue(_propDp);
            AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, initialAnimationValue);
        }

        //Note: stateContainerGroupName is useful for allowing us to stop the animations made using velocity: 
        // we can stop all the animations from a queue of animations that has a given name
        internal virtual void Apply(IterationParameters parameters, bool isLastLoop)
        {
            // Needs to be overriden
        }

        private void Initialize(IterationParameters parameters)
        {
            GetTargetInformation(parameters);
            InitializeCore();
            // This is a workaround to tell the property the effective value should be the animated value.
            SetInitialAnimationValue();
            _isInitialized = true;
        }

        private void GetTargetInformation(IterationParameters parameters)
        {
            _parameters = parameters;

            _propertyContainer = null;
            _targetProperty = null;
            _propDp = null;

            if (parameters != null && parameters.TimelineMappings.TryGetValue(this, out (DependencyObject, PropertyPath) info))
            {
                DependencyObject finalTarget = info.Item1;
                PropertyPath path = info.Item2;

                if (path.DependencyProperty is null && path.SVI.Length == 0)
                {
                    throw new InvalidOperationException();
                }

                for (int i = 0; i < path.SVI.Length - 1; i++)
                {
                    SourceValueInfo svi = path.SVI[i];
                    switch (svi.type)
                    {
                        case PropertyNodeType.Property:
                            DependencyProperty dp = DPFromName(svi.propertyName, finalTarget.GetType());
                            var value = AsDependencyObject(finalTarget.GetValue(dp));
                            if (i == 0 && value is ICloneOnAnimation<DependencyObject> cloneable && !cloneable.IsClone)
                            {
                                value = cloneable.Clone();
                                finalTarget.SetValue(dp, value);
                            }
                            finalTarget = value;
                            break;

                        case PropertyNodeType.Indexed:
                            if (finalTarget is not IList list)
                            {
                                throw new InvalidOperationException($"'{finalTarget}' must implement IList.");
                            }
                            if (!int.TryParse(svi.param, out int index))
                            {
                                throw new InvalidOperationException($"'{svi.param}' can't be converted to an integer value.");
                            }

                            finalTarget = AsDependencyObject(list[index]);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }

                _propertyContainer = finalTarget;
                _targetProperty = path;
                _propDp = path.GetFinalProperty(finalTarget);
            }

            static DependencyObject AsDependencyObject(object o) =>
                o as DependencyObject ??
                throw new InvalidOperationException($"'{o}' must be a DependencyObject.");

            static DependencyProperty DPFromName(string name, Type ownerType) =>
                DependencyProperty.FromName(name, ownerType) ??
                throw new InvalidOperationException($"No DependencyProperty named '{name}' could be found in '{ownerType}'.");
        }

        internal virtual void InitializeCore() { }

        internal override void Stop(IterationParameters parameters, bool revertToFormerValue = false)
        {
            if (_isInitialized)
            {
                _cancelledAnimation = revertToFormerValue;
                base.Stop(parameters, revertToFormerValue);
                StopAnimation();
                if (revertToFormerValue)
                {
                    UnApply();
                }
                ReleaseCallbacks();
            }
        }

        internal virtual void StopAnimation() { }

        internal void RegisterCallback(JavaScriptCallback callback)
        {
            _velocityCallbacks ??= new List<JavaScriptCallback>();
            _velocityCallbacks.Add(callback);
        }

        private void ReleaseCallbacks()
        {
            if (_velocityCallbacks != null)
            {
                foreach (JavaScriptCallback callback in _velocityCallbacks)
                {
                    callback.Dispose();
                }

                _velocityCallbacks = null;
            }
        }

        private void UnApply()
        {
            AnimationHelpers.ApplyValue(_propertyContainer, _targetProperty, DependencyProperty.UnsetValue);
        }
    }
}
