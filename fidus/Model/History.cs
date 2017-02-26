using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;


namespace fidus
{
	[DataTable("History")]
	public class History
	{
		[JsonProperty]
		public string id { get; set; }

		[JsonProperty]
		public string Person { get; set; }

		[JsonProperty]
		public string Place { get; set; }

		[JsonProperty]
		public DateTime DateTime { get; set; }

		[JsonProperty]
		public int EarnPoints { get; set; }

		[JsonProperty]
		public bool IsDebit {get;set;}

		[JsonProperty]
		public string Reward {get;set;}

        [JsonProperty]
        public string Branch { get; set; }

        [JsonProperty]
        public string ExchangeCode { get; set; }

		[JsonProperty]
		public int Rating { get; set;}

		[JsonProperty]
		public string Comment { get; set;}

        [Version]
		public string Version { get; set; }

	}
}
