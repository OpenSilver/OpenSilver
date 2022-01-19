

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


#if MIGRATION

namespace System.Windows.Interactivity
{
	/// <summary>
	/// Represents an action that can be targeted to affect an object other than its AssociatedObject.
	/// </summary>
	/// <typeparam name="T">The type constraint on the target.</typeparam>
	/// <remarks>
	///		TargetedTriggerAction extends TriggerAction to add knowledge of another element than the one it is attached to. 
	///		This allows a user to invoke the action on an element other than the one it is attached to in response to a 
	///		trigger firing. Override OnTargetChanged to hook or unhook handlers on the target element, and OnAttached/OnDetaching 
	///		for the associated element. The type of the Target element can be constrained by the generic type parameter. If 
	///		you need control over the type of the AssociatedObject, set a TypeConstraintAttribute on your derived type.
	/// </remarks>
	public abstract partial class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TargetedTriggerAction&lt;T&gt;"/> class.
		/// </summary>
		protected TargetedTriggerAction()
			: base(typeof(T))
		{
		}

		/// <summary>
		/// Gets the target object. If TargetName is not set or cannot be resolved, defaults to the AssociatedObject.
		/// </summary>
		/// <value>The target.</value>
		/// <remarks>In general, this property should be used in place of AssociatedObject in derived classes.</remarks>
		protected new T Target
		{
			get
			{
				return (T)base.Target;
			}
		}

		internal sealed override void OnTargetChangedImpl(object oldTarget, object newTarget)
		{
			base.OnTargetChangedImpl(oldTarget, newTarget);
			this.OnTargetChanged(oldTarget as T, newTarget as T);
		}

		/// <summary>
		/// Called when the target property changes.
		/// </summary>
		/// <remarks>Override this to hook and unhook functionality on the specified Target, rather than the AssociatedObject.</remarks>
		/// <param name="oldTarget">The old target.</param>
		/// <param name="newTarget">The new target.</param>
		protected virtual void OnTargetChanged(T oldTarget, T newTarget)
		{
		}
	}
}

#endif