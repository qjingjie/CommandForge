using Newtonsoft.Json;
using System;
using System.IO;

namespace CommandForge.Models
{
    public class ConfigManager
    {
        #region Constructor
        public ConfigManager()
        {
            Config = new ConfigFile();
        }
        #endregion

        #region Properties
        public ConfigFile Config
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load configuration file - If the file does not exist, a new configuration file named "Config.json" will be created.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>True is successfully loaded configuration file, False otherwise</returns>
        public bool LoadConfig(string fileName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           "CommandForge",
                                           fileName + ".json");

            bool isCreated = false;

            if (File.Exists(filePath))
            {
                try
                {
                    Config = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(filePath));
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                Config = GenerateDefaultConfig();
                File.WriteAllText(filePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
                isCreated = true;
            }

            return isCreated;
        }

        /// <summary>
        /// Writes to a config file.
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteConfig(string fileName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           "CommandForge",
                                           fileName + ".json");

            File.WriteAllText(filePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        /// <summary>
        /// Generate a default configuration file.
        /// </summary>
        /// <returns>A default configuration file</returns>
        private static ConfigFile GenerateDefaultConfig()
        {
            ConfigFile config = new ConfigFile();
            config.Defaults.EnableLogging = false;
            config.Defaults.ZmqSubscriberIPv4 = "localhost";
            config.Defaults.ZmqSubscriberPort = 3200;
            config.Defaults.ZmqPublisherIPv4 = "localhost";
            config.Defaults.ZmqPublisherPort = 3300;

            return config;
        }
        #endregion
    }
}