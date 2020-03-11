

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Converts a uniform resource identifier (URI) into a new URI based on the
    /// rules of a matching object specified in a collection of mapping objects.
    /// </summary>
    [ContentProperty("UriMappings")]
    public sealed partial class UriMapper : UriMapperBase
    {
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Navigation.UriMapper class.
        ///// </summary>
        //public UriMapper();

        List<UriMapping> _uriMappings = new List<UriMapping>(); //I don't see the point of this class if we do not set some UriMappings on it so might as well initialize it directly when constructing it.
        /// <summary>
        /// Gets a collection of objects that are used to convert a uniform resource
        /// identifier (URI) into a new URI.
        /// </summary>
        public List<UriMapping> UriMappings { get { return _uriMappings; } }

       
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.Windows.Navigation.UriMapper.UriMappings property is null.
        /// <summary>
        /// Converts a specified uniform resource identifier (URI) into a new URI based
        /// on the rules of a matching object in the System.Windows.Navigation.UriMapper.UriMappings
        /// collection.
        /// </summary>
        /// <param name="uri">Original URI value to be converted to a new URI.</param>
        /// <returns>
        /// A URI to use for handling the request instead of the value of the uri parameter.
        /// If no object in the System.Windows.Navigation.UriMapper.UriMappings collection
        /// matches uri, the original value for uri is returned.
        /// </returns>
        public override Uri MapUri(Uri uri)
        {
            //I'm assuming the order in which the UriMappings are defined should be their order of priority so as soon as we find one that works, we stop:
            foreach(UriMapping mapping in _uriMappings)
            {
                Uri mappedUri = mapping.MapUri(uri);
                if(mappedUri != null)
                    return mappedUri;
            }
            return uri;
        }
    }
}
