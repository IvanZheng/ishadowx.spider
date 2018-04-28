using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using IShadowx.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IShadowx.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), true, true);
            var configuration = configurationBuilder.Build();
            ConfigurationManager.UseConfiguration(configuration);
            var sources = ConfigurationManager.Get<List<Source>>("sources")?
                                              .ToArray();
            var capturer = new Capturer();
            var configs = capturer.ParseFromSource(sources);
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.Get("GuiConfigPath"));
            var guiConfigJson = File.ReadAllText(configFile);
            var guiConfig = JsonConvert.DeserializeObject<JObject>(guiConfigJson);
            guiConfig["configs"] = configs;

            File.WriteAllText(configFile, JsonConvert.SerializeObject(guiConfig));

            System.Console.WriteLine("Update Successful Done");
            System.Console.WriteLine("Update Successful Ok");
        }
    }
}
