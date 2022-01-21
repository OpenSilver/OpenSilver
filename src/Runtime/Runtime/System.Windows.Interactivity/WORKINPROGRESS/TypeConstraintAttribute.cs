

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


namespace System.Windows.Interactivity
{
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