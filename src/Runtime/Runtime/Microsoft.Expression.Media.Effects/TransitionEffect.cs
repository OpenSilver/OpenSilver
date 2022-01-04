﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See THIRD-PARTY-NOTICES file in the project root for full license information.

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


using System.Windows.Media.Effects;

#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.Expression.Media.Effects
{
    /// <summary>
    /// Defines a transition effect shader that transitions from one visual to another visual
    /// using an interpolated value between 0 and 1.
    /// </summary>
    public abstract class TransitionEffect : ShaderEffect
    {
#region Dependency Properties
        /// <summary>
        /// Brush-valued properties that turn into sampler-properties in the shader.
        /// Represents the image present in the final state of the transition.
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransitionEffect), 0, SamplingMode.NearestNeighbor);

        /// <summary>
        /// Brush-valued properties that turn into sampler-properties in the shader.
        /// Represents the image present in the initial state of the transition.
        /// </summary>
        public static readonly DependencyProperty OldImageProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("OldImage", typeof(TransitionEffect), 1, SamplingMode.NearestNeighbor);

        /// <summary>
        /// A Dependency property as the backing store for Progress.
        /// Also used to represent the state of a transition from start to finish (range between 0 and 1).
        /// </summary>
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(TransitionEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

#endregion

#region Constructors

        /// <summary>
        /// Creates a modifiable clone (deep copy) of the <see cref="T:TransitionEffect"/> using its current values.
        /// </summary>
        public TransitionEffect CloneCurrentValue()
        {
            // WPF-only
            //return (TransitionEffect)base.CloneCurrentValue();
            return DeepCopy();
        }

        /// <summary>
        /// Makes a deep copy of the transition effect. Implements CloneCurrentValue in Silverlight.
        /// </summary>
        /// <returns>A clone of current instance of transition effect.</returns>
        protected abstract TransitionEffect DeepCopy();

        /// <summary>
        /// Updates the shader's variables to the default values.
        /// </summary>
        protected TransitionEffect()
        {
            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(OldImageProperty);
            UpdateShaderValue(ProgressProperty);
        }

#endregion

        /// <summary>
        /// Gets or sets the Input variable within the shader.
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OldImage variable within the shader.
        /// </summary>
        public Brush OldImage
        {
            get { return (Brush)GetValue(OldImageProperty); }
            set { SetValue(OldImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Progress variable within the shader.
        /// </summary>
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
    }
}
