using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace fidus
{
	[DataTable("Place")]
	public class Place
	{
		[JsonProperty]
		public string id { get; set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty]
		public string Address { get; set; }

		[JsonProperty]
		public string Logo { get; set; }

		[JsonProperty]
		public string Admin { get; set; }

		[JsonProperty]
		public string Category { get; set; }

		[Version]
		public string Version { get; set; }
	}
}

