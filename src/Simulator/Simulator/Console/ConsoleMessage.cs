using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Console
{
    public class ConsoleMessage
    {
        public const string ErrorLevel = "error";
        public const string WarningLevel = "warning";
        public const string InfoLevel = "info";
        public const string DebugLevel = "debug";
        public const string LogLevel = "log";

        public const string XmlSource = "xml";
        public const string JavaScriptSource = "javascript";
        public const string NetworkSource = "network";
        public const string ConsoleApiSource = "console-api";
        public const string StorageSource = "storage";
        public const string AppCacheSource = "appcache";
        public const string RenderingSource = "rendering";
        public const string SecuritySource = "security";
        public const string WorkerSource = "worker";
        public const string DeprecationSource = "deprecation";
        public const string OtherSource = "other";

        [JsonPropertyName("level")]
        public string Level { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Level);

            if (!string.IsNullOrEmpty(Text))
            {
                sb.AppendLine($": {Text}");
            }
            else
            {
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(Source))
            {
                sb.AppendLine($"Source: {Source}");
            }

            if (!string.IsNullOrEmpty(Url))
            {
                sb.AppendLine($"Url: {Url}");
            }

            return sb.ToString();
        }
    }

    public class ConsoleMessageHolder
    {
        [JsonPropertyName("message")]
        public ConsoleMessage Message { get; set; }

        [JsonPropertyName("entry")]
        public ConsoleMessage Entry
        {
            get => Message; set => Message = value;
        }
    }
}
