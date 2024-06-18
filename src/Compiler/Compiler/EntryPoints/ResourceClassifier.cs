
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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler;

public class ResourceClassifier : Task
{
    [Required]
    public ITaskItem[] ResourceFiles { get; set; }

    [Required]
    public ITaskItem[] ContentFiles { get; set; }

    [Output]
    public ITaskItem[] MainEmbeddedFiles { get; set; }

    [Output]
    public ITaskItem[] RemovedContentFiles { get; set; }

    public override bool Execute()
    {
        try
        {
            var mainEmbeddedList = new List<ITaskItem>();
            var removedContentFiles = new List<ITaskItem>();

            foreach (ITaskItem resource in ResourceFiles)
            {
                mainEmbeddedList.Add(CreateResourceItem(resource));
            }

            foreach (ITaskItem content in ContentFiles)
            {
                if (!IsResourceExtension(content))
                {
                    continue;
                }

                mainEmbeddedList.Add(CreateResourceItem(content));
                removedContentFiles.Add(new TaskItem(content));
            }

            MainEmbeddedFiles = mainEmbeddedList.ToArray();
            RemovedContentFiles = removedContentFiles.ToArray();

            return true;
        }
        catch (Exception e)
        {
            string errorId = Log.ExtractMessageCode(e.Message, out string message);

            if (string.IsNullOrEmpty(errorId))
            {
                errorId = "FC1000";
                message = $"Unknown build error, '{message}' ";
            }

            Log.LogError(null, errorId, null, null, 0, 0, 0, 0, message, null);

            return false;
        }
    }

    private static ITaskItem CreateResourceItem(ITaskItem inputItem)
    {
        var outputItem = new TaskItem
        {
            ItemSpec = inputItem.ItemSpec,
        };

        // Selectively copy metadata over.
        outputItem.SetMetadata("Link", inputItem.GetMetadata("Link"));
        outputItem.SetMetadata("LogicalName", inputItem.GetMetadata("LogicalName"));

        return outputItem;
    }

    private static bool IsResourceExtension(ITaskItem item)
    {
        string extension = item.GetMetadata("Extension");

        return string.Equals(extension, ".js", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".css", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".ico", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".mp4", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".ogv", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".webm", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".3gp", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".mp3", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".ogg", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".ttf", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".woff", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".woff2", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".cur", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".config", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".clientconfig", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".htm", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(extension, ".svg", StringComparison.OrdinalIgnoreCase);
    }
}
