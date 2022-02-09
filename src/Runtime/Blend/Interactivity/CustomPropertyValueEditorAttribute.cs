// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System;

    /// <summary>
    /// Enumerates possible values for reusable property value editors.
    /// </summary>
    public enum CustomPropertyValueEditor
    {
        /// <summary>
        /// Uses the element picker, if supported, to edit this property at design time.
        /// </summary>
        Element,
        /// <summary>
        /// Uses the storyboard picker, if supported, to edit this property at design time.
        /// </summary>
        Storyboard,
        /// <summary>
        /// Uses the state picker, if supported, to edit this property at design time.
        /// </summary>
        StateName,
        /// <summary>
        /// Uses the element-binding picker, if supported, to edit this property at design time.
        /// </summary>
        ElementBinding,
        /// <summary>
        /// Uses the property-binding picker, if supported, to edit this property at design time.
        /// </summary>
        PropertyBinding,

    }

    /// <summary>
    /// Associates the given editor type with the property on which the CustomPropertyValueEditor is applied.
    /// </summary>
    /// <remarks>Use this attribute to get improved design-time editing for properties that denote element (by name), storyboards, or states (by name).</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CustomPropertyValueEditorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the custom property value editor.
        /// </summary>
        /// <value>The custom property value editor.</value>
        public CustomPropertyValueEditor CustomPropertyValueEditor
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyValueEditorAttribute"/> class.
        /// </summary>
        /// <param name="customPropertyValueEditor">The custom property value editor.</param>
        public CustomPropertyValueEditorAttribute(CustomPropertyValueEditor customPropertyValueEditor)
        {
            this.CustomPropertyValueEditor = customPropertyValueEditor;
        }
    }
}