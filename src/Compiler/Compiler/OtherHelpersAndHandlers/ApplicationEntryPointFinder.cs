#if !BRIDGE && !CSHTML5BLAZOR
extern alias DotNetForHtml5Core;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotNetForHtml5.Compiler.Common;

namespace DotNetForHtml5.Compiler
{
    internal static class ApplicationEntryPointFinder
    {
        public static void GetFullNameOfClassThatInheritsFromApplication(string pathOfAssemblyThatContainsEntryPoint, out string applicationClassFullName, out string assemblyName, out string assemblyFullName)
        {
            //-----------------
            // Thanks to: http://www.c-sharpcorner.com/UploadFile/girish.nehte/how-to-unload-an-assembly-loaded-dynamically-using-reflection/
            //-----------------

            // Load the assembly in a new AppDomain mainly so that we are able to unload it, which is necessary when we want to delete all temporary files including this assembly.
            AppDomainSetup setupInformation = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newAppDomain = AppDomain.CreateDomain("newAppDomain", AppDomain.CurrentDomain.Evidence, setupInformation);

            //Create an instance of the inspector class in the new domain:
            //System.Runtime.Remoting.ObjectHandle obj = newAppDomain.CreateInstance(typeof(AssemblyInspectorOnOtherDomain).Assembly.FullName, typeof(AssemblyInspectorOnOtherDomain).FullName);
            string pathOfThisVeryAssembly = PathsHelper.GetPathOfThisVeryAssembly();
            System.Runtime.Remoting.ObjectHandle obj = newAppDomain.CreateInstanceFrom(pathOfThisVeryAssembly, typeof(AssemblyInspectorOnOtherDomain).FullName);

            IAssemblyInspectorOnOtherDomain inspector = (IAssemblyInspectorOnOtherDomain)obj.Unwrap(); // As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it

            // Call LoadAssembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only:
            inspector.LoadAssembly(pathOfAssemblyThatContainsEntryPoint);

            // Call the method that finds the type that inherits from Application:
            applicationClassFullName = inspector.FindApplicationClassFullName();

            // Get the assembly name and full name too:
            assemblyName = inspector.GetAssemblyName();
            assemblyFullName = inspector.GetAssemblyFullName();

            // Unload the assembly (so that we can later delete it if necessary):
            AppDomain.Unload(newAppDomain);
            GC.Collect(); // Collects all unused memory
            GC.WaitForPendingFinalizers(); // Waits until GC has finished its work
            GC.Collect();
        }


        public class AssemblyInspectorOnOtherDomain : MarshalByRefObject, IAssemblyInspectorOnOtherDomain
        {
            private Assembly _assembly;
            System.Type MyType = null;
            object inst = null;
            public override object InitializeLifetimeService()
            {
                return null;
            }
            public void LoadAssembly(string path)
            {
                _assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));
            }
            public string GetAssemblyName()
            {
                return _assembly.GetName().Name;
            }
            public string GetAssemblyFullName()
            {
                return _assembly.FullName;
            }
            public string FindApplicationClassFullName()
            {
#if BRIDGE || CSHTML5BLAZOR
                throw new NotSupportedException();
#else
                // Get the base "Application" type:
#if SILVERLIGHTCOMPATIBLEVERSION 
                Type baseApplicationType = typeof(DotNetForHtml5Core::System.Windows.Application);
#else
                Type baseApplicationType = typeof(DotNetForHtml5Core::Windows.UI.Xaml.Application);
#endif

                // Find a class that inherits from the base application type:
                Type applicationType = null;
                foreach (Type type in _assembly.GetTypes())
                {
                    if (baseApplicationType.IsAssignableFrom(type))
                    {
                        applicationType = type;
                        break;
                    }
                }

                // If no such class is found, throw an exception:
                if (applicationType == null)
                    throw new Exception("Error: the project contains no entry point. The project must contain a class that inherits from Application.");

                return applicationType.FullName;
#endif
            }
        }


        //class AssemblyReflectionOnlyLoaderIncludingDependencies
        //{
        //    // This class is used to load an assembly for reflection only, including all its dependencies.

        //    Assembly _assembly;
        //    string _assemblyPath;

        //    public AssemblyReflectionOnlyLoaderIncludingDependencies(string assemblyPath)
        //    {
        //        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ReflectionOnlyAssemblyResolve;
        //        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
        //        _assemblyPath = assemblyPath;
        //        _assembly = Assembly.ReflectionOnlyLoadFrom(_assemblyPath);
        //    }

        //    public Assembly Assembly
        //    {
        //        get
        //        {
        //            return _assembly;
        //        }
        //    }

        //    private Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        //    {
        //        AssemblyName name = new AssemblyName(args.Name);
        //        String asmToCheck = Path.GetDirectoryName(_assemblyPath) + "\\" + name.Name + ".dll";

        //        if (File.Exists(asmToCheck))
        //        {
        //            return Assembly.ReflectionOnlyLoadFrom(asmToCheck);
        //        }

        //        return Assembly.ReflectionOnlyLoad(args.Name);
        //    }
        //}
    }
}
