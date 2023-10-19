
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
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes a data item in an <see cref="ItemsControl.Items"/>  collection
    /// to UI automation.
    /// </summary>
    public abstract class ItemAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Provides initialization for base class values when called by the constructor
        /// of a derived class.
        /// </summary>
        /// <param name="item">
        /// The data item in the <see cref="ItemsControl.Items"/>  collection that
        /// is associated with this <see cref="ItemAutomationPeer"/> .
        /// </param>
        protected ItemAutomationPeer(UIElement item)
            : base(item)
        {
            Item = item ?? throw new InvalidOperationException("Cannot have null parent for non-UIElement item.");
        }

        /// <summary>
        /// Provides initialization for base class values when called by the constructor
        /// of a derived class.
        /// </summary>
        /// <param name="item">
        /// The data item in the <see cref="ItemsControl.Items"/> collection that
        /// is associated with this <see cref="ItemAutomationPeer"/> .
        /// </param>
        /// <param name="itemsControlAutomationPeer">
        /// The <see cref="ItemsControlAutomationPeer"/>  that is associated with the 
        /// <see cref="ItemsControl"/>  that holds the <see cref="ItemsControl.Items"/> 
        /// collection.
        /// </param>
        protected ItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
            : base(item)
        {
            ItemsControlAutomationPeer = itemsControlAutomationPeer;
            
            if (itemsControlAutomationPeer is null && !(item is UIElement))
            {
                throw new InvalidOperationException("Cannot have null parent for non-UIElement item.");
            }

            Item = item;
        }

        /// <summary>
        /// Gets the requested data item in the <see cref="ItemsControl.Items"/> 
        /// collection that is associated with this <see cref="ItemAutomationPeer"/> .
        /// </summary>
        /// <returns>
        /// The requested data item.
        /// </returns>
        protected object Item { get; }

        /// <summary>
        /// Gets the <see cref="ItemsControlAutomationPeer"/>  that is associated
        /// with the <see cref="ItemsControl"/>  for this peer's <see cref="ItemsControl.Items"/> .
        /// </summary>
        /// <returns>
        /// The <see cref="ItemsControlAutomationPeer"/>  associated with the <see cref="ItemsControl"/> 
        /// that holds the <see cref="ItemsControl.Items"/> collection.
        /// </returns>
        protected internal ItemsControlAutomationPeer ItemsControlAutomationPeer
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns an object that supports the requested pattern, based on the patterns
        /// supported by this <see cref="ItemAutomationPeer"/> .
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern
        /// interface is not implemented.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (Owner != null)
            {
                return base.GetPattern(patternInterface);
            }

            return GetWrapperPeer()?.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns the accelerator key for the item element that is associated with this
        /// <see cref="ItemAutomationPeer"/>. This method is called by <see cref="AutomationPeer.GetAcceleratorKey"/> .
        /// </summary>
        /// <returns>
        /// The <see cref="P:System.Windows.Automation.AutomationProperties.AcceleratorKey"/> that is returned
        /// by <see cref="AutomationProperties.GetAcceleratorKey(DependencyObject)"/> .
        /// </returns>
        protected override string GetAcceleratorKeyCore()
        {
            if (Owner != null)
            {
                return base.GetAcceleratorKeyCore();
            }

            return GetWrapperPeer()?.GetAcceleratorKey() ?? string.Empty;
        }

        /// <summary>
        /// Returns the access key for the item element that is associated with this <see cref="ItemAutomationPeer"/> .
        /// This method is called by <see cref="AutomationPeer.GetAccessKey"/> .
        /// </summary>
        /// <returns>
        /// The access key for the element that is associated with this <see cref="ItemAutomationPeer"/> .
        /// </returns>
        protected override string GetAccessKeyCore()
        {
            if (Owner != null)
            {
                return base.GetAccessKeyCore();
            }

            return GetWrapperPeer()?.GetAccessKey() ?? string.Empty;
        }

        /// <summary>
        /// Returns the control type for the item element that is associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            if (Owner != null)
            {
                return base.GetAutomationControlTypeCore();
            }

            return GetWrapperPeer()?.GetAutomationControlType() ?? AutomationControlType.ListItem;
        }

        /// <summary>
        /// Returns the string that uniquely identifies the item element that is associated
        /// with this <see cref="ItemAutomationPeer"/>. This method is called by 
        /// <see cref="AutomationPeer.GetAutomationId"/> .
        /// </summary>
        /// <returns>
        /// The automation identifier for the element associated with the <see cref="ItemAutomationPeer"/>,
        /// or <see cref="string.Empty"/> if there is no automation identifier.
        /// </returns>
        protected override string GetAutomationIdCore()
        {
            if (Owner != null)
            {
                return base.GetAutomationIdCore();
            }

            return GetWrapperPeer()?.GetAutomationId() ?? string.Empty;
        }

        /// <summary>
        /// Returns the <see cref="Rect"/>  that represents the bounding rectangle of the
        /// item element that is associated with this <see cref="ItemAutomationPeer"/> .
        /// This method is called by <see cref="AutomationPeer.GetBoundingRectangle"/> .
        /// </summary>
        /// <returns>
        /// The <see cref="Rect"/>  that contains the coordinates of the element item.
        /// </returns>
        protected override Rect GetBoundingRectangleCore()
        {
            if (Owner != null)
            {
                return base.GetBoundingRectangleCore();
            }

            return GetWrapperPeer()?.GetBoundingRectangle() ?? default;
        }

        /// <summary>
        /// Returns automation peers for the collection of child elements of the owner. The
        /// owner class is associated with the <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetChildren"/>.
        /// </summary>
        /// <returns>
        /// A list of <see cref="AutomationPeer"/> elements.
        /// </returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            if (Owner != null)
            {
                return base.GetChildrenCore();
            }

            return GetWrapperPeer()?.GetChildren();
        }

        /// <summary>
        /// Returns name of the class that is associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The class name. See Remarks.
        /// </returns>
        /// <remarks>
        /// If the item is a <see cref="UIElement"/>  with its own peer as reported by Owner, then 
        /// information from that peer is forwarded. Next, the container peer (which could be 
        /// <see cref="ItemsControlAutomationPeer"/>, <see cref="ListBoxAutomationPeer"/>, or another 
        /// class) is used as a possible source. Finally, if there is no container peer, then this method 
        /// returns <see cref="string.Empty"/>.
        /// </remarks>
        protected override string GetClassNameCore()
        {
            if (Owner != null)
            {
                return base.GetClassNameCore();
            }

            return GetWrapperPeer()?.GetClassName() ?? string.Empty;
        }

        /// <summary>
        /// Returns a <see cref="Point"/> that represents the clickable space for the item
        /// element that is associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetClickablePoint"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="Point"/> on the element that allows a click.
        /// </returns>
        protected override Point GetClickablePointCore()
        {
            if (Owner != null)
            {
                return base.GetClickablePointCore();
            }

            return GetWrapperPeer()?.GetClickablePoint() ?? new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns the string that describes the functionality of the item element that
        /// is associated with this <see cref="ItemAutomationPeer"/>. This method is called 
        /// by <see cref="AutomationPeer.GetHelpText"/>.
        /// </summary>
        /// <returns>
        /// The help text, or <see cref="string.Empty"/> if there is no help text.
        /// </returns>
        protected override string GetHelpTextCore()
        {
            if (Owner != null)
            {
                return base.GetHelpTextCore();
            }

            return GetWrapperPeer()?.GetHelpText() ?? string.Empty;
        }

        /// <summary>
        /// Returns a string that communicates the visual status of the item element that
        /// is associated with this <see cref="ItemAutomationPeer"/>. This method is called 
        /// by <see cref="AutomationPeer.GetItemStatus"/>.
        /// </summary>
        /// <returns>
        /// The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.ItemStatus"/> 
        /// that is returned by <see cref="AutomationProperties.GetItemStatus(DependencyObject)"/>.
        /// </returns>
        protected override string GetItemStatusCore()
        {
            if (Owner != null)
            {
                return base.GetItemStatusCore();
            }

            return GetWrapperPeer()?.GetItemStatus() ?? string.Empty;
        }

        /// <summary>
        /// Returns a human-readable string that contains the type of item element that the
        /// item represents. This method is called by <see cref="AutomationPeer.GetItemType"/>.
        /// </summary>
        /// <returns>
        /// The item type string.
        /// </returns>
        protected override string GetItemTypeCore()
        {
            if (Owner != null)
            {
                return base.GetItemTypeCore();
            }

            return GetWrapperPeer()?.GetItemType() ?? string.Empty;
        }

        /// <summary>
        /// Returns the <see cref="AutomationPeer"/> for the object that targets the item that is 
        /// associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetLabeledBy"/>.
        /// </summary>
        /// <returns>
        /// An automation peer for the object that label-targets the peer's owner.
        /// </returns>
        protected override AutomationPeer GetLabeledByCore()
        {
            if (Owner != null)
            {
                return base.GetLabeledByCore();
            }

            return GetWrapperPeer()?.GetLabeledBy();
        }

        /// <summary>
        /// Returns a localized human-readable string that represents a control type. The
        /// control is the owner type that is associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetLocalizedControlType"/>.
        /// </summary>
        /// <returns>
        /// The string that contains the type of control.
        /// </returns>
        protected override string GetLocalizedControlTypeCore()
        {
            if (Owner != null)
            {
                return base.GetLocalizedControlTypeCore();
            }

            return GetWrapperPeer()?.GetLocalizedControlType() ?? string.Empty;
        }

        /// <summary>
        /// Gets the UI Automation Name from the element that corresponds to a data item.
        /// The item is the element in an <see cref="ItemsControl.Items"/> collection that 
        /// is associated with this <see cref="ItemAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.GetName"/>.
        /// </summary>
        /// <returns>
        /// The UI Automation Name from the element that corresponds to a data item.
        /// </returns>
        protected override string GetNameCore()
        {
            if (Owner != null)
            {
                return base.GetNameCore();
            }

            return GetWrapperPeer()?.GetName() ?? string.Empty;
        }

        /// <summary>
        /// Returns a value that indicates whether the element that is associated with this
        /// <see cref="ItemAutomationPeer"/> is laid out in a specific direction.
        /// This method is called by <see cref="AutomationPeer.GetOrientation"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationOrientation GetOrientationCore()
        {
            if (Owner != null)
            {
                return base.GetOrientationCore();
            }

            return GetWrapperPeer()?.GetOrientation() ?? AutomationOrientation.None;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> currently has keyboard input focus.
        /// This method is called by <see cref="AutomationPeer.HasKeyboardFocus"/>.
        /// </summary>
        /// <returns>
        /// true if the element has keyboard input focus; otherwise, false.
        /// </returns>
        protected override bool HasKeyboardFocusCore()
        {
            if (Owner != null)
            {
                return base.HasKeyboardFocusCore();
            }

            return GetWrapperPeer()?.HasKeyboardFocus() ?? false;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> is an element that contains data that is 
        /// presented to the user. This method is called by <see cref="AutomationPeer.IsContentElement"/>.
        /// </summary>
        /// <returns>
        /// true if the element contains data for the user to read; otherwise, false.
        /// </returns>
        protected override bool IsContentElementCore()
        {
            if (Owner != null)
            {
                return base.IsContentElementCore();
            }

            return GetWrapperPeer()?.IsContentElement() ?? true;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> is understood by the end user as interactive.
        /// Optionally, the user might understand the element as contributing to the logical 
        /// structure of the control in the GUI. This method is called by <see cref="AutomationPeer.IsControlElement"/>.
        /// </summary>
        /// <returns>
        /// true if the element is interactive; otherwise, false.
        /// </returns>
        protected override bool IsControlElementCore()
        {
            if (Owner != null)
            {
                return base.IsControlElementCore();
            }

            return GetWrapperPeer()?.IsControlElement() ?? true;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> is enabled.
        /// This method
        /// is called by <see cref="AutomationPeer.IsEnabled"/>.
        /// </summary>
        /// <returns>
        /// true if the element is enabled; otherwise, false.
        /// </returns>
        protected override bool IsEnabledCore()
        {
            if (Owner != null)
            {
                return base.IsEnabledCore();
            }

            return GetWrapperPeer()?.IsEnabled() ?? false;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> can accept keyboard focus.
        /// This method is called by <see cref="AutomationPeer.IsKeyboardFocusable"/>.
        /// </summary>
        /// <returns>
        /// true if the item element is focusable by the keyboard; otherwise, false.
        /// </returns>
        protected override bool IsKeyboardFocusableCore()
        {
            if (Owner != null)
            {
                return base.IsKeyboardFocusableCore();
            }

            return GetWrapperPeer()?.IsKeyboardFocusable() ?? false;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> is off the screen.
        /// This method is called by <see cref="AutomationPeer.IsOffscreen"/>.
        /// </summary>
        /// <returns>
        /// true if the item element is not on the screen; otherwise, false.
        /// </returns>
        protected override bool IsOffscreenCore()
        {
            if (Owner != null)
            {
                return base.IsOffscreenCore();
            }

            return GetWrapperPeer()?.IsOffscreen() ?? true;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> contains protected content.
        /// This method is called by <see cref="AutomationPeer.IsPassword"/>.
        /// </summary>
        /// <returns>
        /// true if the item element contains sensitive content; otherwise, false.
        /// </returns>
        protected override bool IsPasswordCore()
        {
            if (Owner != null)
            {
                return base.IsPasswordCore();
            }

            return GetWrapperPeer()?.IsPassword() ?? false;
        }

        /// <summary>
        /// Returns a value that indicates whether the item element that is associated with
        /// this <see cref="ItemAutomationPeer"/> is required to be completed on a form.
        /// This method is called by <see cref="AutomationPeer.IsRequiredForForm"/>.
        /// </summary>
        /// <returns>
        /// The value that is returned by <see cref="AutomationProperties.GetIsRequiredForForm(DependencyObject)"/>,
        /// if the value is set; otherwise, false.
        /// </returns>
        protected override bool IsRequiredForFormCore()
        {
            if (Owner != null)
            {
                return base.IsRequiredForFormCore();
            }

            return GetWrapperPeer()?.IsRequiredForForm() ?? false;
        }

        /// <summary>
        /// Sets the keyboard input focus on the item element that is associated with this
        /// <see cref="FrameworkElementAutomationPeer"/>.
        /// This method is called by <see cref="AutomationPeer.SetFocus"/>.
        /// </summary>
        protected override void SetFocusCore()
        {
            if (Owner != null)
            {
                base.SetFocusCore();
            }

            GetWrapperPeer()?.SetFocus();
        }

        internal override AutomationPeer GetParentCore()
            => ItemsControlAutomationPeer ?? base.GetParentCore();

        private UIElement GetWrapper()
        {
            UIElement wrapper = null;
            if (ItemsControlAutomationPeer != null)
            {
                ItemsControl owner = (ItemsControl)ItemsControlAutomationPeer.Owner;
                wrapper = owner.ItemContainerGenerator.ContainerFromItem(Item) as UIElement;
            }

            return wrapper;
        }

        private AutomationPeer GetWrapperPeer()
        {
            AutomationPeer wrapperPeer = null;
            UIElement wrapper = GetWrapper();
            if (wrapper != null)
            {
                wrapperPeer = CreatePeerForElement(wrapper)
                    ?? new FrameworkElementAutomationPeer(wrapper);
            }

            return wrapperPeer;
        }
    }
}
