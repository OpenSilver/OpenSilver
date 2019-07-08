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
    public abstract class AnimationTimeline : Timeline
    {
        internal string _targetName;
        internal PropertyPath _targetProperty;
        internal DependencyObject _propertyContainer;
        internal DependencyObject _target;
        internal IterationParameters _parameters;
        internal bool _isInitialized = false;
        internal bool _cancelledAnimation = false;
        protected override Duration GetNaturalDurationCore()
        {
            return new TimeSpan(0, 0, 1);
        }

        internal override void IterateOnce(IterationParameters parameters, bool isLastLoop)
        {
            if (!_isInitialized)
            {
                Initialize(parameters);
            }
            else
            {
                RestoreDefault();
            }
            base.IterateOnce(parameters, isLastLoop);

            // This is a workaround to tell the property the effective value should be the animated value.
            SetInitialAnimationValue();

            Apply(parameters, isLastLoop);
        }

        // todo: find a way to not have to do this.
        private void SetInitialAnimationValue()
        {
            DependencyProperty dp = (DependencyProperty)_propertyContainer.GetType().GetProperty(_targetProperty.INTERNAL_DependencyPropertyName).DeclaringType.GetField(_targetProperty.INTERNAL_DependencyPropertyName + "Property").GetValue(null);
            _targetProperty.INTERNAL_PropertySetAnimationValue(_propertyContainer, _propertyContainer.GetValue(dp));
        }

        //Note: stateContainerGroupName is useful for allowing us to stop the animations made using velocity: 
        // we can stop all the animations from a queue of animations that has a given name
        internal virtual void Apply(IterationParameters parameters, bool isLastLoop)
        {
            // Needs to be overriden
        }

        private void RestoreDefault()
        {
            _cancelledAnimation = false;
            RestoreDefaultCore();
        }

        internal virtual void RestoreDefaultCore()
        {

        }

        internal void Initialize(IterationParameters parameters)
        {
            GetTargetInformation(parameters);
            InitializeCore();
            ComputeDuration();
            _isInitialized = true;
        }

        internal virtual void GetTargetInformation(IterationParameters parameters)
        {

        }

        internal virtual void InitializeCore()
        {

        }

        internal override void Stop(FrameworkElement frameworkElement, string groupName, bool revertToFormerValue = false)
        {
            if (_isInitialized)
            {
                _cancelledAnimation = revertToFormerValue;
                base.Stop(frameworkElement, groupName, revertToFormerValue);
                StopAnimation(groupName);
                if (revertToFormerValue)
                {
                    UnApply();
                }
            }
        }

        internal virtual void StopAnimation(string groupName)
        {
            
        }

        private void UnApply()
        {
            _targetProperty.INTERNAL_PropertySetAnimationValue(_propertyContainer, INTERNAL_NoValue.NoValue);
        }
    }
}
