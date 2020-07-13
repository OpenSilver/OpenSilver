using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Dynamic
{
    [Bridge.NonScriptable]
    public class DynamicObject : IDynamicMetaObjectProvider
    {
        protected DynamicObject() { throw new NotImplementedException(); }

        public virtual IEnumerable<string> GetDynamicMemberNames() { throw new NotImplementedException(); }

        public virtual DynamicMetaObject GetMetaObject(Expression parameter) { throw new NotImplementedException(); }

        //public virtual bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) { throw new NotImplementedException(); }

        //public virtual bool TryConvert(ConvertBinder binder, out object result) { throw new NotImplementedException(); }

        //public virtual bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result) { throw new NotImplementedException(); }

        //public virtual bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) { throw new NotImplementedException(); }

        //public virtual bool TryDeleteMember(DeleteMemberBinder binder) { throw new NotImplementedException(); }

        //public virtual bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) { throw new NotImplementedException(); }

        public virtual bool TryGetMember(GetMemberBinder binder, out object result) { throw new NotImplementedException(); }

        //public virtual bool TryInvoke(InvokeBinder binder, object[] args, out object result) { throw new NotImplementedException(); }

        public virtual bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result){ throw new NotImplementedException(); }

        //public virtual bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) { throw new NotImplementedException(); }

        public virtual bool TrySetMember(SetMemberBinder binder, object value){ throw new NotImplementedException(); }

        //public virtual bool TryUnaryOperation(UnaryOperationBinder binder, out object result) { throw new NotImplementedException(); }
    }
}