using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace IShadowx.Core
{
    public static class ConfigurationManager
    {
        private static IConfiguration _configuration;
        public static void UseConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string Get(string key)
        {
            return _configuration[key];
        }

        public static T Get<T>(string key)
        {
            T appSetting = default(T);
            if (typeof(T).IsPrimitive)
            {
                appSetting = _configuration.GetValue<T>(key);
            }
            else
            {
                var configSection = _configuration. GetSection(key);
                if (configSection.Exists())
                {
                    appSetting = Activator.CreateInstance<T>();
                    configSection.Bind(appSetting);
                }
            }
            return appSetting;
        }
    }
}
