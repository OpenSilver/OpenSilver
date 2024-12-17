
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
using System.Globalization;
using System.IO;
using System.Net.Http;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler;

public sealed class Updates : Task
{
    private const string IdentifierKey = "Identifier";
    private const string LastUpdateDateKey = "LastUpdateDate";

    [Required]
    public string PackagePath { get; set; }

    public string IDE { get; set; }

    public override bool Execute()
    {
        Update("OpenSilver");
        return true;
    }

    private void Update(string productId)
    {
        if (IgnoreUpdate())
        {
            return;
        }

        SetLastUpdateDate(DateTime.UtcNow);

        try
        {
            string identifier = GetIdentifier();
            string productVersion = GetProductVersion();
            string date = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
            string os = Uri.EscapeDataString(Environment.OSVersion.ToString());
            string ide = Uri.EscapeDataString(IDE ?? "");

            using var client = new HttpClient()
            {
                // If the request takes more than a few seconds, it is not worth waiting for,
                // as this would unnecessarily delay the build process.
                Timeout = TimeSpan.FromSeconds(5)
            };
            var query = new UriBuilder("https://opensilver-service.azurewebsites.net/api/Updates")
            {
                Query = $"id={identifier}&productId={productId}&version={productVersion}&date={date}&os={os}&ide={ide}"
            }.ToString();
            client.GetAsync(query).GetAwaiter().GetResult();
        }
        catch { }

        SaveSettings();
    }

    private bool IgnoreUpdate() => (DateTime.UtcNow - GetLastUpdateDate()) < TimeSpan.FromDays(1);

    private static string GetIdentifier()
    {
        string id = OpenSilverSettings.Instance.GetValue(IdentifierKey);
        if (string.IsNullOrWhiteSpace(id))
        {
            id = Guid.NewGuid().ToString();
            OpenSilverSettings.Instance.SetValue(IdentifierKey, id);
        }
        return id;
    }

    private string GetProductVersion() => PackagePath.Substring(Path.GetDirectoryName(PackagePath).Length + 1);

    private DateTime GetLastUpdateDate()
    {
        string value = OpenSilverSettings.Instance.GetValue(LastUpdateDateKey);
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long ticks))
        {
            return new DateTime(ticks);
        }
        return DateTime.MinValue;
    }

    private void SetLastUpdateDate(DateTime utcDate) =>
        OpenSilverSettings.Instance.SetValue(LastUpdateDateKey, utcDate.Ticks.ToString(CultureInfo.InvariantCulture));

    private void SaveSettings()
    {
        try
        {
            OpenSilverSettings.Instance.SaveSettings();
        }
        catch { }
    }
}
