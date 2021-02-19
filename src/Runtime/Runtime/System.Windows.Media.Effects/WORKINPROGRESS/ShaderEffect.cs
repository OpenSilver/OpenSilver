#if WORKINPROGRESS
#if MIGRATION
using System.Windows;
using System;

namespace System.Windows.Media.Effects
{
    public abstract partial class ShaderEffect : Effect
    {
        protected static readonly DependencyProperty PixelShaderProperty = DependencyProperty.Register(nameof(PixelShader), typeof(PixelShader), typeof(ShaderEffect), new PropertyMetadata());
        

        protected PixelShader PixelShader
        {
            get { return (PixelShader)this.GetValue(ShaderEffect.PixelShaderProperty); }
            set { this.SetValue(ShaderEffect.PixelShaderProperty, value); }
        }

        protected double PaddingBottom { get; set; }
        

        protected ShaderEffect()
        {
        }
        protected static PropertyChangedCallback PixelShaderConstantCallback(int @register)
        {
            return null;
        }
        protected void UpdateShaderValue(DependencyProperty @dp)
        {
        }
        protected static DependencyProperty RegisterPixelShaderSamplerProperty(string @dpName, Type @ownerType, int @samplerRegisterIndex)
        {
            return null;
        }
        protected static DependencyProperty RegisterPixelShaderSamplerProperty(string @dpName, Type @ownerType, int @samplerRegisterIndex, SamplingMode @samplingMode)
        {
            return null;
        }
        

    }
}
#endif
#endif