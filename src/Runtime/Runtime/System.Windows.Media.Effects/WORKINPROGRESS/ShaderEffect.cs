
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

namespace System.Windows.Media.Effects
{
	[OpenSilver.NotImplemented]
    public abstract class ShaderEffect : Effect
    {
		[OpenSilver.NotImplemented]
        protected static readonly DependencyProperty PixelShaderProperty = DependencyProperty.Register(nameof(PixelShader), typeof(PixelShader), typeof(ShaderEffect), new PropertyMetadata());
        

		[OpenSilver.NotImplemented]
        protected PixelShader PixelShader
        {
            get { return (PixelShader)GetValue(PixelShaderProperty); }
            set { SetValueInternal(PixelShaderProperty, value); }
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
            return DependencyProperty.Register(dpName, typeof(Brush), ownerType, new PropertyMetadata());
        }
		[OpenSilver.NotImplemented]
        protected static DependencyProperty RegisterPixelShaderSamplerProperty(string @dpName, Type @ownerType, int @samplerRegisterIndex, SamplingMode @samplingMode)
        {
            return DependencyProperty.Register(dpName, typeof(Brush), ownerType, new PropertyMetadata());
        }
    }
}
