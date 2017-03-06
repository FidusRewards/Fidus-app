using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace fidus
{
    [DataTable("WhiteList")]
	public class WhiteList
    {
        [JsonProperty]
        public string id { get; set; }

        [JsonProperty]
        public string Place { get; set; }

        [JsonProperty]
        public string Branch { get; set; }

        [JsonProperty]
        public string ExchangeCode { get; set; }

        [Version]
        public string Version { get; set; }
    }
}
