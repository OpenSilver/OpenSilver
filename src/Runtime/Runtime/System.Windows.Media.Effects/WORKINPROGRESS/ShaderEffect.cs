
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

namespace System.Windows.Media.Effects;

/// <summary>
/// Provides a custom bitmap effect by using a <see cref="Effects.PixelShader"/>.
/// </summary>
[OpenSilver.NotImplemented]
public abstract class ShaderEffect : Effect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderEffect"/> class.
    /// </summary>
    protected ShaderEffect() { }

    /// <summary>
    /// Gets or sets a value that indicates the shader register to use for the partial derivatives of the texture 
    /// coordinates with respect to screen space.
    /// </summary>
    /// <returns>
    /// The index of the register that contains the partial derivatives.
    /// </returns>
    protected int DdxUvDdyUvRegisterIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets the amount by which the effect's output texture is larger than its input texture along the 
    /// bottom edge of the effect.
    /// </summary>
    /// <returns>
    /// The padding along the bottom edge of the effect.
    /// </returns>
    protected double PaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets the amount by which the effect's output texture is larger than its input texture along the 
    /// left edge.
    /// </summary>
    /// <returns>
    /// The padding along the left edge of the effect.
    /// </returns>
    protected double PaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets the amount by which the effect's output texture is larger than its input texture along the 
    /// right edge.
    /// </summary>
    /// <returns>
    /// The padding along the right edge of the effect.
    /// </returns>
    protected double PaddingRight { get; set; }

    /// <summary>
    /// Gets or sets the amount by which the effect's output texture is larger than its input texture along the 
    /// top edge.
    /// </summary>
    /// <returns>
    /// The padding along the top edge of the effect.
    /// </returns>
    protected double PaddingTop { get; set; }

    /// <summary>
    /// Identifies the <see cref="PixelShader"/> dependency property.
    /// </summary>
    protected static readonly DependencyProperty PixelShaderProperty =
        DependencyProperty.Register(
            nameof(PixelShader),
            typeof(PixelShader),
            typeof(ShaderEffect),
            null);

    /// <summary>
    /// Gets or sets the <see cref="Effects.PixelShader"/> to use for the effect.
    /// </summary>
    /// <returns>
    /// The <see cref="Effects.PixelShader"/> for the effect.
    /// </returns>
    protected PixelShader PixelShader
    {
        get => (PixelShader)GetValue(PixelShaderProperty);
        set => SetValueInternal(PixelShaderProperty, value);
    }

    /// <summary>
    /// Associates a dependency property value with a pixel shader's float constant register.
    /// </summary>
    /// <param name="register">
    /// The index of the shader register associated with the dependency property.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyChangedCallback"/> delegate that associates a dependency property and the shader 
    /// constant register specified by register.
    /// </returns>
    protected static PropertyChangedCallback PixelShaderConstantCallback(int register) => null;

    /// <summary>
    /// Associates a dependency property value with a pixel shader's sampler register.
    /// </summary>
    /// <param name="register">
    /// The index of the shader sampler associated with the dependency property.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyChangedCallback"/> delegate that associates a dependency property and the shader 
    /// sampler register specified by register.
    /// </returns>
    protected static PropertyChangedCallback PixelShaderSamplerCallback(int register) =>
        PixelShaderSamplerCallback(register, SamplingMode.Auto);

    /// <summary>
    /// Associates a dependency property value with a pixel shader's sampler register and a <see cref="SamplingMode"/>.
    /// </summary>
    /// <param name="register">
    /// The index of the shader sampler associated with the dependency property.
    /// </param>
    /// <param name="samplingMode">
    /// The <see cref="SamplingMode"/> for the shader sampler.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyChangedCallback"/> delegate that associates a dependency property and the shader 
    /// sampler register specified by register.
    /// </returns>
    protected static PropertyChangedCallback PixelShaderSamplerCallback(int register, SamplingMode samplingMode) => null;

    /// <summary>
    /// Associates a dependency property with a shader sampler register.
    /// </summary>
    /// <param name="dpName">
    /// The name of the dependency property.
    /// </param>
    /// <param name="ownerType">
    /// The type of the effect that has the dependency property.
    /// </param>
    /// <param name="samplerRegisterIndex">
    /// The index of the shader sampler associated with the dependency property.
    /// </param>
    /// <returns>
    /// A dependency property associated with the shader sampler specified by samplerRegisterIndex.
    /// </returns>
    protected static DependencyProperty RegisterPixelShaderSamplerProperty(string dpName, Type ownerType, int samplerRegisterIndex) =>
        RegisterPixelShaderSamplerProperty(dpName, ownerType, samplerRegisterIndex, SamplingMode.Auto);

    /// <summary>
    /// Associates a dependency property with a shader sampler register and a sampling mode.
    /// </summary>
    /// <param name="dpName">
    /// The name of the dependency property.
    /// </param>
    /// <param name="ownerType">
    /// The type of the effect that has the dependency property.
    /// </param>
    /// <param name="samplerRegisterIndex">
    /// The index of the shader sampler associated with the dependency property.
    /// </param>
    /// <param name="samplingMode">
    /// One of the enumeration values that specifies the sampling mode for the shader.
    /// </param>
    /// <returns>
    /// A dependency property associated with the shader sampler specified by samplerRegisterIndex.
    /// </returns>
    protected static DependencyProperty RegisterPixelShaderSamplerProperty(string dpName, Type ownerType, int samplerRegisterIndex, SamplingMode samplingMode) =>
        DependencyProperty.Register(
            dpName,
            typeof(Brush),
            ownerType,
            new PropertyMetadata(PixelShaderSamplerCallback(samplerRegisterIndex, samplingMode)));

    /// <summary>
    /// Notifies the effect that the shader constant or sampler corresponding to the specified dependency 
    /// property should be updated.
    /// </summary>
    /// <param name="dp">
    /// The dependency property to be updated.
    /// </param>
    protected void UpdateShaderValue(DependencyProperty dp) { }
}
