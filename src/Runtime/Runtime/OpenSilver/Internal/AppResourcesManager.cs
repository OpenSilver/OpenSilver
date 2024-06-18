
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
using System.IO;
using System.Reflection;
using System.Resources;
using OpenSilver.Runtime.CompilerServices;

namespace OpenSilver.Internal;

internal static class AppResourcesManager
{
    public const string Component = ";component/";
    public const string PackApp = "pack://application:,,,/";
    public const string MsAppx = "ms-appx:/";
    public const string Http = "http://";
    public const string Https = "https://";

    private static Dictionary<string, AssemblyResourceManager> _resourceManagers;

    private static Dictionary<string, AssemblyResourceManager> ResourceManagers
    {
        get
        {
            EnsureAssembliesLoaded();
            return _resourceManagers;
        }
    }

    public static Stream GetResourceStream(string uri)
    {
        if (IsComponentUri(uri))
        {
            string assemblyName = ExtractAssemblyNameFromComponentUri(uri);
            if (ResourceManagers.TryGetValue(assemblyName, out AssemblyResourceManager resourceManager))
            {
                string resourcePart = ExtractResourcePartFromComponentUri(uri);
                return resourceManager.GetStream(resourcePart.ToLowerInvariant());
            }
        }

        return null;
    }

    private static void EnsureAssembliesLoaded()
    {
        if (_resourceManagers is not null)
        {
            return;
        }

        _resourceManagers = new Dictionary<string, AssemblyResourceManager>(StringComparer.OrdinalIgnoreCase);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (IsOpenSilverAssembly(assembly))
            {
                CacheAssembly(assembly);
            }
        }

        AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoad);
    }

    private static void CacheAssembly(Assembly assembly) =>
        _resourceManagers[assembly.GetName().Name] = new(assembly);

    private static bool IsOpenSilverAssembly(Assembly assembly) =>
        assembly.GetCustomAttribute<OpenSilverAssemblyAttribute>() is not null;

    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs e)
    {
        if (IsOpenSilverAssembly(e.LoadedAssembly))
        {
            CacheAssembly(e.LoadedAssembly);
        }
    }

    internal static bool IsComponentUri(string uri)
    {
        int index = uri.IndexOf(';');
        if (index > -1)
        {
            return uri.AsSpan(index).StartsWith(Component.AsSpan(), StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    internal static string ExtractAssemblyNameFromComponentUri(string uri)
    {
        int offset = uri[0] == '/' ? 1 : 0;
        return uri.Substring(offset, uri.IndexOf(';') - offset);
    }

    internal static string ExtractResourcePartFromComponentUri(string uri)
    {
        int offset = uri[0] == '/' ? 1 : 0;
        return FormatResourcePart(uri.Substring(uri.IndexOf('/', offset) + 1));
    }

    private static string FormatResourcePart(string part) =>
        new Uri(new Uri("http://foo/"), part.Replace("#", "%23")).GetComponents(UriComponents.Path, UriFormat.UriEscaped);

    internal static bool IsMsAppxUri(string uri) => uri.StartsWith(MsAppx, StringComparison.OrdinalIgnoreCase);

    internal static void ExtractPartsFromMsAppxUri(string uri, out string assemblyName, out string resourcePart)
    {
        ReadOnlySpan<char> chars = uri.AsSpan(MsAppx.Length).TrimStart('/');

        int index = chars.IndexOfAny('/', '\\');
        if (index > -1)
        {
            assemblyName = chars.Slice(0, index).ToString();
            if (ResourceManagers.ContainsKey(assemblyName))
            {
                resourcePart = FormatResourcePart(chars.Slice(index + 1).ToString());
                return;
            }
        }

        assemblyName = null;
        resourcePart = FormatResourcePart(chars.ToString());
    }

    internal static bool IsPackUri(string uri) => uri.StartsWith(PackApp, StringComparison.OrdinalIgnoreCase);

    internal static void ExtractPartsFromPackUri(string uri, out string assemblyName, out string resourcePart)
    {
        ReadOnlySpan<char> chars = uri.AsSpan(PackApp.Length);

        int index = chars.IndexOfAny('/', '\\');
        if (index > -1)
        {
            assemblyName = chars.Slice(0, index).ToString();
            if (ResourceManagers.ContainsKey(assemblyName))
            {
                resourcePart = FormatResourcePart(chars.Slice(index + 1).ToString());
                return;
            }
        }

        assemblyName = null;
        resourcePart = FormatResourcePart(chars.ToString());
    }

    internal static bool IsHttpUri(string uri) => uri.StartsWith(Http, StringComparison.OrdinalIgnoreCase);

    internal static bool IsHttpsUri(string uri) => uri.StartsWith(Https, StringComparison.OrdinalIgnoreCase);

    private sealed class AssemblyResourceManager
    {
        public readonly Assembly _assembly;
        private ResourceManager _resourceManager;

        public AssemblyResourceManager(Assembly assembly)
        {
            _assembly = assembly;
        }

        public Stream GetStream(string name)
        {
            _resourceManager ??= new ResourceManager($"{_assembly.GetName().Name}.g", _assembly);
            try
            {
                return _resourceManager.GetStream(name);
            }
            catch
            {
                return null;
            }
        }
    }
}
