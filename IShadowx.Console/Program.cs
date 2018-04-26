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
            var sources = ConfigurationManager.Get<List<Source>>("sources");
            var httpClient = new HttpClient();

            var configs = new JArray();
            foreach (var source in sources)
            {
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                var html = httpClient.GetAsync(source.Url).Result.Content.ReadAsStringAsync().Result;
                htmlDocument.LoadHtml(html);
                var nodes = htmlDocument.DocumentNode.SelectNodes(source.Selector);
                foreach (var node in nodes)
                {
                    var ip = node.SelectSingleNode(source.IpSelector)?.InnerText.Trim();
                    var port = node.SelectSingleNode(source.PortSelector)?.InnerText.Trim();
                    var password = node.SelectSingleNode(source.PwdSelector)?.InnerText.Trim();
                    var method = node.SelectSingleNode(source.MethodSelector)?.InnerText.Split(":")?.LastOrDefault()?.Trim();
                    var config = new
                    {
                        server = ip,
                        server_port = int.Parse(port),
                        password,
                        method,
                        plugin = "",
                        plugin_opts = "",
                        remarks = "",
                        timeout = 5
                    };
                    configs.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(config)));
                }
            }

            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gui-config.json");
            var guiConfigJson = File.ReadAllText(configFile);
            var guiConfig = JsonConvert.DeserializeObject<JObject>(guiConfigJson);
            guiConfig["configs"] = configs;
            File.WriteAllText(configFile, JsonConvert.SerializeObject(guiConfig));
            System.Console.WriteLine("Hello world");
        }
    }
}
