
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

using System.IO;

namespace OpenSilver.Simulator
{
    internal static class MimeTypesMap
    {
        private static readonly Dictionary<string, string> MimeTypes = new (StringComparer.InvariantCultureIgnoreCase)
        {
            // Text
            { ".txt", "text/plain" },
            { ".css", "text/css" },
            { ".csv", "text/csv" },
            { ".html", "text/html" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            // Images
            { ".bmp", "image/bmp" },
            { ".gif", "image/gif" },
            { ".ico", "image/vnd.microsoft.icon" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" },
            { ".svg", "image/svg+xml" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" },
            { ".webp", "image/webp" },
            // Audio
            { ".aac", "audio/aac" },
            { ".midi", "audio/midi" },
            { ".mp3", "audio/mpeg" },
            { ".oga", "audio/ogg" },
            { ".wav", "audio/wav" },
            { ".weba", "audio/webm" },
            // Video
            { ".avi", "video/x-msvideo" },
            { ".mpeg", "video/mpeg" },
            { ".mp4", "video/mp4" },
            { ".ogv", "video/ogg" },
            { ".webm", "video/webm" },
            { ".mov", "video/quicktime" },
            // Applications
            { ".pdf", "application/pdf" },
            { ".zip", "application/zip" },
            { ".rar", "application/x-rar-compressed" },
            { ".tar", "application/x-tar" },
            { ".7z", "application/x-7z-compressed" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            // Fonts
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".ttf", "font/ttf" },
            { ".otf", "font/otf" },
            // Add more file types as needed
        };

        public static string GetMimeType(string fileName)
        {
            if (MimeTypes.TryGetValue(Path.GetExtension(fileName), out var mimeType))
            {
                return mimeType;
            }

            return "application/octet-stream"; // Fallback MIME type
        }
    }
}

