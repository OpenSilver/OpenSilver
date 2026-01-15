
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

using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace OpenSilver.Internal.Data;

internal static class TrivialBinderHelper
{
    public static DynamicMetaObject ThrowExpression(string message, Type returnType) =>
        new DynamicMetaObject(
            Expression.Throw(
                Expression.New(
                    typeof(InvalidOperationException).GetConstructor([typeof(string)]),
                    Expression.Constant(message)),
                returnType),
            BindingRestrictions.Empty);

    public static DynamicMetaObject ThrowMissingMemberExpression(object target, string name, Type returnType) =>
        ThrowExpression(MissingMemberErrorString(target, name), returnType);

    private static string MissingMemberErrorString(object target, string name) =>
        string.Format(Strings.PropertyPathNoProperty, target, name);
}