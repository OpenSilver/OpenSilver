

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


#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Media.Effects
{
	[OpenSilver.NotImplemented]
    public abstract partial class ShaderEffect : Effect
    {
		[OpenSilver.NotImplemented]
        protected static readonly DependencyProperty PixelShaderProperty = DependencyProperty.Register(nameof(PixelShader), typeof(PixelShader), typeof(ShaderEffect), new PropertyMetadata());
        

		[OpenSilver.NotImplemented]
        protected PixelShader PixelShader
        {
            get { return (PixelShader)this.GetValue(ShaderEffect.PixelShaderProperty); }
            set { this.SetValue(ShaderEffect.PixelShaderProperty, value); }
        }

		[OpenSilver.NotImplemented]
        protected double PaddingBottom { get; set; }
        

		[OpenSilver.NotImplemented]
        protected ShaderEffect()
        {
        }
		[OpenSilver.NotImplemented]
        protected static PropertyChangedCallback PixelShaderConstantCallback(int @register)
        {
            return null;
        }
		[OpenSilver.NotImplemented]
        protected void UpdateShaderValue(DependencyProperty @dp)
        {
        }
		[OpenSilver.NotImplemented]
        protected static DependencyProperty RegisterPixelShaderSamplerProperty(string @dpName, Type @ownerType, int @samplerRegisterIndex)
        {
            return null;
        }
		[OpenSilver.NotImplemented]
        protected static DependencyProperty RegisterPixelShaderSamplerProperty(string @dpName, Type @ownerType, int @samplerRegisterIndex, SamplingMode @samplingMode)
        {
            return null;
        }

        [OpenSilver.NotImplemented]
        public ShaderEffect CloneCurrentValue()
        {
            return default(ShaderEffect);
        }
    }
}
