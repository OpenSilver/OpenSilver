
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes how content is resized to fill its allocated space.
    /// </summary>
    public enum Stretch
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        None = 0,

        /// <summary>
        /// The content is resized to fill the destination dimensions. The aspect ratio is not preserved.
        /// </summary>
        Fill = 1,

        /// <summary>
        /// The content is resized to fit in the destination dimensions while it preserves its native aspect ratio.
        /// </summary>
        Uniform = 2,

        /// <summary>
        /// The content is resized to fill the destination dimensions while it preserves
        /// its native aspect ratio. If the aspect ratio of the destination rectangle
        /// differs from the source, the source content is clipped to fit in the destination
        /// dimensions.
        /// </summary>
        UniformToFill = 3,
    }
}