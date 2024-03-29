﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenSilver.Internal.Expression.Interactivity {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionStringTable {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionStringTable() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Expression.Interactivity.ExceptionStringTable", typeof(ExceptionStringTable).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find method named &apos;{0}&apos; on object of type &apos;{1}&apos; that matches the expected signature..
        /// </summary>
        internal static string CallMethodActionValidMethodNotFoundExceptionMessage {
            get {
                return ResourceManager.GetString("CallMethodActionValidMethodNotFoundExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one potential addition operator was found on type &apos;{0}&apos;..
        /// </summary>
        internal static string ChangePropertyActionAmbiguousAdditionOperationExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionAmbiguousAdditionOperationExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot animate a property change on a type &apos;{0}&apos; Target. Property changes can only be animated on types derived from DependencyObject..
        /// </summary>
        internal static string ChangePropertyActionCannotAnimateTargetTypeExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionCannotAnimateTargetTypeExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find a property named &quot;{0}&quot; on type &quot;{1}&quot;..
        /// </summary>
        internal static string ChangePropertyActionCannotFindPropertyNameExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionCannotFindPropertyNameExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Increment property cannot be set to True if the Duration property is set..
        /// </summary>
        internal static string ChangePropertyActionCannotIncrementAnimatedPropertyChangeExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionCannotIncrementAnimatedPropertyChangeExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; property cannot be incremented because its value cannot be read..
        /// </summary>
        internal static string ChangePropertyActionCannotIncrementWriteOnlyPropertyExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionCannotIncrementWriteOnlyPropertyExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot assign value of type &quot;{0}&quot; to property &quot;{1}&quot; of type &quot;{2}&quot;. The &quot;{1}&quot; property can be assigned only values of type &quot;{2}&quot;..
        /// </summary>
        internal static string ChangePropertyActionCannotSetValueExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionCannotSetValueExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &quot;{0}&quot; defined by type &quot;{1}&quot; does not expose a set method and therefore cannot be modified..
        /// </summary>
        internal static string ChangePropertyActionPropertyIsReadOnlyExceptionMessage {
            get {
                return ResourceManager.GetString("ChangePropertyActionPropertyIsReadOnlyExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find state named &apos;{0}&apos; on type &apos;{1}&apos;. Ensure that the state exists and that it can be accessed from this context..
        /// </summary>
        internal static string DataStateBehaviorStateNameNotFoundExceptionMessage {
            get {
                return ResourceManager.GetString("DataStateBehaviorStateNameNotFoundExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Target {0} does not define any VisualStateGroups. .
        /// </summary>
        internal static string GoToStateActionTargetHasNoStateGroups {
            get {
                return ResourceManager.GetString("GoToStateActionTargetHasNoStateGroups", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LeftOperand of type &quot;{0}&quot; cannot be used with operator &quot;{1}&quot;..
        /// </summary>
        internal static string InvalidLeftOperand {
            get {
                return ResourceManager.GetString("InvalidLeftOperand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LeftOperand of type &quot;{1}&quot; and RightOperand of type &quot;{0}&quot; cannot be used with operator &quot;{2}&quot;..
        /// </summary>
        internal static string InvalidOperands {
            get {
                return ResourceManager.GetString("InvalidOperands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RightOperand of type &quot;{0}&quot; cannot be used with operator &quot;{1}&quot;..
        /// </summary>
        internal static string InvalidRightOperand {
            get {
                return ResourceManager.GetString("InvalidRightOperand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The target of the RemoveElementAction is not supported..
        /// </summary>
        internal static string UnsupportedRemoveTargetExceptionMessage {
            get {
                return ResourceManager.GetString("UnsupportedRemoveTargetExceptionMessage", resourceCulture);
            }
        }
    }
}
