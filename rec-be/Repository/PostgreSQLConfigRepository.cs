using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rec_be.Data;
using rec_be.Interfaces.Repository;
using rec_be.Models;

namespace rec_be.Repository
{
    public class PostgreSQLConfigRepository : IConfigRepository
    {
        protected RACPostgreSQLDbContext dbContext;
        public PostgreSQLConfigRepository(RACPostgreSQLDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<KeyValuePair<string, string>> CreateConfig(KeyValuePair<string, string> newConfig)
        {
            Config DictToModelConfig = new Config
            {
                ConfigKey = newConfig.Key,
                ConfigValue = newConfig.Value
            };
            await dbContext.Configs.AddAsync(DictToModelConfig);
            await dbContext.SaveChangesAsync();
            return newConfig;
        }
        public async Task<KeyValuePair<string, string>> GetConfigByKey(string ConfigKey)
        {
            var Config = await dbContext.Configs.FindAsync(ConfigKey);
            if (Config == null) throw new Exception("CONFIG REPOSITORY ERROR: No config was found in the config table.");
            return new KeyValuePair<string, string>(Config.ConfigKey, Config.ConfigValue);
        }
        public async Task<Dictionary<string, string>> GetConfigAllConfigs()
        {
            var Configs = await dbContext.Configs.ToListAsync();
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var c in Configs)
            {
                result.Add(c.ConfigKey, c.ConfigValue);
            }
            return result;
        }
        public async Task SetNewConfig(string _ConfigKey, string _ConfigValue)
        {
            Config configTarget = new Config
            {
                ConfigKey = _ConfigKey,
                ConfigValue = _ConfigValue
            };
            bool exists = await dbContext.Configs.AnyAsync(c => (c.ConfigKey == _ConfigKey) && (c.ConfigValue == c.ConfigValue));
            if (!exists) throw new Exception($"CONFIG REPOSITORY ERROR: {_ConfigKey} was found in the config table.");
            dbContext.Configs.Update(configTarget);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteConfig(KeyValuePair<string, string> SelectedConfig)
        {
            Config configTarget = new Config
            {
                ConfigKey = SelectedConfig.Key,
                ConfigValue = SelectedConfig.Value
            };
            bool exists = await dbContext.Configs.AnyAsync(c => (c.ConfigKey == SelectedConfig.Key) && (c.ConfigValue == SelectedConfig.Value));
            if (!exists) throw new Exception($"CONFIG REPOSITORY ERROR: {SelectedConfig.Key} was found in the config table.");
            dbContext.Configs.Update(configTarget);
            await dbContext.SaveChangesAsync();
        }
    }
}