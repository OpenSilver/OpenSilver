// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System;

    /// <summary>
    /// Specifies type constraints on the AssociatedObject of TargetedTriggerAction and EventTriggerBase.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TypeConstraintAttribute : Attribute
    {
        /// <summary>
        /// Gets the constraint type.
        /// </summary>
        /// <value>The constraint type.</value>
        public Type Constraint
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraint">The constraint type.</param>
        public TypeConstraintAttribute(Type constraint)
        {
            this.Constraint = constraint;
        }
    }
}
