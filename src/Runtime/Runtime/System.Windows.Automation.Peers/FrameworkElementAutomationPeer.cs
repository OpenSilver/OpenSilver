
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
using System.Collections.Generic;
using System.Windows.Input;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="FrameworkElement" /> types (including controls) to UI automation.
    /// </summary>
    public class FrameworkElementAutomationPeer : AutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="FrameworkElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner" /> is null.
        /// </exception>
        public FrameworkElementAutomationPeer(FrameworkElement owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>
        /// Special constructor for <see cref="ItemAutomationPeer"/>.
        /// </summary>
        internal FrameworkElementAutomationPeer(object item)
        {
            Owner = item as UIElement;
        }

        /// <summary>
        /// Gets the <see cref="UIElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// </summary>
        /// <returns>
        /// The element that owns this peer class.
        /// </returns>
        public UIElement Owner { get; }

        /// <summary>
        /// Returns the <see cref="FrameworkElementAutomationPeer" /> for the specified <see cref="UIElement" />.
        /// </summary>
        /// <returns>
        /// The <see cref="FrameworkElementAutomationPeer" />, or null if the <see cref="FrameworkElementAutomationPeer" /> 
        /// was not created by the <see cref="CreatePeerForElement(UIElement)" /> method.
        /// </returns>
        /// <param name="element">
        /// The <see cref="UIElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static AutomationPeer FromElement(UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetAutomationPeer();
        }

        /// <summary>
        /// Creates a <see cref="FrameworkElementAutomationPeer" /> for the specified <see cref="UIElement" />.
        /// </summary>
        /// <returns>
        /// The <see cref="FrameworkElementAutomationPeer" /> for the specified <see cref="UIElement" />.
        /// </returns>
        /// <param name="element">
        /// The <see cref="UIElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public static AutomationPeer CreatePeerForElement(UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.CreateAutomationPeer();
        }

        /// <summary>
        /// Returns an object that supports the requested pattern, based on the patterns supported 
        /// by this <see cref="FrameworkElementAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// See Remarks.
        /// </returns>
        /// <remarks>
        /// The return value for this method in the <see cref="FrameworkElementAutomationPeer"/> implementation 
        /// returns null in all cases. Override this method to report a specific pattern.
        /// </remarks>
        public override object GetPattern(PatternInterface patternInterface) => null;

        /// <summary>
        /// Returns the accelerator key for the <see cref="UIElement" /> that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetAcceleratorKey" />.
        /// </summary>
        /// <returns>
        /// The <see cref="P:System.Windows.Automation.AutomationProperties.AcceleratorKey" /> that is returned 
        /// by <see cref="AutomationProperties.GetAcceleratorKey(DependencyObject)" />.
        /// </returns>
        protected override string GetAcceleratorKeyCore()
            => (string)Owner.GetValue(AutomationProperties.AcceleratorKeyProperty);

        /// <summary>
        /// Returns the access key for the <see cref="UIElement" /> that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetAccessKey" />.
        /// </summary>
        /// <returns>
        /// The access key for the element that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// </returns>
        protected override string GetAccessKeyCore()
            => (string)Owner.GetValue(AutomationProperties.AccessKeyProperty);

        /// <summary>
        /// Returns the string that uniquely identifies the <see cref="FrameworkElement" /> that is associated 
        /// with this <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetAutomationId" />.
        /// </summary>
        /// <returns>
        /// The automation identifier for the element associated with the <see cref="FrameworkElementAutomationPeer" />, 
        /// or <see cref="string.Empty" /> if there is no automation identifier.
        /// </returns>
        protected override string GetAutomationIdCore()
            => (string)Owner.GetValue(AutomationProperties.AutomationIdProperty);

        /// <summary>
        /// Returns the string that describes the functionality of the <see cref="FrameworkElement" /> 
        /// that is associated with this <see cref="FrameworkElementAutomationPeer" />. This method is 
        /// called by <see cref="AutomationPeer.GetHelpText" />.
        /// </summary>
        /// <returns>
        /// The help text, or <see cref="string.Empty" /> if there is no help text.
        /// </returns>
        protected override string GetHelpTextCore()
            => (string)Owner.GetValue(AutomationProperties.HelpTextProperty);

        /// <summary>
        /// Returns a string that communicates the visual status of the <see cref="UIElement" /> that 
        /// is associated with this <see cref="FrameworkElementAutomationPeer" />. This method is called 
        /// by <see cref="AutomationPeer.GetItemStatus" />.
        /// </summary>
        /// <returns>
        /// The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.ItemStatus" /> 
        /// that is returned by <see cref="AutomationProperties.GetItemStatus(DependencyObject)" />.
        /// </returns>
        protected override string GetItemStatusCore()
            => (string)Owner.GetValue(AutomationProperties.ItemStatusProperty);

        /// <summary>
        /// Returns a human-readable string that contains the item type that the <see cref="UIElement" /> 
        /// for this <see cref="FrameworkElementAutomationPeer" /> represents. This method is called by 
        /// <see cref="AutomationPeer.GetItemType" />.
        /// </summary>
        /// <returns>
        /// The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.ItemType" /> 
        /// that is returned by <see cref="AutomationProperties.GetItemType(DependencyObject)" />.
        /// </returns>
        protected override string GetItemTypeCore()
            => (string)Owner.GetValue(AutomationProperties.ItemTypeProperty);

        /// <summary>
        /// Returns the <see cref="AutomationPeer" /> for the <see cref="UIElement" /> that targets the 
        /// <see cref="FrameworkElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />. 
        /// This method is called by <see cref="AutomationPeer.GetLabeledBy" />.
        /// </summary>
        /// <returns>
        /// The <see cref="AutomationPeer" /> for the element that is targeted by the <see cref="UIElement" />.
        /// </returns>
        protected override AutomationPeer GetLabeledByCore()
            => ((UIElement)Owner.GetValue(AutomationProperties.LabeledByProperty))?.CreateAutomationPeer();

        /// <summary>
        /// Returns the UI Automation Name from the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetName" />.
        /// </summary>
        /// <returns>
        /// The UI Automation Name from the element that is associated with this automation peer.
        /// </returns>
        protected override string GetNameCore()
            => (string)Owner.GetValue(AutomationProperties.NameProperty);

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> is required to be completed on a form. 
        /// This method is called by <see cref="AutomationPeer.IsRequiredForForm" />.
        /// </summary>
        /// <returns>
        /// The value that is returned by <see cref="AutomationProperties.GetIsRequiredForForm(DependencyObject)" />, 
        /// if the value is set; otherwise, false.
        /// </returns>
        protected override bool IsRequiredForFormCore()
            => (bool)Owner.GetValue(AutomationProperties.IsRequiredForFormProperty);

        /// <summary>
        /// Returns the control type for the <see cref="UIElement" /> that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Custom;

        /// <summary>
        /// Returns the collection of child elements of the <see cref="UIElement" /> that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by <see cref="AutomationPeer.GetChildren" />.
        /// </summary>
        /// <returns>
        /// A list of child <see cref="AutomationPeer" /> elements.
        /// </returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            bool iterate(DependencyObject parent, Func<AutomationPeer, bool> callback)
            {
                bool done = false;

                if (parent != null)
                {
                    AutomationPeer peer = null;
                    int count = VisualTreeHelper.GetChildrenCount(parent);
                    for (int i = 0; i < count && !done; i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                        if (child is UIElement uie && (peer = CreatePeerForElement(uie)) != null)
                        {
                            done = callback(peer);
                        }
                        else
                        {
                            done = iterate(child, callback);
                        }
                    }
                }

                return done;
            }

            List<AutomationPeer> children = null;

            iterate(Owner,
                delegate (AutomationPeer peer)
                {
                    if (children == null)
                    {
                        children = new List<AutomationPeer>();
                    }

                    children.Add(peer);
                    return false;
                });

            return children;
        }

        /// <summary>
        /// Returns the name of the <see cref="UIElement" /> that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this <see cref="FrameworkElementAutomationPeer" />.
        /// See "Notes for Inheritors".
        /// </returns>
        /// <remarks>
        /// <b>Notes to Inheritors</b>
        /// 
        /// The return value for this method in the <see cref="FrameworkElementAutomationPeer"/> implementation 
        /// comes from an internal property store, and it returns an empty string. Specific subclasses generally 
        /// override  this method again and return the class name of the owner type for the automation peer. 
        /// There are two general patterns: return a string that is coded into the implementation, for example 
        /// "Button"; call Owner.GetType().Name. The latter approach is appropriate if your peer is otherwise 
        /// suitable for use on subclasses of its primary owner control type. In this case the dynamic value for 
        /// ClassName saves the control subclasser from having to re-implement that aspect of the automation peer.
        /// </remarks>
        protected override string GetClassNameCore() => string.Empty;

        /// <summary>
        /// Returns the <see cref="Rect" /> that represents the bounding rectangle of the <see cref="UIElement" /> 
        /// that is associated with this <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.GetBoundingRectangle" />.
        /// </summary>
        /// <returns>
        /// The <see cref="Rect" /> that contains the coordinates of the element.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override Rect GetBoundingRectangleCore() => default;

        /// <summary>
        /// Returns a <see cref="Point" /> that represents the clickable space that is on the 
        /// <see cref="UIElement" /> that is associated with this <see cref="FrameworkElementAutomationPeer" />. 
        /// This method is called by <see cref="AutomationPeer.GetClickablePoint" />.
        /// </summary>
        /// <returns>
        /// The <see cref="Point" /> on the element that allows a click.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override Point GetClickablePointCore() => default;

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> is an element that contains data that 
        /// is presented to the user. This method is called by <see cref="AutomationPeer.IsContentElement" />.
        /// </summary>
        /// <returns>
        /// true if the element contains data for the user to read; otherwise, false.
        /// </returns>
        protected override bool IsContentElementCore() => true;

        /// <summary>
        /// Returns a value that indicates whether the object that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> is understood by the end user as 
        /// interactive. Optionally, the user might understand the object as contributing 
        /// to the logical structure of the control in the GUI. This method is called by 
        /// <see cref="AutomationPeer.IsControlElement" />.
        /// </summary>
        /// <returns>
        /// true if the object is interactive; otherwise, false.
        /// </returns>
        protected override bool IsControlElementCore() => true;

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> contains protected content. This 
        /// method is called by <see cref="AutomationPeer.IsPassword" />.
        /// </summary>
        /// <returns>
        /// true if the element contains sensitive content; otherwise, false.
        /// </returns>
        protected override bool IsPasswordCore() => false;

        /// <summary>
        /// Returns a value that indicates whether the <see cref="UIElement" /> that is associated 
        /// with this <see cref="FrameworkElementAutomationPeer" /> is enabled. This method is 
        /// called by <see cref="AutomationPeer.IsEnabled" />.
        /// </summary>
        /// <returns>
        /// true if the element is enabled; otherwise, false.
        /// </returns>
        protected override bool IsEnabledCore() => ((FrameworkElement)Owner).IsEnabled;

        /// <summary>
        /// Returns a localized human-readable string that identifies a control type. The control type 
        /// is for the owner type that is associated with this <see cref="FrameworkElementAutomationPeer" />. 
        /// This method is called by <see cref="AutomationPeer.GetLocalizedControlType" />.
        /// </summary>
        /// <returns>
        /// A localized string that contains the type name of the owner control.
        /// </returns>
        protected override string GetLocalizedControlTypeCore()
        {
            string controlType;

            AutomationControlType type = GetAutomationControlTypeCore();

            switch (type)
            {
                case AutomationControlType.Button: controlType = "button"; break;
                case AutomationControlType.Calendar: controlType = "calendar"; break;
                case AutomationControlType.CheckBox: controlType = "checkbox"; break;
                case AutomationControlType.ComboBox: controlType = "combobox"; break;
                case AutomationControlType.Edit: controlType = "edit"; break;
                case AutomationControlType.Hyperlink: controlType = "hyperlink"; break;
                case AutomationControlType.Image: controlType = "image"; break;
                case AutomationControlType.ListItem: controlType = "listitem"; break;
                case AutomationControlType.List: controlType = "list"; break;
                case AutomationControlType.Menu: controlType = "menu"; break;
                case AutomationControlType.MenuBar: controlType = "menubar"; break;
                case AutomationControlType.MenuItem: controlType = "menuitem"; break;
                case AutomationControlType.ProgressBar: controlType = "progressbar"; break;
                case AutomationControlType.RadioButton: controlType = "radiobutton"; break;
                case AutomationControlType.ScrollBar: controlType = "scrollbar"; break;
                case AutomationControlType.Slider: controlType = "slider"; break;
                case AutomationControlType.Spinner: controlType = "spinner"; break;
                case AutomationControlType.StatusBar: controlType = "statusbar"; break;
                case AutomationControlType.Tab: controlType = "tab"; break;
                case AutomationControlType.TabItem: controlType = "tabitem"; break;
                case AutomationControlType.Text: controlType = "text"; break;
                case AutomationControlType.ToolBar: controlType = "toolbar"; break;
                case AutomationControlType.ToolTip: controlType = "tooltip"; break;
                case AutomationControlType.Tree: controlType = "tree"; break;
                case AutomationControlType.TreeItem: controlType = "treeitem"; break;
                case AutomationControlType.Custom: controlType = "custom"; break;
                case AutomationControlType.Group: controlType = "group"; break;
                case AutomationControlType.Thumb: controlType = "thumb"; break;
                case AutomationControlType.DataGrid: controlType = "datagrid"; break;
                case AutomationControlType.DataItem: controlType = "dataitem"; break;
                case AutomationControlType.Document: controlType = "document"; break;
                case AutomationControlType.SplitButton: controlType = "splitbutton"; break;
                case AutomationControlType.Window: controlType = "window"; break;
                case AutomationControlType.Pane: controlType = "pane"; break;
                case AutomationControlType.Header: controlType = "header"; break;
                case AutomationControlType.HeaderItem: controlType = "headeritem"; break;
                case AutomationControlType.Table: controlType = "table"; break;
                case AutomationControlType.TitleBar: controlType = "titlebar"; break;
                case AutomationControlType.Separator: controlType = "separator"; break;
                default: controlType = string.Empty; break;
            }

            return controlType;
        }

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> is laid out in a specific direction. 
        /// This method is called by <see cref="AutomationPeer.GetOrientation" />.
        /// </summary>
        /// <returns>
        /// The <see cref="AutomationOrientation.None" /> enumeration value.
        /// </returns>
        protected override AutomationOrientation GetOrientationCore() => AutomationOrientation.None;

        /// <summary>
        /// Returns a value that indicates whether the <see cref="UIElement" /> that is associated 
        /// with this <see cref="FrameworkElementAutomationPeer" /> currently has keyboard input focus. 
        /// This method is called by <see cref="AutomationPeer.HasKeyboardFocus" />.
        /// </summary>
        /// <returns>
        /// true if the element has keyboard input focus; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        protected override bool HasKeyboardFocusCore() => false;

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" /> can accept keyboard focus. This 
        /// method is called by <see cref="AutomationPeer.IsKeyboardFocusable" />.
        /// </summary>
        /// <returns>
        /// true if the element is focusable by the keyboard; otherwise, false.
        /// </returns>
        protected override bool IsKeyboardFocusableCore() 
            => Owner is Control c && Keyboard.IsFocusable(c);

        /// <summary>
        /// Returns a value that indicates whether the <see cref="UIElement" /> that is 
        /// associated with this <see cref="FrameworkElementAutomationPeer" /> is off the screen. 
        /// This method is called by <see cref="AutomationPeer.IsOffscreen" />.
        /// </summary>
        /// <returns>
        /// true if the element is not on the screen; otherwise, false.
        /// </returns>
        protected override bool IsOffscreenCore() => !Owner.IsVisible;

        /// <summary>
        /// Sets the keyboard input focus on the element that is associated with this 
        /// <see cref="FrameworkElementAutomationPeer" />. This method is called by 
        /// <see cref="AutomationPeer.SetFocus" />.
        /// </summary>
        protected override void SetFocusCore()
        {
            if (Owner is Control c)
            {
                c.Focus();
            }
        }

        internal override AutomationPeer GetParentCore()
        {
            if (Owner != null)
            {
                UIElement parent = VisualTreeHelper.GetParent(Owner) as UIElement;
                while (parent != null)
                {
                    AutomationPeer peer = CreatePeerForElement(parent);
                    if (peer != null)
                    {
                        return peer;
                    }

                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }
    }
}
