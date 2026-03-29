using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IConfigRepository
    {
        Task CreateConfig(Dictionary<string, string> newConfig);
        Task<Dictionary<string, string>> GetConfigByKey(string ConfigKey);
        Task SetNewConfig(string ConfigKey, string ConfigValue);
        Task DeleteConfig(Dictionary<string, string> SelectedConfig);
    }
}