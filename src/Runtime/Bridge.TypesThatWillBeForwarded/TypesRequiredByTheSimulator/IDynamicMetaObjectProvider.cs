using System.Linq.Expressions;

namespace System.Dynamic
{
    public interface IDynamicMetaObjectProvider
    {
        DynamicMetaObject GetMetaObject(Expression parameter);
    }
}