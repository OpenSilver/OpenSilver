
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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows
{
    public partial class FrameworkElement
    {
        // Style/Template state (internals maintained by Style, per-instance data in StyleDataField)
        private Style _styleCache;

        // ThemeStyle used only when a ThemeStyleKey is specified (per-instance data in ThemeStyleDataField)
        private Style _themeStyleCache;

        private Style _implicitStyleCache;
        private InheritanceBehavior? _resourceLookupMode;

        /// <summary>
        /// Indicates the current mode of lookup for resources.
        /// </summary>
        public InheritanceBehavior ResourceLookupMode
        {
            get
            {
                if (_resourceLookupMode.HasValue)
                {
                    return _resourceLookupMode.Value;
                }

                Application app = Application.Current;
                if (app != null)
                {
                    return app.Host.Settings.DefaultResourceLookupMode;
                }

                return InheritanceBehavior.Default;
            }
            set
            {
                if (value != InheritanceBehavior.Default
                    && value != InheritanceBehavior.SkipToAppNow
                    && value != InheritanceBehavior.SkipAllNow)
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(InheritanceBehavior));
                }

                _resourceLookupMode = value;
            }
        }

        /// <summary>
        /// Identifies the <see cref="Style"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(
                nameof(Style),
                typeof(Style),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(null, OnStyleChanged));

        /// <summary>
        /// Gets or sets an instance Style that is applied for this object during rendering.
        /// </summary>
        public Style Style
        {
            get { return _styleCache; }
            set { SetValue(StyleProperty, value); }
        }

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            Style oldStyle = fe.HasLocalStyle ? (Style)e.OldValue : fe.ImplicitStyle;
            Style newStyle = (Style)e.NewValue;

            fe.HasLocalStyle = fe.ReadLocalValueInternal(StyleProperty) != DependencyProperty.UnsetValue;

            StyleHelper.UpdateStyleCache(fe, oldStyle, newStyle, ref fe._styleCache);
        }

        /// <summary>
        /// Identifies the <see cref="DefaultStyleKey"/> dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty DefaultStyleKeyProperty =
            DependencyProperty.Register(
                nameof(DefaultStyleKey),
                typeof(object),
                typeof(FrameworkElement),
                new PropertyMetadata(null, OnThemeStyleKeyChanged));

        /// <summary>
        /// Gets or sets the key that references the default style for the control.
        /// </summary>
        protected object DefaultStyleKey
        {
            get { return GetValue(DefaultStyleKeyProperty); }
            set { SetValue(DefaultStyleKeyProperty, value); }
        }

        private static void OnThemeStyleKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Re-evaluate ThemeStyle because it is
            // a factor of the ThemeStyleKey property
            ((FrameworkElement)d).UpdateThemeStyleProperty();
        }

        /// <summary>
        ///     This method causes the ThemeStyleProperty to be re-evaluated
        /// </summary>
        private void UpdateThemeStyleProperty()
        {
            if (IsThemeStyleUpdateInProgress == false)
            {
                IsThemeStyleUpdateInProgress = true;
                try
                {
                    StyleHelper.GetThemeStyle(this);
                }
                finally
                {
                    IsThemeStyleUpdateInProgress = false;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cyclic reference found while evaluating the ThemeStyle property on element '{0}'.", this));
            }
        }

        // Invoked when the ThemeStyle property is changed
        internal static void OnThemeStyleChanged(DependencyObject d, Style oldStyle, Style newStyle)
        {
            FrameworkElement fe = (FrameworkElement)d;
            StyleHelper.UpdateThemeStyleCache(fe, oldStyle, newStyle, ref fe._themeStyleCache);
        }

        private void UpdateStyleProperty()
        {
            if (!HasStyleInvalidated)
            {
                if (IsStyleUpdateInProgress == false)
                {
                    IsStyleUpdateInProgress = true;
                    try
                    {
                        InvalidateStyleProperty();
                        HasStyleInvalidated = true;
                    }
                    finally
                    {
                        IsStyleUpdateInProgress = false;
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cyclic reference found while evaluating the Style property on element '{0}'.", this));
                }
            }
        }

        private void InvalidateStyleProperty()
        {
            // Try to find an implicit style
            Style implicitStyle = FindImplicitStyleResource(this, GetType()) as Style;
            Style oldStyle = ImplicitStyle;

            // Set the flag associated with the StyleProperty
            HasImplicitStyleFromResources = implicitStyle != null;

            if (oldStyle != implicitStyle)
            {
                StyleHelper.UpdateImplicitStyleCache(this, oldStyle, implicitStyle, ref _implicitStyleCache);
            }
        }

        internal static object FindImplicitStyleResource(FrameworkElement fe, Type resourceKey)
        {
            if (fe.ShouldLookupImplicitStyles)
            {
                // For non-controls the implicit StyleResource lookup must stop at
                // the templated parent.
                DependencyObject boundaryElement = null;
                if (!(fe is Control) || fe is TextBlock)
                {
                    boundaryElement = fe.TemplatedParent;
                }

                object implicitStyle;
                // First, try to find an implicit style in parents' resources.
                FrameworkElement f = fe;
                InheritanceBehavior inheritanceBehavior = InheritanceBehavior.Default;
                while (f != null)
                {
                    inheritanceBehavior = f.ResourceLookupMode;
                    if (inheritanceBehavior != InheritanceBehavior.Default)
                    {
                        break;
                    }

                    if (f.HasResources && f.Resources.HasImplicitStyles)
                    {
                        implicitStyle = f.Resources[resourceKey];
                        if (implicitStyle != null)
                        {
                            return implicitStyle;
                        }
                    }

                    f = (f.Parent ?? VisualTreeHelper.GetParent(f)) as FrameworkElement;
                    if (boundaryElement != null && f == boundaryElement)
                    {
                        return null;
                    }
                }

                if ((inheritanceBehavior == InheritanceBehavior.Default ||
                     inheritanceBehavior == InheritanceBehavior.SkipToAppNow)
                    && boundaryElement == null)
                {
                    // Then we try to find the resource in the App's Resources
                    // if we can't find it in the parents.
                    Application app = Application.Current;
                    if (app != null)
                    {
                        implicitStyle = app.FindImplicitResourceInternal(resourceKey);
                        if (implicitStyle != null)
                        {
                            return implicitStyle;
                        }
                    }
                }                
            }

            return null;
        }

        // Cache the ThemeStyle for the current instance if there is a DefaultStyleKey specified for it
        internal Style ThemeStyle
        {
            get { return _themeStyleCache; }
        }

        internal Style ImplicitStyle
        {
            get { return _implicitStyleCache; }
        }

        // Indicates if the StyleProperty has been invalidated during a tree walk
        internal bool HasStyleInvalidated
        {
            get { return ReadInternalFlag(InternalFlags.HasStyleInvalidated); }
            set { WriteInternalFlag(InternalFlags.HasStyleInvalidated, value); }
        }

        // Indicates if the Style is being re-evaluated
        internal bool IsStyleUpdateInProgress
        {
            get { return ReadInternalFlag(InternalFlags.IsStyleUpdateInProgress); }
            set { WriteInternalFlag(InternalFlags.IsStyleUpdateInProgress, value); }
        }

        // Indicates if there are any implicit styles in the ancestry
        internal bool ShouldLookupImplicitStyles
        {
            get { return ReadInternalFlag(InternalFlags.ShouldLookupImplicitStyles); }
            set { WriteInternalFlag(InternalFlags.ShouldLookupImplicitStyles, value); }
        }

        // Note: this is used to be able to tell whether the style applied on 
        // the FrameworkElement is an ImplicitStyle, which means that it must 
        // be removed from the element when it is detached from the visual tree.

        // Indicates if this instance has an implicit style
        internal bool HasImplicitStyleFromResources
        {
            get { return ReadInternalFlag(InternalFlags.HasImplicitStyleFromResources); }
            set { WriteInternalFlag(InternalFlags.HasImplicitStyleFromResources, value); }
        }

        // Indicates if the ThemeStyle is being re-evaluated
        internal bool IsThemeStyleUpdateInProgress
        {
            get { return ReadInternalFlag(InternalFlags.IsThemeStyleUpdateInProgress); }
            set { WriteInternalFlag(InternalFlags.IsThemeStyleUpdateInProgress, value); }
        }

        // Indicates that the ThemeStyleProperty full fetch has been
        // performed atleast once on this node
        internal bool HasThemeStyleEverBeenFetched
        {
            get { return ReadInternalFlag(InternalFlags.HasThemeStyleEverBeenFetched); }
            set { WriteInternalFlag(InternalFlags.HasThemeStyleEverBeenFetched, value); }
        }

        // Indicates that the StyleProperty has been set locally on this element
        internal bool HasLocalStyle
        {
            get { return ReadInternalFlag(InternalFlags.HasLocalStyle); }
            set { WriteInternalFlag(InternalFlags.HasLocalStyle, value); }
        }
    }
}
