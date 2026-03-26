
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
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;

internal static class BuiltInValueConverters
{
    private static XamlValueConverter<TypeConverter> _string;
    private static XamlValueConverter<TypeConverter> _object;

    internal static XamlValueConverter<TypeConverter> String =>
        _string ??= new BuiltInValueConverter<TypeConverter>(typeof(StringConverter), () => new StringConverter());

    internal static XamlValueConverter<TypeConverter> Object =>
        _object ??= new XamlValueConverter<TypeConverter>(null, XamlLanguage.Object);

    private sealed class BuiltInValueConverter<TConverterBase> : XamlValueConverter<TConverterBase>
        where TConverterBase : class
    {
        private readonly Func<TConverterBase> _factory;

        internal BuiltInValueConverter(Type converterType, Func<TConverterBase> factory)
            : base(converterType, null)
        {
            _factory = factory;
        }

        protected override TConverterBase CreateInstance() => _factory.Invoke();
    }
}