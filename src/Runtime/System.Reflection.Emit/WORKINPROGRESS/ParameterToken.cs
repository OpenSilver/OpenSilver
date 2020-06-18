#if WORKINPROGRESS
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	//
	// Summary:
	//     The ParameterToken struct is an opaque representation of the token returned by
	//     the metadata to represent a parameter.
	[ComVisible(true)]
	public struct ParameterToken
	{
		//
		// Summary:
		//     The default ParameterToken with System.Reflection.Emit.ParameterToken.Token value
		//     0.
		public static readonly ParameterToken Empty;

		//
		// Summary:
		//     Retrieves the metadata token for this parameter.
		//
		// Returns:
		//     Read-only. Retrieves the metadata token for this parameter.
		public int Token { get; }

		//
		// Summary:
		//     Checks if the given object is an instance of ParameterToken and is equal to this
		//     instance.
		//
		// Parameters:
		//   obj:
		//     The object to compare to this object.
		//
		// Returns:
		//     true if obj is an instance of ParameterToken and equals the current instance;
		//     otherwise, false.
		public override bool Equals(object obj)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Indicates whether the current instance is equal to the specified System.Reflection.Emit.ParameterToken.
		//
		// Parameters:
		//   obj:
		//     The System.Reflection.Emit.ParameterToken to compare to the current instance.
		//
		// Returns:
		//     true if the value of obj is equal to the value of the current instance; otherwise,
		//     false.
		public bool Equals(ParameterToken obj)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Generates the hash code for this parameter.
		//
		// Returns:
		//     Returns the hash code for this parameter.
		public override int GetHashCode()
		{
			return default(int);
		}

		//
		// Summary:
		//     Indicates whether two System.Reflection.Emit.ParameterToken structures are equal.
		//
		// Parameters:
		//   a:
		//     The System.Reflection.Emit.ParameterToken to compare to b.
		//
		//   b:
		//     The System.Reflection.Emit.ParameterToken to compare to a.
		//
		// Returns:
		//     true if a is equal to b; otherwise, false.
		public static bool operator ==(ParameterToken a, ParameterToken b)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Indicates whether two System.Reflection.Emit.ParameterToken structures are not
		//     equal.
		//
		// Parameters:
		//   a:
		//     The System.Reflection.Emit.ParameterToken to compare to b.
		//
		//   b:
		//     The System.Reflection.Emit.ParameterToken to compare to a.
		//
		// Returns:
		//     true if a is not equal to b; otherwise, false.
		public static bool operator !=(ParameterToken a, ParameterToken b)
		{
			return default(bool);
		}
	}
}
#endif