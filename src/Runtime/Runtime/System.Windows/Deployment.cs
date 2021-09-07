

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
#if !MIGRATION
using Windows.UI.Xaml;
#endif
namespace System.Windows
{
    /// <summary>
    /// Provides application part and localization information in the application
    /// manifest when deploying a Silverlight-based application.
    /// </summary>
    public sealed partial class Deployment : DependencyObject
    {
        private Deployment() { } //Note: There is normally a public parameterless constructor but I'm assuming it works like the Current property so we keep this private for now.
        static Deployment _deployment = null;

        /// <summary>
        /// Gets the current System.Windows.Deployment object.
        /// </summary>
        public static Deployment Current
        {
            get
            {
                if (_deployment == null)
                {
                    _deployment = new Deployment();
                }
                return _deployment;
            }
        }

		[OpenSilver.NotImplemented]
        public OutOfBrowserSettings OutOfBrowserSettings { get; private set; }

		[OpenSilver.NotImplemented]
        public AssemblyPartCollection Parts { get; private set; }

        #region not implemented

        //// Summary:
        ////     Identifies the System.Windows.Deployment.EntryPointAssembly dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.EntryPointAssembly dependency
        ////     property.
        //public static readonly DependencyProperty EntryPointAssemblyProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.EntryPointType dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.EntryPointType dependency
        ////     property.
        //public static readonly DependencyProperty EntryPointTypeProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.ExternalCallersFromCrossDomain dependency
        ////     property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.ExternalCallersFromCrossDomain
        ////     dependency property.
        //public static readonly DependencyProperty ExternalCallersFromCrossDomainProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.ExternalParts dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.ExternalParts dependency
        ////     property.
        //public static readonly DependencyProperty ExternalPartsProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.InBrowserSettings dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.InBrowserSettings dependency
        ////     property.
        //public static readonly DependencyProperty InBrowserSettingsProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.OutOfBrowserSettings dependency
        ////     property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.OutOfBrowserSettings dependency
        ////     property.
        //public static readonly DependencyProperty OutOfBrowserSettingsProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.Parts dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.Parts dependency property.
        //public static readonly DependencyProperty PartsProperty;
        ////
        //// Summary:
        ////     Identifies the System.Windows.Deployment.RuntimeVersion dependency property.
        ////
        //// Returns:
        ////     The identifier for the System.Windows.Deployment.RuntimeVersion dependency
        ////     property.
        //public static readonly DependencyProperty RuntimeVersionProperty;

        //// Summary:
        ////     Initializes a new instance of the System.Windows.Deployment class.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The System.Windows.Deployment.Current property has already been initialized.
        //public Deployment();


        ////
        //// Summary:
        ////     Gets a string name that identifies which part in the System.Windows.Deployment
        ////     is the entry point assembly.
        ////
        //// Returns:
        ////     A string that names the assembly that should be used as the entry point assembly.
        ////     This is expected to be the name of one of the assemblies you specified as
        ////     an System.Windows.AssemblyPart.
        //public string EntryPointAssembly { get; }
        ////
        //// Summary:
        ////     Gets a string that identifies the namespace and type name of the class that
        ////     contains the System.Windows.Application entry point for your application.
        ////
        //// Returns:
        ////     The namespace and type name of the class that contains the System.Windows.Application
        ////     entry point.
        //public string EntryPointType { get; }
        ////
        //// Summary:
        ////     Gets a value that indicates the level of access that cross-domain callers
        ////     have to the Silverlight-based application in this deployment.
        ////
        //// Returns:
        ////     A value that indicates the access level of cross-domain callers.
        //public CrossDomainAccess ExternalCallersFromCrossDomain { get; }
        ////
        //// Summary:
        ////     Gets a collection of System.Windows.ExternalPart instances that represent
        ////     the external assemblies required by the application.
        ////
        //// Returns:
        ////     The collection of external assembly parts. The default is an empty collection.
        //public ExternalPartCollection ExternalParts { get; }
        ////
        //// Summary:
        ////     Gets an object that contains information about the application that is used
        ////     for in-browser support.
        ////
        //// Returns:
        ////     An object that contains information about the application that is used for
        ////     in-browser support.
        //public InBrowserSettings InBrowserSettings { get; }
        ////
        //// Summary:
        ////     Gets an object that contains information about the application that is used
        ////     for out-of-browser support.
        ////
        //// Returns:
        ////     Information about the application that is used for out-of-browser support.
        //public OutOfBrowserSettings OutOfBrowserSettings { get; }
        ////
        //// Summary:
        ////     Gets a collection of assembly parts that are included in the deployment.
        ////
        //// Returns:
        ////     The collection of assembly parts. The default is an empty collection.
        //public AssemblyPartCollection Parts { get; }
        ////
        //// Summary:
        ////     Gets the Silverlight runtime version that this deployment supports.
        ////
        //// Returns:
        ////     The Silverlight runtime version that this deployment supports.
        //public string RuntimeVersion { get; }

        //// Summary:
        ////     [SECURITY CRITICAL] Gets a value that represents a unique ID for an out-of-browser
        ////     application.
        ////
        //// Parameters:
        ////   AppUri:
        ////     The absolute URI of the application's .xap file on its host server.
        ////
        ////   xapLocationStr:
        ////     When this method returns, contains the local path to the .xap file in the
        ////     offline application cache. This parameter is passed uninitialized.
        ////
        //// Returns:
        ////     A unique ID for the out-of-browser application.
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[SecurityCritical]
        //public static string GetAppIdForUri(Uri AppUri, out string xapLocationStr);
        ////
        //// Summary:
        ////     [SECURITY CRITICAL] Enables a native Silverlight host, such as Expression
        ////     Blend or Visual Studio, to instruct Silverlight to register an assembly that
        ////     the Silverlight host has separately loaded into the host-managed application
        ////     domain in which a Silverlight application is running.
        ////
        //// Parameters:
        ////   assembly:
        ////     The assembly that the Silverlight host has separately loaded.
        //[SecurityCritical]
        //public static void RegisterAssembly(Assembly assembly);
        ////
        //// Summary:
        ////     [SECURITY CRITICAL] Allows a native host of the Silverlight plug-in to specify
        ////     the current System.Windows.Application object of the running Silverlight
        ////     application.
        ////
        //// Parameters:
        ////   application:
        ////     The System.Windows.Application object that the native host is setting as
        ////     the current System.Windows.Application.
        //[SecurityCritical]
        //public static void SetCurrentApplication(Application application);

        #endregion
    }
}

