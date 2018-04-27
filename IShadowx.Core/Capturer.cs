using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IShadowx.Core
{
    public class Capturer
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public JArray ParseFromSource(params Source[] sources)
        {
            var configs = new JArray();
            foreach (var source in sources)
            {
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                var html = HttpClient.GetAsync(source.Url).Result.Content.ReadAsStringAsync().Result;
                htmlDocument.LoadHtml(html);
                var nodes = htmlDocument.DocumentNode.SelectNodes(source.Selector);
                foreach (var node in nodes)
                {
                    var ip = node.SelectSingleNode(source.IpSelector)?.InnerText.Trim();
                    var port = node.SelectSingleNode(source.PortSelector)?.InnerText.Trim();
                    var password = node.SelectSingleNode(source.PwdSelector)?.InnerText.Trim();
                    var method = node.SelectSingleNode(source.MethodSelector)?.InnerText.Split(':')?.LastOrDefault()?.Trim();
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
            return configs;
        }
    }
}
