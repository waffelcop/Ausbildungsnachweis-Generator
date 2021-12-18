﻿using AusbildungsnachweisGenerator.Model;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace AusbildungsnachweisGenerator
{
    public static class AppHelper
    {

        private static string settingsPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\FiratCanSueluenkue\AusbildungsnachweisGenerator";
        private static string settingsFilePath = @$"{settingsPath}\config.json";
        private static string GetSettingsFileContent()
        {
            return File.ReadAllText(settingsFilePath);
        }
        public static Settings GetSettings()
        {
            try
            {
                if(!Directory.Exists(settingsPath))
                    Directory.CreateDirectory(settingsPath);

                if (!File.Exists(settingsFilePath))
                    SaveSettings(new(new()));

                return JsonConvert.DeserializeObject<Settings>(GetSettingsFileContent());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
        public static void SaveSettings(Settings settings)
        {
            File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(settings));
        }
        public static void AddProfile(Profile profile)
        {
            var settings = GetSettings();
            settings.Profiles.Add(profile);
            SaveSettings(settings);
        }
        public static void DeleteProfile(Profile profile)
        {
            var settings = GetSettings();
            settings.Profiles.Remove(settings.Profiles.First(x => x.Timestamp == profile.Timestamp));
            SaveSettings(settings);
        }
    }
}
