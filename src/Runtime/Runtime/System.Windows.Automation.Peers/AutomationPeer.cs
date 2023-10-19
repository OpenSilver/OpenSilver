
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
using System.ComponentModel;
using System.Windows.Automation.Provider;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Provides a base class that exposes an automation peer for an associated class 
    /// to UI Automation.
    /// </summary>
    public abstract class AutomationPeer : DependencyObject
    {
        /// <summary>
        /// Provides initialization for base class values when they are called by 
        /// the constructor of a derived class.
        /// </summary>
        protected AutomationPeer() { }

        /// <summary>
        /// Gets or sets an <see cref="AutomationPeer" /> that is reported to the automation client as a 
        /// source for all the events that come from this <see cref="AutomationPeer" />.
        /// </summary>
        /// <returns>
        /// The <see cref="AutomationPeer" /> that is the source of events.
        /// </returns>
        [OpenSilver.NotImplemented]
        public AutomationPeer EventsSource { get; set; }

        /// <summary>
        /// Gets a value that indicates whether UI Automation is listening for the specified event.
        /// </summary>
        /// <param name="eventId">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// true if UI Automation is listening for the specified event; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static bool ListenerExists(AutomationEvents eventId) => false;

        /// <summary>
        /// Gets the accelerator key combination for the object that is associated 
        /// with the UI Automation peer.
        /// </summary>
        /// <returns>
        /// The accelerator key combination hint string.
        /// </returns>
        public string GetAcceleratorKey() => GetAcceleratorKeyCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetAcceleratorKey" />.
        /// </summary>
        /// <returns>
        /// The accelerator key.
        /// </returns>
        protected abstract string GetAcceleratorKeyCore();

        /// <summary>
        /// Gets the access key for the object that is associated with the automation peer.
        /// </summary>
        /// <returns>
        /// The access key.
        /// </returns>
        public string GetAccessKey() => GetAccessKeyCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetAccessKey" />.
        /// </summary>
        /// <returns>
        /// The access key.
        /// </returns>
        protected abstract string GetAccessKeyCore();

        /// <summary>
        /// Gets the control type for the object that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>
        /// The control type, as a value of the enumeration.
        /// </returns>
        public AutomationControlType GetAutomationControlType() => GetAutomationControlTypeCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// The control type, as a value of the enumeration.
        /// </returns>
        protected abstract AutomationControlType GetAutomationControlTypeCore();

        /// <summary>
        /// Gets the <see cref="P:System.Windows.Automation.AutomationProperties.AutomationId" /> 
        /// of the object that is associated with the automation peer.
        /// </summary>
        /// <returns>
        /// The automation identifier.
        /// </returns>
        public string GetAutomationId() => GetAutomationIdCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetAutomationId" />.
        /// </summary>
        /// <returns>
        /// The automation identifier.
        /// </returns>
        protected abstract string GetAutomationIdCore();

        /// <summary>
        /// Gets the <see cref="Rect" /> object that represents the screen coordinates of the element 
        /// that is associated with the automation peer.
        /// </summary>
        /// <returns>
        /// The bounding rectangle.
        /// </returns>
        public Rect GetBoundingRectangle() => GetBoundingRectangleCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetBoundingRectangle" />.
        /// </summary>
        /// <returns>
        /// The bounding rectangle.
        /// </returns>
        protected abstract Rect GetBoundingRectangleCore();

        /// <summary>
        /// Gets the collection of child elements that are represented in the UI Automation tree 
        /// as immediate child elements of the automation peer.
        /// </summary>
        /// <returns>
        /// The collection of child elements.
        /// </returns>
        public List<AutomationPeer> GetChildren() => GetChildrenCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetChildren" />.
        /// </summary>
        /// <returns>
        /// The collection of child elements.
        /// </returns>
        protected abstract List<AutomationPeer> GetChildrenCore();

        /// <summary>
        /// Gets the name of the control class that is associated with the peer.
        /// </summary>
        /// <returns>
        /// The class name of the associated control class.
        /// </returns>
        public string GetClassName() => GetClassNameCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetClassName" />.
        /// </summary>
        /// <returns>
        /// The class name of the related control class.
        /// </returns>
        protected abstract string GetClassNameCore();

        /// <summary>
        /// Gets a <see cref="Point" /> on the object that is associated with the automation peer 
        /// that responds to a mouse click.
        /// </summary>
        /// <returns>
        /// A point in the clickable area of the element.
        /// </returns>
        public Point GetClickablePoint() => GetClickablePointCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetClickablePoint" />.
        /// </summary>
        /// <returns>
        /// A point within the clickable area of the element.
        /// </returns>
        protected abstract Point GetClickablePointCore();

        /// <summary>
        /// Gets text that describes the functionality of the control that is associated with 
        /// the automation peer.
        /// </summary>
        /// <returns>
        /// The help text.
        /// </returns>
        public string GetHelpText() => GetHelpTextCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetHelpText" />.
        /// </summary>
        /// <returns>
        /// The help text.
        /// </returns>
        protected abstract string GetHelpTextCore();

        /// <summary>
        /// Gets text that conveys the visual status of the object that is associated with this
        /// automation peer.
        /// </summary>
        /// <returns>
        /// The item status.
        /// </returns>
        public string GetItemStatus() => GetItemStatusCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetItemStatus" />.
        /// </summary>
        /// <returns>
        /// The item status.
        /// </returns>
        protected abstract string GetItemStatusCore();

        /// <summary>
        /// Gets a string that describes what kind of item an element represents.
        /// </summary>
        /// <returns>
        /// The kind of item.
        /// </returns>
        public string GetItemType() => GetItemTypeCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetItemType" />.
        /// </summary>
        /// <returns>
        /// The kind of item.
        /// </returns>
        protected abstract string GetItemTypeCore();

        /// <summary>
        /// Gets the <see cref="AutomationPeer" /> for the <see cref="UIElement" /> that 
        /// is targeted to the element.
        /// </summary>
        /// <returns>
        /// The <see cref="AutomationPeer" /> for the element that is targeted by the 
        /// <see cref="UIElement" />.
        /// </returns>
        public AutomationPeer GetLabeledBy() => GetLabeledByCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetLabeledBy" />.
        /// </summary>
        /// <returns>
        /// The <see cref="AutomationPeer" /> for the element that is targeted by the 
        /// <see cref="UIElement" />.
        /// </returns>
        protected abstract AutomationPeer GetLabeledByCore();

        /// <summary>
        /// Gets a localized string that represents the control type, for the control 
        /// that is associated with this automation peer. The localized string parallels 
        /// a <see cref="AutomationControlType" /> value.
        /// </summary>
        /// <returns>
        /// A string that reports the localized type of the associated control.
        /// </returns>
        public string GetLocalizedControlType() => GetLocalizedControlTypeCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetLocalizedControlType" />.
        /// </summary>
        /// <returns>
        /// A string that reports the localized type of the associated control.
        /// </returns>
        protected abstract string GetLocalizedControlTypeCore();

        /// <summary>
        /// Gets the value that the automation peer reports as the UI Automation Name for the 
        /// associated control.
        /// </summary>
        /// <returns>
        /// The value to report as the UI Automation Name.
        /// </returns>
        public string GetName()
        {
            AutomationPeer labeledBy = GetLabeledByCore();
            return labeledBy is null ? GetNameCore() : labeledBy.GetName();
        }

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetName" />.
        /// </summary>
        /// <returns>
        /// The name.
        /// </returns>
        protected abstract string GetNameCore();

        /// <summary>
        /// Gets a value that indicates the explicit control orientation, if any.
        /// </summary>
        /// <returns>
        /// The orientation of the control as a value of the enumeration.
        /// </returns>
        public AutomationOrientation GetOrientation() => GetOrientationCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="GetOrientation" />.
        /// </summary>
        /// <returns>
        /// The orientation of the control.
        /// </returns>
        protected abstract AutomationOrientation GetOrientationCore();

        /// <summary>
        /// Gets a value that indicates whether the object that is associated with this automation 
        /// peer currently has keyboard focus.
        /// </summary>
        /// <returns>
        /// true if the element has keyboard focus; otherwise, false.
        /// </returns>
        public bool HasKeyboardFocus() => HasKeyboardFocusCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="HasKeyboardFocus" />.
        /// </summary>
        /// <returns>
        /// true if the element has keyboard focus; otherwise, false.
        /// </returns>
        protected abstract bool HasKeyboardFocusCore();

        /// <summary>
        /// Gets a value that indicates whether the object that is associated with this automation 
        /// peer contains data that is presented to the user.
        /// </summary>
        /// <returns>
        /// true if the element is a content element; otherwise, false.
        /// </returns>
        public bool IsContentElement() => IsContentElementCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsContentElement" />.
        /// </summary>
        /// <returns>
        /// true if the element is a content element; otherwise, false.
        /// </returns>
        protected abstract bool IsContentElementCore();

        /// <summary>
        /// Gets a value that indicates whether the element is understood by the user as interactive 
        /// or as contributing to the logical structure of the control in the GUI.
        /// </summary>
        /// <returns>
        /// true if the element is a control; otherwise, false.
        /// </returns>
        public bool IsControlElement() => IsControlElementCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsControlElement" />.
        /// </summary>
        /// <returns>
        /// true if the element is a control; otherwise, false.
        /// </returns>
        protected abstract bool IsControlElementCore();

        /// <summary>
        /// Gets a value that indicates whether the element associated with this automation peer 
        /// supports interaction.
        /// </summary>
        /// <returns>
        /// true if the element supports interaction; otherwise, false.
        /// </returns>
        public bool IsEnabled() => IsEnabledCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsEnabled" />.
        /// </summary>
        /// <returns>
        /// true if the automation peer can receive and send events; otherwise, false.
        /// </returns>
        protected abstract bool IsEnabledCore();

        /// <summary>
        /// Gets a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        /// <returns>
        /// true if the element can accept keyboard focus; otherwise, false.
        /// </returns>
        public bool IsKeyboardFocusable() => IsKeyboardFocusableCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsKeyboardFocusable" />.
        /// </summary>
        /// <returns>
        /// true if the element can accept keyboard focus; otherwise, false.
        /// </returns>
        protected abstract bool IsKeyboardFocusableCore();

        /// <summary>
        /// Gets a value that indicates whether an element is off the screen.
        /// </summary>
        /// <returns>
        /// true if the element is not on the screen; otherwise, false.
        /// </returns>
        public bool IsOffscreen() => IsOffscreenCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsOffscreen" />.
        /// </summary>
        /// <returns>
        /// true if the element is not on the screen; otherwise, false.
        /// </returns>
        protected abstract bool IsOffscreenCore();

        /// <summary>
        /// Gets a value that indicates whether the element contains sensitive content.
        /// </summary>
        /// <returns>
        /// true if the element contains sensitive content such as a password; otherwise, false.
        /// </returns>
        public bool IsPassword() => IsPasswordCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsPassword" />.
        /// </summary>
        /// <returns>
        /// true if the element contains sensitive content; otherwise, false.
        /// </returns>
        protected abstract bool IsPasswordCore();

        /// <summary>
        /// Gets a value that indicates whether the object that is associated with this peer must 
        /// be completed on a form.
        /// </summary>
        /// <returns>
        /// true if the element must be completed; otherwise, false.
        /// </returns>
        public bool IsRequiredForForm() => IsRequiredForFormCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="IsRequiredForForm" />.
        /// </summary>
        /// <returns>
        /// true if the element must be completed; otherwise, false.
        /// </returns>
        protected abstract bool IsRequiredForFormCore();

        /// <summary>
        /// Sets the keyboard focus on the object that is associated with this automation peer.
        /// </summary>
        public void SetFocus() => SetFocusCore();

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="SetFocus" />.
        /// </summary>
        protected abstract void SetFocusCore();

        /// <summary>
        /// When overridden in a derived class, gets an object that supports the 
        /// requested pattern, based on <see cref="PatternInterface" /> input and 
        /// the peer's implementation of known patterns.
        /// </summary>
        /// <param name="patternInterface">
        /// A value from the <see cref="PatternInterface" /> enumeration.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface; null if this peer 
        /// does not support this interface.
        /// </returns>
        public abstract object GetPattern(PatternInterface patternInterface);

        /// <summary>
        /// Triggers recalculation of the main properties of the <see cref="AutomationPeer" /> and 
        /// raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> notification to the 
        /// automation client if the properties have changed. 
        /// </summary>
        [OpenSilver.NotImplemented]
        public void InvalidatePeer() { }

        /// <summary>
        /// Gets the <see cref="AutomationPeer" /> that is the parent of this <see cref="AutomationPeer" />.
        /// </summary>
        /// <returns>
        /// The parent automation peer.
        /// </returns>
        public AutomationPeer GetParent() => GetParentCore();

        internal virtual AutomationPeer GetParentCore() => null;

        /// <summary>
        /// Raises an automation event.
        /// </summary>
        /// <param name="eventId">
        /// The event identifier for the event to raise, as a value of the enumeration. See 
        /// <see cref="AutomationEvents" />.
        /// </param>
        [OpenSilver.NotImplemented]
        public void RaiseAutomationEvent(AutomationEvents eventId) { }

        /// <summary>
        /// Raises an event to notify the automation client of a changed property value.
        /// </summary>
        /// <param name="property">
        /// The property that changed.
        /// </param>
        /// <param name="oldValue">
        /// The previous value of the property.
        /// </param>
        /// <param name="newValue">
        /// The new value of the property.
        /// </param>
        [OpenSilver.NotImplemented]
        public void RaisePropertyChangedEvent(AutomationProperty property, object oldValue, object newValue) { }

        /// <summary>
        /// Gets an <see cref="AutomationPeer" /> for the specified <see cref="IRawElementProviderSimple" /> proxy.
        /// </summary>
        /// <param name="provider">
        /// The class that implements <see cref="IRawElementProviderSimple" />.
        /// </param>
        /// <returns>
        /// The <see cref="AutomationPeer" /> for the specified <see cref="IRawElementProviderSimple" /> proxy.
        /// </returns>
        protected AutomationPeer PeerFromProvider(IRawElementProviderSimple provider) => provider.Peer;

        /// <summary>
        /// Gets the <see cref="IRawElementProviderSimple" /> proxy for the specified <see cref="AutomationPeer" />.
        /// </summary>
        /// <param name="peer">
        /// The automation peer.
        /// </param>
        /// <returns>
        /// The proxy.
        /// </returns>
        protected IRawElementProviderSimple ProviderFromPeer(AutomationPeer peer)
            => peer is null ? null : new IRawElementProviderSimple(peer);        
    }
}
