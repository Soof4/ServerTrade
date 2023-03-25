using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServerTrade {
    public class Config {
        public IDictionary<string, int[]> shopList = new Dictionary<string, int[]>();

        public void Write() {
            File.WriteAllText(ServerTrade.path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static Config Read() {
            if (!File.Exists(ServerTrade.path)) {
                return new Config();
            }
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ServerTrade.path));
        }
    }
}
