
using System.Security;

namespace System.Reflection.Emit
{
    /// <summary>
    /// Generates Microsoft intermediate language (MSIL) instructions.
    /// </summary>
	[OpenSilver.NotImplemented]
    public partial class ILGenerator
    {
        /// <summary>
        /// Puts the specified instruction onto the stream of instructions.
        /// </summary>
        /// <param name="opcode">
        /// The Microsoft Intermediate Language (MSIL) instruction to be put onto the stream.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode)
        {
        }

        /// <summary>
        /// Puts the specified instruction and metadata token for the specified field onto
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.
        /// </param>
        /// <param name="field">
        /// A field that is the target of opcode.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, FieldInfo field)
        {
        }

        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate
        /// language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be put onto the stream.
        /// </param>
        /// <param name="arg">
        /// The Single argument pushed onto the stream immediately after the instruction.
        /// </param>
        [SecuritySafeCritical]
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, float arg)
        {
        }

        /// <summary>
        /// Puts the specified instruction and character argument onto the Microsoft intermediate
        /// language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be put onto the stream.
        /// </param>
        /// <param name="arg">
        /// The character argument pushed onto the stream immediately after the instruction.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, byte arg)
        {
        }
  
        /// <summary>
        /// Puts the specified instruction and character argument onto the Microsoft intermediate
        /// language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be put onto the stream.
        /// </param>
        /// <param name="arg">
        /// The character argument pushed onto the stream immediately after the instruction.
        /// </param>
        [CLSCompliant(false)]
		[OpenSilver.NotImplemented]
        public void Emit(OpCode opcode, sbyte arg)
        {
        }

        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate
        /// language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.
        /// </param>
        /// <param name="arg">
        /// The Int argument pushed onto the stream immediately after the instruction.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, short arg)
        {
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        /// stream followed by the metadata token for the given method.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.
        /// </param>
        /// <param name="meth">
        /// A method that is the target of opcode.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// meth is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// meth is a generic method for which the <see cref="MethodBase.IsGenericMethodDefinition"/>
        /// property is false.
        /// </exception>
        [SecuritySafeCritical]
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, MethodInfo meth)
        {
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        /// stream followed by the metadata token for the given type.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be put onto the stream.
        /// </param>
        /// <param name="cls">
        /// The type that is the target of opcode.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// cls is null.
        /// </exception>
        [SecuritySafeCritical]
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, Type cls)
        {
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        /// stream followed by the index of the given local variable.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.
        /// </param>
        /// <param name="local">
        /// A local variable.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The parent method of the local parameter does not match the method associated
        /// with this <see cref="ILGenerator"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// local is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// opcode is a single-byte instruction, and local represents a local variable with
        /// an index greater than <see cref="Byte.MaxValue"/>.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, LocalBuilder local)
        {
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        /// stream and leaves space to include a label when fixes are done.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.
        /// </param>
        /// <param name="label">
        /// The label to branch to.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, Label label)
        {
        }

        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        /// <param name="localType">
        /// The type of the local variable.
        /// </param>
        /// <returns>
        /// The declared local variable.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// localType is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The containing type has been created by the <see cref="TypeBuilder.CreateType"/>
        /// method.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual LocalBuilder DeclareLocal(Type localType)
        {
            return new LocalBuilder();
        }

        /// <summary>
        /// Declares a new label.
        /// </summary>
        /// <returns>
        /// A new label that can be used as a token for branching.
        /// </returns>
		[OpenSilver.NotImplemented]
        public virtual Label DefineLabel()
        {
            return new Label();
        }

        /// <summary>
        /// Marks the Microsoft intermediate language (MSIL) stream's current position with
        /// the given label.
        /// </summary>
        /// <param name="loc">
        /// The label for which to set an index.
        /// </param>
        /// <exception cref="ArgumentException">
        /// loc represents an invalid index into the label array.-or- An index for loc has
        /// already been defined.
        /// </exception>
		[OpenSilver.NotImplemented]
        public virtual void MarkLabel(Label loc)
        {
        }

        /// <summary>
        /// Puts a call or callvirt instruction onto the Microsoft intermediate language
        /// (MSIL) stream to call a varargs method.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream. Must be <see cref="OpCodes.Call"/>,
        /// <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/>.
        /// </param>
        /// <param name="methodInfo">
        /// The varargs method to be called.
        /// </param>
        /// <param name="optionalParameterTypes">
        /// The types of the optional arguments if the method is a varargs method; otherwise,
        /// null.
        /// </param>
        /// <exception cref="ArgumentException">
        /// opcode does not specify a method call.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// methodInfo is null.
        /// </exception>
        [SecuritySafeCritical]
		[OpenSilver.NotImplemented]
        public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
        }

        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        /// stream followed by the metadata token for the given string.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="str">
        /// The string that is the target of opcode.
        /// </param>
		[OpenSilver.NotImplemented]
        public virtual void Emit(OpCode opcode, string str)
        {
        }

    }
}

