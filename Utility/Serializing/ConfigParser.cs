using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration; 

namespace CFlat.Utility
{
    public class ConfigParser
    {
        private static void setValue(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");    
        }

        private static string getValue(Configuration config, string key)
        {
            if (config.AppSettings.Settings[key] == null)
                return "";
            else
                return config.AppSettings.Settings[key].Value;
        }

        private static Configuration appConfig()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        private static Configuration cflatConfig()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = "config/cflat.config"; ;
            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        public static void setValueToAppConfig(string key, string value)
        {
            setValue(appConfig(), key, value);
        }

        public static string getValueFromAppConfig(string key)
        {
            return getValue(appConfig(), key);
        }

        public static void setValueToCFlatConfig(string key, string value)
        {
            setValue(cflatConfig(), key, value);
        }
        public static string getValueFromCFlatConfig(string key)
        {
            return getValue(cflatConfig(), key);
        }
    }
}
