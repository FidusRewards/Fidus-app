using System;
using System.Security.Cryptography;
using System.Text;
using fidus.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Hash256_Droid))]

namespace fidus.Droid
{
	public class Hash256_Droid : IHash256
	{
		public Hash256_Droid()
		{
		}

		public string Hash256(String input)
		{
			SHA512 shaM = new SHA512Managed();
			// Convert the input string to a byte array and compute the hash.
			byte[] data = shaM.ComputeHash(Encoding.UTF8.GetBytes(input));
			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();
			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			// Return the hexadecimal string.
			input = sBuilder.ToString();
			return (input);
		}
	}
}
