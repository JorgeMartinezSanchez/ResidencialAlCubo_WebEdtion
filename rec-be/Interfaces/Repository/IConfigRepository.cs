using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IConfigRepository
    {
        Task<KeyValuePair<string, string>> CreateConfig(KeyValuePair<string, string> newConfig);
        Task<KeyValuePair<string, string>> GetConfigByKey(string ConfigKey);
        Task<Dictionary<string, string>> GetConfigAllConfigs();
        Task SetNewConfig(string _ConfigKey, string _ConfigValue);
        Task DeleteConfig(KeyValuePair<string, string> SelectedConfig);
    }
}