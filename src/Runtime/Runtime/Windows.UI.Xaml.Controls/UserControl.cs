

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
using OpenSilver.Internal.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if !FOR_DESIGN_TIME && !MIGRATION
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides the base class for defining a new control that encapsulates related
    /// existing controls and provides its own logic.
    /// </summary>
    [ContentProperty("Content")]
    public partial class UserControl : Control, INameScope
    {
        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Content == null)
                {
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return new SingleChildEnumerator(this.Content);
            }
        }

#region Constructors

        static UserControl()
        {
            // UseContentTemplate
            ControlTemplate template = new ControlTemplate();
            template._methodToInstantiateFrameworkTemplate = (owner) =>
            {
                TemplateInstance instance = new TemplateInstance();

                instance.TemplateOwner = owner;
                instance.TemplateContent = ((UserControl)owner).Content as FrameworkElement;

                return instance;
            };
            template.Seal();

            UseContentTemplate = template;
        }

        public UserControl()
        {
            IsTabStop = false; //we want to avoid stopping on this element's div when pressing tab.
        }

#endregion Constructors

        /// <summary>
        /// Gets or sets the content that is contained within a user control.
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Content dependency property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(UIElement),
                typeof(UserControl),
                new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserControl uc = (UserControl)d;

            uc.TemplateChild = null;
            uc.RemoveLogicalChild(e.OldValue);
            uc.AddLogicalChild(e.NewValue);

            if (VisualTreeHelper.GetParent(uc) != null)
            {
                uc.InvalidateMeasureInternal();
            }
        }

        /// <summary>
        /// Gets the element that should be used as the StateGroupRoot for VisualStateMangager.GoToState calls
        /// </summary>
        internal override FrameworkElement StateGroupsRoot
        {
            get
            {
                return Content as FrameworkElement;
            }
        }

#if REVAMPPOINTEREVENTS
        internal override bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
#if MIGRATION
            // In Silverlight, it appears that UserControl does not support mouse events because its Background property is ignored.
            return false;
#else
            // We only check the Background property even if BorderBrush not null + BorderThickness > 0 is a sufficient condition to enable pointer events on the borders of the control.
            // There is no way right now to differentiate the Background and BorderBrush as they are both defined on the same DOM element.
            return Background != null;
#endif
        }
#endif

        internal override FrameworkTemplate TemplateCache
        {
            get { return UseContentTemplate; }
            set { }
        }

        internal override FrameworkTemplate TemplateInternal
        {
            get { return UseContentTemplate; }
        }

        private static ControlTemplate UseContentTemplate
        {
            get;
        }

#region ---------- INameScope implementation ----------

        Dictionary<string, object> _nameScopeDictionary = new Dictionary<string,object>();

        /// <summary>
        /// Finds the UIElement with the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The object with the specified name if any; otherwise null.</returns>
        public new object FindName(string name)
        {
            if (_nameScopeDictionary.ContainsKey(name))
                return _nameScopeDictionary[name];
            else
                return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (_nameScopeDictionary.ContainsKey(name) && _nameScopeDictionary[name] != scopedElement)
                throw new ArgumentException(string.Format("Cannot register duplicate name '{0}' in this scope.", name));

            _nameScopeDictionary[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            if (!_nameScopeDictionary.ContainsKey(name))
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name));

            _nameScopeDictionary.Remove(name);
        }

#endregion
    }
}
