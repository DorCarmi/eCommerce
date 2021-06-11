using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace eCommerce
{
    public class AppConfig
    {
        private static AppConfig _instance = new AppConfig();

        private IConfigurationRoot _config;
        private AppConfig()
        {
        }

        public static AppConfig GetInstance()
        {
            return _instance;
        }
        
        public bool Init(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            _config = new ConfigurationBuilder()
                .AddJsonFile(filePath)
                .Build();
            return true;
        }

        public string GetData(string dataPath)
        {
            return _config?[dataPath];
        }

        public void ThrowErrorOfData(string data, string state)
        {
            throw new InvalidDataException($"{data} data in the config file is {state}");
        }
    }
}