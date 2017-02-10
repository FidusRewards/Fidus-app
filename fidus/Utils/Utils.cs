using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace fidus
{
	public static class Utils
	{
		public static bool IsAny<T>(this IEnumerable<T> data)
		{
			return (data != null && data.Any());
		}
	}
}

