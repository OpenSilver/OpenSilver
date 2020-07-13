using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace System.Dynamic
{
    [Bridge.NonScriptable]
    public abstract class DynamicMetaObjectBinder : CallSiteBinder
    {
        //protected DynamicMetaObjectBinder();
        //public virtual Type ReturnType { get; }
        //public abstract DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
        //public sealed override Expression Bind(object[] args, ReadOnlyCollection<ParameterExpression> parameters, LabelTarget returnLabel);
        //public DynamicMetaObject Defer(params DynamicMetaObject[] args);
        //public DynamicMetaObject Defer(DynamicMetaObject target, params DynamicMetaObject[] args);
        //public Expression GetUpdateExpression(Type type);
    }
}