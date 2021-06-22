using CSHTML5.Internal;
using System;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace OpenSilver.Internal
{
    /// <summary>
    ///     This is the data that is passed through the DescendentsWalker
    ///     during an inheritable property change tree-walk.
    /// </summary>
    internal struct InheritablePropertyChangeInfo
    {
#region Constructors

        internal InheritablePropertyChangeInfo(
            DependencyObject rootElement,
            DependencyProperty property,
            object oldValue,
            BaseValueSourceInternal oldValueSource,
            object newValue,
            BaseValueSourceInternal newValueSource)
        {
            _rootElement = rootElement;
            _property = property;
            _oldValue = oldValue;
            _oldValueSource = oldValueSource;
            _newValue = newValue;
            _newValueSource = newValueSource;
        }

#endregion Constructors

#region Properties

        internal DependencyObject RootElement
        {
            get { return _rootElement; }
        }

        internal DependencyProperty Property
        {
            get { return _property; }
        }

        internal object OldValue
        {
            get { return _oldValue; }
        }

        internal BaseValueSourceInternal OldValueSource
        {
            get { return _oldValueSource; }
        }

        internal object NewValue
        {
            get { return _newValue; }
        }

        internal BaseValueSourceInternal NewValueSource
        {
            get { return _newValueSource; }
        }

#endregion Properties

#region Data

        private DependencyObject _rootElement;
        private DependencyProperty _property;
        private object _oldValue;
        private BaseValueSourceInternal _oldValueSource;
        private object _newValue;
        private BaseValueSourceInternal _newValueSource;

#endregion Data
    }
}
