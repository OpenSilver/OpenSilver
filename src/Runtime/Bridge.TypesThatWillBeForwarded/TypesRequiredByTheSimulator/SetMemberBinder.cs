namespace System.Dynamic
{
    [Bridge.NonScriptable]
    public abstract class SetMemberBinder : DynamicMetaObjectBinder
    {
        //protected SetMemberBinder(string name, bool ignoreCase);
        //public bool IgnoreCase { get; }
        public string Name { get { throw new NotImplementedException(); } }
        //public sealed override Type ReturnType { get; }
        //public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
        //public DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value);
        //public abstract DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion);
    }
}