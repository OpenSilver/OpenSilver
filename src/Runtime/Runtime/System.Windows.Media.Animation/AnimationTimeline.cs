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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    public abstract partial class AnimationTimeline : Timeline
    {
        private List<JavaScriptCallback> _velocityCallbacks;
        internal string _targetName;
        internal PropertyPath _targetProperty;
        internal DependencyObject _propertyContainer;
        internal DependencyObject _target;
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
            
            // This is a workaround to tell the property the effective value should be the animated value.
            SetInitialAnimationValue();
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

        internal void Initialize(IterationParameters parameters)
        {
            GetTargetInformation(parameters);
            InitializeCore();
            _isInitialized = true;
        }

        internal virtual void GetTargetInformation(IterationParameters parameters)
        {

        }

        internal virtual void InitializeCore()
        {

        }

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
