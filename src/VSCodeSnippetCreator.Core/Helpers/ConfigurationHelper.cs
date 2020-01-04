using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

namespace VSCodeSnippetCreator.Core.Helpers
{
    public static class ConfigurationHelper
    {
        public static string ReadSetting(string key)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException ex)
            {
                Debug.WriteLine("Error: Reading app settings. " + ex.Message);
                return null;
            }
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;
                KeyValueConfigurationElement keyElement = settings[key];
                if (keyElement == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                Debug.WriteLine("Error: Writing app settings. " + ex.Message);
            }
        }
    }
}
