namespace System.Dynamic
{
    [Bridge.NonScriptable]
    public abstract class InvokeMemberBinder : DynamicMetaObjectBinder
    {
        //protected InvokeMemberBinder(string name, bool ignoreCase, CallInfo callInfo);
        //public CallInfo CallInfo { get; }
        //public bool IgnoreCase { get; }
        public string Name { get { throw new NotImplementedException(); } }
        //public sealed override Type ReturnType { get; }
        //public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
        //public abstract DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);
        //public DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args);
        //public abstract DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);
    }
}