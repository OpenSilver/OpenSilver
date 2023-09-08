using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenSilver.Compiler
{
    internal class OpenSilverSettings
    {
        private const char Separator = '|';
        private const string AppName = "OpenSilver";

        private static readonly string SettingsPath = GetSettingsPath();

        private static OpenSilverSettings _instance;

        private readonly Dictionary<string, string> _settingsDictionary;

        private OpenSilverSettings()
        {
            _settingsDictionary = File.Exists(SettingsPath) ? DeserializeDictionary(File.ReadAllLines(SettingsPath)) : new Dictionary<string, string>();
        }


        private static string GetSettingsPath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fullPath = Path.Combine(appDataFolder, AppName, "app.txt");
            return fullPath;
        }

        public static OpenSilverSettings Instance
        {
            get
            {
                _instance ??= new OpenSilverSettings();
                return _instance;
            }
        }

        public void SaveSettings()
        {
            const int numberOfAttempts = 3;
            lock (_settingsDictionary)
            {
                var currentAttempt = 0;
                while (currentAttempt < numberOfAttempts)
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                        File.WriteAllText(SettingsPath, SerializeDictionary(_settingsDictionary));
                        return;
                    }
                    catch (IOException ex)
                    {
                        currentAttempt++;
                        if (currentAttempt == numberOfAttempts)
                        {
                            throw new Exception($"Failed to save the settings file after {numberOfAttempts} attempts.", ex);
                        }
                    }
                }
            }
        }

        public void SetValue(string name, string value)
        {
            lock (_settingsDictionary)
            {
                _settingsDictionary[name] = value;
            }
        }

        public string GetValue(string name)
        {
            lock (_settingsDictionary)
            {
                return _settingsDictionary.TryGetValue(name, out var value) ? value : null;
            }
        }

        private static string SerializeDictionary(Dictionary<string, string> dict)
        {
            return string.Join(Environment.NewLine, dict.Select(kvp => $"{Base64Encode(kvp.Key)}{Separator}{Base64Encode(kvp.Value)}"));
        }

        private static Dictionary<string, string> DeserializeDictionary(IEnumerable<string> lines)
        {
            return lines.Select(l => l.Split(Separator))
                .ToDictionary(l => Base64Decode(l[0]), l => Base64Decode(l[1]));
        }

        private static string Base64Encode(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
