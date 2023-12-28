// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides the necessary infrastructure to enable drawing connecting
    /// lines between the TreeViewItems in a TreeView.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class TreeViewConnectingLines
    {
        #region internal attached TreeViewItemConnectingLineInfo ConnectingLineInfo
        /// <summary>
        /// Gets the value of the ConnectingLineInfo attached property for a
        /// specified TreeViewItem.
        /// </summary>
        /// <param name="element">
        /// The TreeViewItem from which the property value is read.
        /// </param>
        /// <returns>
        /// The ConnectingLineInfo property value for the TreeViewItem.
        /// </returns>
        internal static TreeViewItemConnectingLineInfo GetConnectingLineInfo(TreeViewItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // Get the info and create on demand if necessary
            TreeViewItemConnectingLineInfo info = element.GetValue(ConnectingLineInfoProperty) as TreeViewItemConnectingLineInfo;
            if (info == null)
            {
                info = new TreeViewItemConnectingLineInfo(element);
                element.SetValue(ConnectingLineInfoProperty, info);
            }

            return info;
        }

        /// <summary>
        /// Identifies the ConnectingLineInfo dependency property.
        /// </summary>
        internal static readonly DependencyProperty ConnectingLineInfoProperty =
            DependencyProperty.RegisterAttached(
                "ConnectingLineInfo",
                typeof(TreeViewItemConnectingLineInfo),
                typeof(TreeViewConnectingLines),
                new PropertyMetadata(null));
        #endregion internal attached TreeViewItemConnectingLineInfo ConnectingLineInfo

        #region public attached TreeViewItem IsVerticalConnectingLineOf
        /// <summary>
        /// Gets the value of the IsVerticalConnectingLineOf attached property
        /// for a specified Line.
        /// </summary>
        /// <param name="element">The Line from which the property value is read.</param>
        /// <returns>The IsVerticalConnectingLineOf property value for the Line.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's vertical connecting line.")]
        public static TreeViewItem GetIsVerticalConnectingLineOf(Line element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsVerticalConnectingLineOfProperty) as TreeViewItem;
        }

        /// <summary>
        /// Sets the value of the IsVerticalConnectingLineOf attached property to a specified Line.
        /// </summary>
        /// <param name="element">The Line to which the attached property is written.</param>
        /// <param name="value">The needed IsVerticalConnectingLineOf value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's vertical connecting line.")]
        public static void SetIsVerticalConnectingLineOf(Line element, TreeViewItem value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsVerticalConnectingLineOfProperty, value);
        }

        /// <summary>
        /// Identifies the IsVerticalConnectingLineOf dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVerticalConnectingLineOfProperty =
            DependencyProperty.RegisterAttached(
                "IsVerticalConnectingLineOf",
                typeof(TreeViewItem),
                typeof(TreeViewConnectingLines),
                new PropertyMetadata(null, OnIsVerticalConnectingLineOfPropertyChanged));

        /// <summary>
        /// IsVerticalConnectingLineOfProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// Line that changed its IsVerticalConnectingLineOf TreeViewItem.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsVerticalConnectingLineOfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line source = d as Line;
            TreeViewItem value = e.NewValue as TreeViewItem;

            if (value != null)
            {
                TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                info.VerticalConnectingLine = source;
            }
            else
            {
                value = e.OldValue as TreeViewItem;
                if (value != null)
                {
                    TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                    info.VerticalConnectingLine = null;
                }
            }
        }
        #endregion public attached TreeViewItem IsVerticalConnectingLineOf

        #region public attached TreeViewItem IsHorizontalConnectingLineOf
        /// <summary>
        /// Gets the value of the IsHorizontalConnectingLineOf attached property
        /// for a specified Line.
        /// </summary>
        /// <param name="element">
        /// The Line from which the property value is read.
        /// </param>
        /// <returns>
        /// The IsHorizontalConnectingLineOf property value for the Line.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's horizontal connecting line.")]
        public static TreeViewItem GetIsHorizontalConnectingLineOf(Line element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsHorizontalConnectingLineOfProperty) as TreeViewItem;
        }

        /// <summary>
        /// Sets the value of the IsHorizontalConnectingLineOf attached property
        /// to a specified Line.
        /// </summary>
        /// <param name="element">
        /// The Line to which the attached property is written.
        /// </param>
        /// <param name="value">
        /// The needed IsHorizontalConnectingLineOf value.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's horizontal connecting line.")]
        public static void SetIsHorizontalConnectingLineOf(Line element, TreeViewItem value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsHorizontalConnectingLineOfProperty, value);
        }

        /// <summary>
        /// Identifies the IsHorizontalConnectingLineOf dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHorizontalConnectingLineOfProperty =
            DependencyProperty.RegisterAttached(
                "IsHorizontalConnectingLineOf",
                typeof(TreeViewItem),
                typeof(TreeViewConnectingLines),
                new PropertyMetadata(null, OnIsHorizontalConnectingLineOfPropertyChanged));

        /// <summary>
        /// IsHorizontalConnectingLineOfProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// Line that changed its IsHorizontalConnectingLineOf TreeViewItem.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsHorizontalConnectingLineOfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Line source = d as Line;
            TreeViewItem value = e.NewValue as TreeViewItem;

            if (value != null)
            {
                TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                info.HorizontalConnectingLine = source;
            }
            else
            {
                value = e.OldValue as TreeViewItem;
                if (value != null)
                {
                    TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                    info.HorizontalConnectingLine = null;
                }
            }
        }
        #endregion public attached TreeViewItem IsHorizontalConnectingLineOf

        #region public attached TreeViewItem IsExpanderButtonOf
        /// <summary>
        /// Gets the value of the IsExpanderButtonOf attached property for a
        /// specified ToggleButton.
        /// </summary>
        /// <param name="element">
        /// The ToggleButton from which the property value is read.
        /// </param>
        /// <returns>
        /// The IsExpanderButtonOf property value for the ToggleButton.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's ExpanderButton.")]
        public static TreeViewItem GetIsExpanderButtonOf(ToggleButton element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsExpanderButtonOfProperty) as TreeViewItem;
        }

        /// <summary>
        /// Sets the value of the IsExpanderButtonOf attached property to a
        /// specified ToggleButton.
        /// </summary>
        /// <param name="element">
        /// The ToggleButton to which the attached property is written.
        /// </param>
        /// <param name="value">The needed IsExpanderButtonOf value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's ExpanderButton.")]
        public static void SetIsExpanderButtonOf(ToggleButton element, TreeViewItem value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsExpanderButtonOfProperty, value);
        }

        /// <summary>
        /// Identifies the IsExpanderButtonOf dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpanderButtonOfProperty =
            DependencyProperty.RegisterAttached(
                "IsExpanderButtonOf",
                typeof(TreeViewItem),
                typeof(TreeViewConnectingLines),
                new PropertyMetadata(null, OnIsExpanderButtonOfPropertyChanged));

        /// <summary>
        /// IsExpanderButtonOfProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// ToggleButton that changed its IsExpanderButtonOf TreeViewItem.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsExpanderButtonOfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton source = d as ToggleButton;
            TreeViewItem value = e.NewValue as TreeViewItem;

            if (value != null)
            {
                TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                info.ExpanderButton = source;
            }
            else
            {
                value = e.OldValue as TreeViewItem;
                if (value != null)
                {
                    TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                    info.ExpanderButton = null;
                }
            }
        }
        #endregion public attached TreeViewItem IsExpanderButtonOf

        #region public attached TreeViewItem IsHeaderOf
        /// <summary>
        /// Gets the value of the IsHeaderOf attached property for a specified
        /// FrameworkElement.
        /// </summary>
        /// <param name="element">
        /// The FrameworkElement from which the property value is read.
        /// </param>
        /// <returns>
        /// The IsHeaderOf property value for the FrameworkElement.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's Header.")]
        public static TreeViewItem GetIsHeaderOf(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.GetValue(IsHeaderOfProperty) as TreeViewItem;
        }

        /// <summary>
        /// Sets the value of the IsHeaderOf attached property to a specified
        /// FrameworkElement.
        /// </summary>
        /// <param name="element">
        /// The FrameworkElement to which the attached property is written.
        /// </param>
        /// <param name="value">The needed IsHeaderOf value.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific to the TreeViewItem's Header.")]
        public static void SetIsHeaderOf(FrameworkElement element, TreeViewItem value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsHeaderOfProperty, value);
        }

        /// <summary>
        /// Identifies the IsHeaderOf dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHeaderOfProperty =
            DependencyProperty.RegisterAttached(
                "IsHeaderOf",
                typeof(TreeViewItem),
                typeof(TreeViewConnectingLines),
                new PropertyMetadata(null, OnIsHeaderOfPropertyChanged));

        /// <summary>
        /// IsHeaderOfProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// FrameworkElement that changed its IsHeaderOf TreeViewItem.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsHeaderOfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement source = d as FrameworkElement;
            TreeViewItem value = e.NewValue as TreeViewItem;

            if (value != null)
            {
                TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                info.Header = source;
            }
            else
            {
                value = e.OldValue as TreeViewItem;
                if (value != null)
                {
                    TreeViewItemConnectingLineInfo info = GetConnectingLineInfo(value);
                    info.Header = null;
                }
            }
        }
        #endregion public attached TreeViewItem IsHeaderOf
    }
}