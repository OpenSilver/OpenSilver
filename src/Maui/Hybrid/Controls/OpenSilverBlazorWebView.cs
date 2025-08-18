
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

using CSHTML5.Internal;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace OpenSilver.MauiHybrid.Controls
{
    public class OpenSilverBlazorWebView : BlazorWebView
    {
        class InMemoryFileInfo : IFileInfo
        {
            private readonly Stream _stream;
            private readonly string _name;

            public InMemoryFileInfo(string name, Stream stream)
            {
                _name = name;
                _stream = stream;
            }

            public bool Exists => true;

            public long Length => _stream.Length;

            public string? PhysicalPath => null;

            public string Name => _name;

            public DateTimeOffset LastModified => DateTimeOffset.Now;

            public bool IsDirectory => false;

            public Stream CreateReadStream()
            {
                return _stream;
            }
        }

        class FallbackResourceFileProvider : IFileProvider
        {
            private readonly IFileProvider _fileProvider;

            public FallbackResourceFileProvider(IFileProvider fileProvider)
            {
                _fileProvider = fileProvider;
            }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                return _fileProvider.GetDirectoryContents(subpath);
            }

            private static IFileInfo GetResourceFileInfo(string resourceString, IFileInfo fallback)
            {
                var prefix = StartupAssemblyInfo.OutputResourcesPath;

                if (string.IsNullOrWhiteSpace(resourceString) || !resourceString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return fallback;
                }

                var trimmed = resourceString.Substring(prefix.Length);
                var slashIndex = trimmed.IndexOf('/');
                if (slashIndex <= 0)
                {
                    return fallback;
                }

                var assemblyName = trimmed.Substring(0, slashIndex);
                var subPath = trimmed.Substring(slashIndex + 1);

                var uri = new Uri($"/{assemblyName};component/{subPath}", UriKind.Relative);

                var streamInfo = System.Windows.Application.GetResourceStream(uri).Result;
                if (streamInfo?.Stream == null)
                {
                    return fallback;
                }

                return new InMemoryFileInfo(Path.GetFileName(trimmed), streamInfo.Stream);
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                var file = _fileProvider.GetFileInfo(subpath);

                var unescaped = Uri.UnescapeDataString(subpath);
                if (file.Exists || unescaped == subpath)
                {
                    return file;
                }

                var result = GetResourceFileInfo(unescaped, file);

                return result;
            }

            public IChangeToken Watch(string filter)
            {
                return _fileProvider.Watch(filter);
            }
        }

        public override IFileProvider CreateFileProvider(string contentRootDir)
        {
            var fp = base.CreateFileProvider(contentRootDir);

            return new FallbackResourceFileProvider(fp);
        }
    }
}
