using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;


namespace fidus
{
	[DataTable("Rewards")]
	public class Rewards
	{
		[JsonProperty]
		public string id { get; set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty]
		public string Admin { get; set; }

		[JsonProperty]
		public string Place { get; set; }

		[JsonProperty]
		public string Description { get; set; }

		[JsonProperty]
		public string Photo { get; set; }

		[JsonProperty]
		public int ReqPoints { get; set; }

		[Version]
		public string Version { get; set; }
	}
}
