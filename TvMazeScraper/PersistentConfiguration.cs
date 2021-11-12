using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using TvMazeScraper.Domain.Services.Contracts;

namespace TvMazeScraper
{
    // TODO use a database such as redis
    public class PersistentConfiguration : IPersistentConfiguration
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger _logger;

        private readonly string UpdatableConfigurationFilePath = Path.Combine(AppContext.BaseDirectory, "updatableConfiguration.json");

        private readonly IConfiguration _fileConfiguration;


        public PersistentConfiguration(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(UpdatableConfigurationFilePath, FileMode.CreateNew)))
                {
                    sw.WriteLine("{}");
                }
                _logger.Information("Created configuration file");
            }
            catch (IOException e)
            {
                _logger.Information("Configuration file already exists");
                //Ignore. File already Exists
            }

            _fileConfiguration = new ConfigurationBuilder()
                .AddJsonFile(UpdatableConfigurationFilePath, false, reloadOnChange: true)
                .Build();

        }

        public T GetAppSetting<T>(string key)
        {
            //read is done on persistent file with fallback on normal configuration
            T value = _fileConfiguration.GetValue<T>(key);
            if (value.Equals(default(T)))
            {
                return _configuration.GetValue<T>(key);
            }
            return value;

        }

        public void AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {

            // write is done on the persistent file
            string json = File.ReadAllText(UpdatableConfigurationFilePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            SetValueRecursively(sectionPathKey, jsonObj, value);

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(UpdatableConfigurationFilePath, output);
        }

        private void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }
        }


    }
}
