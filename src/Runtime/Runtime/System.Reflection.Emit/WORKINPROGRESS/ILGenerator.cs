#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Reflection.Emit
{
    public partial class ILGenerator
    {
        public virtual void Emit(OpCode opcode)
        {

        }

        public virtual void Emit(OpCode opcode, FieldInfo field)
        {

        }

        public virtual void Emit(OpCode opcode, MethodInfo field)
        {

        }

        public virtual void Emit(OpCode opcode, Type cls)
        {

        }

        public virtual void Emit(OpCode opcode, LocalBuilder local)
        {

        }

        public virtual void Emit(OpCode opcode, Label label)
        {

        }

        public virtual LocalBuilder DeclareLocal(Type localType)
        {
            return null;
        }

        public virtual Label DefineLabel()
        {
            return new Label();
        }

        public virtual void MarkLabel(Label loc)
        {

        }

        public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {

        }

        public virtual void Emit(OpCode opcode, string str)
        {

        }

    }
}

#endif