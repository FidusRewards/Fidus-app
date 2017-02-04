using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;

namespace fidus
{
	public class AzureClient<T> where T: class
	{
		private IMobileServiceClient _client;
		private IMobileServiceTable<T> _table;

		public AzureClient()
		{

			try
			{
				_client = new MobileServiceClient(Settings.AzureUrl);

				_table = _client.GetTable<T>();

			}
			catch (Exception ex)
			{
				Debug.WriteLine("AzureClient: GetTable "+ex.StackTrace);
			}
		}

		public Task<IEnumerable<T>> GetData()
		{
			Task<IEnumerable<T>> _result = null;
			try
			{
			 	_result = _table.ToEnumerableAsync();
			}
			catch (Exception ex) {
				Debug.WriteLine("AzureClient: GetData "+ex.StackTrace);
			}

			return _result;
		}

		public IMobileServiceTable<T> GetTable()
		{

			return _table;

		}


	}
}
