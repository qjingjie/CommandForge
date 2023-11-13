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
        /// <param name="defaultConfigPath"></param>
        /// <returns></returns>
        public bool LoadConfig(string fileName, string defaultConfigPath)
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
                Config = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(Path.Combine(defaultConfigPath, "Config.json")));
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
        #endregion
    }
}