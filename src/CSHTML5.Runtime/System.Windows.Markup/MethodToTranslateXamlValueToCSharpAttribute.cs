

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


namespace System.Windows.Markup
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public partial class MethodToTranslateXamlValueToCSharpAttribute : Attribute
    {
        public MethodToTranslateXamlValueToCSharpAttribute() { }

        public MethodToTranslateXamlValueToCSharpAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; set; }
    }
}
