using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;


namespace fidus
{
	[DataTable("Person")]
	public class Person
	{
		[JsonProperty]
		public string id { get; set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty]
		public string Email { get; set; }

		[JsonProperty]
		public string Pass { get; set; }

		[JsonProperty]
		public int Points { get; set; }

		[JsonProperty]
		public bool IsAdmin { get; set; }

		[JsonProperty]
		public DateTime Birthday { get; set; }

		[JsonProperty]
		public bool Logged { get; set; }

		[JsonProperty]
		public DateTime LastLogin { get; set;}

		[JsonProperty]
		public String Phone { get; set;}

	[Version]
		public string Version { get; set; }
	}

}
