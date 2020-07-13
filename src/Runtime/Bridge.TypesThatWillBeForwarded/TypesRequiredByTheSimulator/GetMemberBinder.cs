namespace System.Dynamic
{
    [Bridge.NonScriptable]
    public class GetMemberBinder : DynamicMetaObjectBinder
    {
        //protected GetMemberBinder(string name, bool ignoreCase) { }

        //public bool IgnoreCase { get { throw new NotImplementedException(); } }

        public string Name { get { throw new NotImplementedException(); } }

        //public override sealed Type ReturnType { get; }

        //public override sealed DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args) { throw new NotImplementedException(); }

        //public DynamicMetaObject FallbackGetMember(DynamicMetaObject target) { throw new NotImplementedException(); }

        //public abstract DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion);
    }
}
