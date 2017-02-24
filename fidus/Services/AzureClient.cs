using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Plugin.Connectivity;
using System.Linq;
using System.IO;

namespace fidus
{
	public class AzureClient<T> where T: class
	{
		public static AzureClient<T> defaultInstance = new AzureClient<T>();

		private IMobileServiceClient _client;
		private IMobileServiceSyncTable<T> _table;
		private IMobileServiceTable<T> _tabla;
		private MobileServiceSQLiteStore store = new MobileServiceSQLiteStore("localstore.db");

		private IMobileServiceTableQuery<Person> PersonQuery;

		public AzureClient()
		{

			try
			{
				_client = new MobileServiceClient(Helpers.Settings.AzureUrl);
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

		public IMobileServiceTable<History> GetHTable()
		{
			return _client.GetTable<History>();
		}

		public async Task PurgeData() {
			if (_client.SyncContext.PendingOperations>0)
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

		public async Task SyncAsync(string queryName=null)
		{
			//string queryName;
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					Helpers.Settings.IsInternetEnabled = true;
					await _client.SyncContext.PushAsync();

					// The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
					// Use a different query name for each unique query in your program.
					//if (Helpers.Settings.IsLogin || Helpers.Settings.IsBoot)
					//	queryName = null;
					//else
					// 	queryName = $"incsync_{typeof(T).Name}";
					//Debug.WriteLine("SyncAsync begin: "+typeof(T));

					//bool Reach = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");
					try
					{


						if (_table.TableName == "History")
						{
							IMobileServiceSyncTable<History> _tablaH = _client.GetSyncTable<History>();
							await _tablaH.PullAsync(queryName, _tablaH.CreateQuery().Where(f => f.Person == Helpers.Settings.UserEmail));
							Debug.WriteLine("SyncAsync: " + typeof(T) + "Pull finished");
						}
						else
						{
							await _table.PullAsync(queryName, _table.CreateQuery());
							Debug.WriteLine("SyncAsync: " + typeof(T) + " Pull finished");
						}
						var itemsInLocalTable = (await _table.ReadAsync()).Count();
						Debug.WriteLine("There are {0} items in the local table {1}", itemsInLocalTable, typeof(T));


					}
					catch (MobileServiceInvalidOperationException e)
					{
						// Handle error
						Debug.WriteLine(e.StackTrace);
					}
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
