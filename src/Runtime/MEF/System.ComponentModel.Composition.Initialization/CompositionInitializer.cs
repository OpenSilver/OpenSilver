
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

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Reflection;
using OpenSilver.Runtime.CompilerServices;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Provides static access to methods for parts to satisfy imports.
    /// </summary>
    public static class CompositionInitializer
    {
        /// <summary>
        /// Fills the imports of the specified attributed part.
        /// </summary>
        /// <param name="attributedPart">
        /// The attributed part to fill the imports of.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="attributedPart" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributedPart" /> contains exports.
        /// </exception>
        /// <exception cref="ChangeRejectedException">
        /// One or more of the imports of <paramref name="attributedPart" /> could not be satisfied.
        /// </exception>
        /// <exception cref="CompositionException">
        /// One or more of the imports of <paramref name="attributedPart" /> caused a composition error.
        /// </exception>
        public static void SatisfyImports(object attributedPart)
        {
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }

            SatisfyImports(AttributedModelServices.CreatePart(attributedPart));
        }

        /// <summary>
        /// Fills the imports of the specified part.
        /// </summary>
        /// <param name="part">
        /// The part to fill the imports of.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="part" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="part" /> contains exports.
        /// </exception>
        /// <exception cref="ChangeRejectedException">
        /// One or more of the imports of <paramref name="part" /> could not be satisfied.
        /// </exception>
        /// <exception cref="CompositionException">
        /// One or more of the imports of <paramref name="part" /> caused a composition error.
        /// </exception>
        public static void SatisfyImports(ComposablePart part)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            var batch = new CompositionBatch();
            batch.AddPart(part);
            if (part.ExportDefinitions.Any())
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeHasExports, part.ToString()), nameof(part));
            }

            CompositionHost.TryGetOrCreateContainer(CreateContainer, out CompositionContainer globalContainer);
            globalContainer.Compose(batch);
        }

        private static CompositionContainer CreateContainer() =>
            new(new AggregateCatalog(GetAssemblies().Select(asm => new AssemblyCatalog(asm))));

        private static IEnumerable<Assembly> GetAssemblies()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetCustomAttribute<OpenSilverAssemblyAttribute>() is not null)
                {
                    yield return asm;
                }
            }
        }
    }
}
