using Microsoft.Extensions.Configuration;

namespace eCommerce
{
    public class AppConfig
    {
        private static AppConfig _instance = new AppConfig();

        private IConfigurationRoot _config;
        private AppConfig()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appConfig.json")
                .Build();
        }

        public static AppConfig GetInstance()
        {
            return _instance;
        }

        public string GetData(string dataPath)
        {
            return _config[dataPath];
        }
    }
}