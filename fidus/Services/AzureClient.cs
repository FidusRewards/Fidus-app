using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Plugin.Connectivity;

namespace fidus
{
	public class AzureClient<T> where T: class
	{
		static AzureClient<T> defaultInstance = new AzureClient<T>();

		private IMobileServiceClient _client;
		private IMobileServiceSyncTable<T> _table;
		private IMobileServiceTable<T> _tabla;

		private IMobileServiceTableQuery<Person> PersonQuery;

		public AzureClient()
		{

			try
			{
				_client = new MobileServiceClient(Settings.AzureUrl);
				var store = new MobileServiceSQLiteStore("localstore.db");
				store.DefineTable<T>();
				_client.SyncContext.InitializeAsync(store);

				_table = _client.GetSyncTable<T>();

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

		public IMobileServiceSyncTable<T> GetTable()
		{

			return _table;

		}

		public IMobileServiceTable<Person> GetPTable()
		{
			return _client.GetTable<Person>();
		}

		public IMobileServiceTable<WhiteList> GetWTable()
		{
			return _client.GetTable<WhiteList>();
		}

		public async Task PurgeData() { 
			await _client.SyncContext.PushAsync();

			await _table.PurgeAsync();


		}

		public async Task Push()
		{
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

			try
			{
				await _client.SyncContext.PushAsync();

			}catch (MobileServicePushFailedException exc)
			{
				if (exc.PushResult != null)
				{
					syncErrors = exc.PushResult.Errors;
				}
			}

			// Simple error/conflict handling.
			if (syncErrors != null)
			{
				foreach (var error in syncErrors)
				{
					if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
					{
						// Update failed, revert to server's copy
						await error.CancelAndUpdateItemAsync(error.Result);
					}
					else
					{
						// Discard local change
						await error.CancelAndDiscardItemAsync();
					}

					Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
				}
			}
		}

		public async Task SyncAsync()
		{
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

			try
			{
				await _client.SyncContext.PushAsync();

				// The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
				// Use a different query name for each unique query in your program.
				string queryName = $"incsync_{typeof(T).Name}";
				Debug.WriteLine("SyncAsync begin: "+typeof(T));

				//bool Reach = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");

				if ( CrossConnectivity.Current.IsConnected)
				{
					Settings.Default.IsInternetEnabled = true;
					await _table.PullAsync(queryName, _table.CreateQuery());
					Debug.WriteLine("SyncAsync: " + typeof(T) + " Pull finished");


				}
			}
			catch (MobileServicePushFailedException exc)
			{
				if (exc.PushResult != null)
				{
					syncErrors = exc.PushResult.Errors;
				}
			}

			// Simple error/conflict handling.
			if (syncErrors != null)
			{
				foreach (var error in syncErrors)
				{
					if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
					{
						// Update failed, revert to server's copy
						await error.CancelAndUpdateItemAsync(error.Result);
					}
					else
					{
						// Discard local change
						await error.CancelAndDiscardItemAsync();
					}

					Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
				}
			}
		}
	}
}
