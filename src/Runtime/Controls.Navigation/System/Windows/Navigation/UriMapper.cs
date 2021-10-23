

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



using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Markup;
using Resource = OpenSilver.Internal.Controls.Navigation.Resource;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Default UriMapperBase implementation that uses a List of UriMapping
    /// objects to map and transform URI values.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ContentProperty("UriMappings")]
    public sealed class UriMapper : UriMapperBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UriMapper()
        {
            this.UriMappings = new Collection<UriMapping>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of UriMapping objects.
        /// </summary>
        public Collection<UriMapping> UriMappings
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Maps a given URI and returns a mapped URI.
        /// </summary>
        /// <param name="uri">Original URI value to be mapped to a new URI.</param>
        /// <returns>A URI derived from the <paramref name="uri"/> parameter.</returns>
        public override Uri MapUri(Uri uri)
        {
            var mappings = this.UriMappings;

            if (mappings == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture,
                                  Resource.UriMapper_MustNotHaveANullUriMappingCollection,
                                  "UriMapper",
                                  "UriMappings"));
            }

            Guard.ArgumentNotNull(uri, "uri");

            Uri mappedUri = null;

            foreach (var mapping in mappings)
            {
                mappedUri = mapping.MapUri(uri);
                if (mappedUri != null)
                {
                    return mappedUri;
                }
            }

            // If no mapping was able to process the uri, return the original
            return uri;
        }

        #endregion Methods
    }
}
