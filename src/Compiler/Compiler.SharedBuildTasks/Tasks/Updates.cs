﻿
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.IO;
using System.Net.Http;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler;

public sealed class Updates : Task
{
    private const string Key = "Identifier";

    [Required]
    public string PackagePath { get; set; }

    public string IDE { get; set; }

    public override bool Execute()
    {
        Update("OpenSilver");
        return true;
    }

    private async void Update(string productId)
    {
        try
        {
            string identifier = GetIdentifier();
            string productVersion = GetProductVersion();
            string date = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            string os = Uri.EscapeDataString(Environment.OSVersion.ToString());
            string ide = Uri.EscapeDataString(IDE ?? "");

            using var client = new HttpClient();
            await client.GetAsync(
                new UriBuilder("https://opensilver-service.azurewebsites.net/api/Updates")
                {
                    Query = $"id={identifier}&productId={productId}&version={productVersion}&date={date}&os={os}&ide={ide}"
                }.ToString());
        }
        catch { }
    }

    private static string GetIdentifier()
    {
        string id = OpenSilverSettings.Instance.GetValue(Key);
        if (string.IsNullOrWhiteSpace(id))
        {
            id = Guid.NewGuid().ToString();
            OpenSilverSettings.Instance.SetValue(Key, id);
            OpenSilverSettings.Instance.SaveSettings();
        }
        return id;
    }

    private string GetProductVersion()
    {
        string pkgDirectory = Path.GetDirectoryName(PackagePath);
        return PackagePath.Substring(pkgDirectory.Length + 1);
    }
}
