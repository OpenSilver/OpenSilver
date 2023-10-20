//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Data;

namespace System.Windows.Controls
{
    internal class ValidationHelper
    {
        #region AttachedProperties

        #region ValidationMetadata

        /// <summary>
        /// Gets or sets the ValidationMetadata, which represents all of the metadata associated with the binding path of the input control.  This includes
        /// IsFieldRequired, RequiredFieldMessage, Caption, and Description.
        /// </summary>
        internal static readonly DependencyProperty ValidationMetadataProperty = DependencyProperty.RegisterAttached(
            "ValidationMetadata",
            typeof(ValidationMetadata),
            typeof(ValidationHelper),
            null);

        /// <summary>
        /// Gets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to get the ValidationMetadata property from.</param>
        /// <returns>The ValidationMetadata associated with the input control.</returns>
        internal static ValidationMetadata GetValidationMetadata(DependencyObject inputControl)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            return inputControl.GetValue(ValidationMetadataProperty) as ValidationMetadata;
        }

        /// <summary>
        /// Sets the ValidationMetadata property for the input control
        /// </summary>
        /// <param name="inputControl">The input control to set the ValidationMetadata property on.</param>
        /// <param name="value">The ValidationMetadata to associate with the input control.</param>
        internal static void SetValidationMetadata(DependencyObject inputControl, ValidationMetadata value)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            inputControl.SetValue(ValidationMetadataProperty, value);
        }

        #endregion ValidationMetadata

        #endregion AttachedProperties

        #region Methods

        #region Static Methods

        /// <summary>
        /// Parse metadata from a target FrameworkElement.  This will cache the metadata on the element as an attached property.
        /// </summary>
        /// <param name="element">The target FrameworkElement to pull metadata from.</param>
        /// <param name="forceUpdate">If set, will not pull metadata from cache.</param>
        /// <param name="entity">The entity used.</param>
        /// <param name="bindingExpression">The bindingExpression used.</param>
        /// <returns>Returns the metadata associated with the element.  Will be null if no metadata was found.</returns>
        internal static ValidationMetadata ParseMetadata(FrameworkElement element, bool forceUpdate, out object entity, out BindingExpression bindingExpression)
        {
            entity = null;
            bindingExpression = null;
            if (element == null)
            {
                return null;
            }

            if (!forceUpdate)
            {
                ValidationMetadata existingVMD = element.GetValue(ValidationMetadataProperty) as ValidationMetadata;
                if (existingVMD != null)
                {
                    return existingVMD;
                }
            }

            BindingExpression be = null;
            FieldInfo[] fields = element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(DependencyProperty))
                {
                    // Found a dependency property
                    be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                    if (be != null && be.ParentBinding != null && be.ParentBinding.Path != null)
                    {
                        // Found a BindingExpression, ensure it has valid data
                        entity = be.DataItem != null ? be.DataItem : element.DataContext;
                        if (entity != null)
                        {
                            if (be.ParentBinding.Mode == BindingMode.TwoWay)
                            {
                                bindingExpression = be;
                                // A twoway binding will be automatically chosen and the rest ignored
                                break;
                            }

                            // Perform an arbitrary sort on path (string), so the same dependency property is chosen consistently.
                            // Reflection ordering is not deterministic and if we just pick the first, we could be 
                            // matched with different dependency properties depending on the run.
                            if (bindingExpression == null || string.Compare(be.ParentBinding.Path.Path, bindingExpression.ParentBinding.Path.Path, StringComparison.Ordinal) < 0)
                            {
                                bindingExpression = be;
                            }
                        }
                    }
                }
            }
            if (bindingExpression != null)
            {
                ValidationMetadata newVMD = ParseMetadata(bindingExpression.ParentBinding.Path.Path, entity);
                element.SetValue(ValidationMetadataProperty, newVMD);
                return newVMD;
            }
            return null;
        }

        /// <summary>
        /// Parse metadata given a binding path and entity object.
        /// </summary>
        /// <param name="bindingPath">The bindingPath is the name of the property on the entity from which to pull metadata from.  This supports dot notation.</param>
        /// <param name="entity">The entity object from which to pull metadata from.</param>
        /// <returns>The validation metadata associated with the entity and binding path.  This will return null if none exists.</returns>
        internal static ValidationMetadata ParseMetadata(string bindingPath, object entity)
        {
            if (entity != null && !String.IsNullOrEmpty(bindingPath))
            {
                Type entityType = entity.GetType();
                PropertyInfo prop = GetProperty(entityType, bindingPath);
                if (prop != null)
                {
                    ValidationMetadata newVMD = new ValidationMetadata();
                    object[] attributes = prop.GetCustomAttributes(false);
                    foreach (object propertyAttribute in attributes)
                    {
                        // Loop through each attribute and update the VMD as appropriate

                        // RequiredField
                        RequiredAttribute reqAttribute = propertyAttribute as RequiredAttribute;
                        if (reqAttribute != null)
                        {
                            newVMD.IsRequired = true;
                            continue;
                        }

                        // Display attribute parsing
                        DisplayAttribute displayAttribute = propertyAttribute as DisplayAttribute;
                        if (displayAttribute != null)
                        {
                            newVMD.Description = displayAttribute.GetDescription();
                            newVMD.Caption = displayAttribute.GetName();
                            continue;
                        }
                    }
                    if (newVMD.Caption == null)
                    {
                        // If the name is not defined via the DisplayAttribute, use the property name. 
                        newVMD.Caption = prop.Name;

                        // Caption can be set to empty string to have an empty Caption and not default 
                        // to the property name.
                    }

                    return newVMD;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the PropertyInfo for a given entity type.  Supports dot notation to represent nested objects.
        /// </summary>
        /// <param name="entityType">The type of the object.</param>
        /// <param name="propertyPath">The property path, supporting dot notation.</param>
        /// <returns>The PropertyInfo corresponding to the final property.</returns>
        private static PropertyInfo GetProperty(Type entityType, string propertyPath)
        {
            Debug.Assert(entityType != null, "Unexpected null entityType in ValidationHelper.GetProperty");
            Debug.Assert(propertyPath != null, "Unexpected null propertyPath in ValidationHelper.GetProperty");
            Type itemType = entityType;

            string[] propertyNames = propertyPath.Split('.');
            if (propertyNames != null)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    PropertyInfo propertyInfo = itemType.GetProperty(propertyNames[i]);
                    if (propertyInfo == null || !propertyInfo.CanRead)
                    {
                        return null;
                    }

                    if (i == propertyNames.Length - 1)
                    {
                        return propertyInfo;
                    }
                    else
                    {
                        itemType = propertyInfo.PropertyType;
                    }
                }
            }
            return null;
        }

#endregion Static Methods

#endregion Methods

    }
}
