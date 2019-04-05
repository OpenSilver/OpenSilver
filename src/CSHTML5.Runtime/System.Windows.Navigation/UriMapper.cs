
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public sealed class UriMapper : UriMapperBase
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
