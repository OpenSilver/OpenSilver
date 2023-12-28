// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using Resource = OpenSilver.Controls.Input.Toolkit.Resources;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes RatingItem types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class RatingItemAutomationPeer : FrameworkElementAutomationPeer, ISelectionItemProvider
    {
        /// <summary>
        /// Gets the RatingItem that owns this RatingItemAutomationPeer.
        /// </summary>
        private RatingItem OwnerRatingItem
        {
            get { return (RatingItem)Owner; }
        }

        /// <summary>
        /// Initializes a new instance of the RatingAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The Rating that is associated with this
        /// RatingAutomationPeer.
        /// </param>
        public RatingItemAutomationPeer(RatingItem owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Returns the localized control type.
        /// </summary>
        /// <returns>The localized control type.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return Resource.RatingItemAutomationPeer_GetLocalizedControlTypeCore;
        }

        /// <summary>
        /// Gets the control type for the RatingItem that is associated
        /// with this RatingItemAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Custom AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem | AutomationControlType.Button;
        }

        /// <summary>
        /// Gets the name of the RatingItem that is associated with this
        /// RatingItemAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name RatingItem.</returns>
        protected override string GetClassNameCore()
        {
            return "RatingItem";
        }

        /// <summary>
        /// Gets the control pattern for the RatingItem that is associated
        /// with this RatingItemAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the rating item.  Uses the index of the rating
        /// item in the list.
        /// </summary>
        /// <returns>The name of the rating item.</returns>
        protected override string GetNameCore()
        {
            int? index = this.OwnerRatingItem.ParentRating.GetRatingItems().IndexOf(this.OwnerRatingItem);
            if (index != null)
            {
                return (index.Value + 1).ToString(CultureInfo.CurrentUICulture);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Adds the RatingItem to the collection of selected items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.AddToSelection()
        {
            RatingItem owner = OwnerRatingItem;
            Rating parent = owner.ParentRating;
            if (parent == null || parent.Value != null)
            {
                throw new InvalidOperationException(Resource.Automation_OperationCannotBePerformed);
            }

            owner.SelectValue();
        }

        /// <summary>
        /// Gets a value indicating whether the Rating is selected.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        bool ISelectionItemProvider.IsSelected
        {
            get { return OwnerRatingItem.Value > 0.0; }
        }

        /// <summary>
        /// Removes the current Rating from the collection of selected
        /// items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            RatingItem owner = OwnerRatingItem;
            Rating parent = owner.ParentRating;
            if (parent == null)
            {
                throw new InvalidOperationException(Resource.Automation_OperationCannotBePerformed);
            }

            if (!parent.IsReadOnly)
            {
                parent.Value = null;
            }
        }

        /// <summary>
        /// Clears selection from currently selected items and then proceeds to
        /// select the current Rating.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.Select()
        {
            OwnerRatingItem.SelectValue();
        }

        /// <summary>
        /// Gets the UI Automation provider that implements ISelectionProvider
        /// and acts as the container for the calling object.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                Rating parent = OwnerRatingItem.ParentRating;
                if (parent != null)
                {
                    AutomationPeer peer = FromElement(parent);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
                return null;
            }
        }
    }
}
