using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;

namespace fidus
{
	public class LoadAsync<T> where T: class
	{
		private MobileServiceClient _client; // = new MobileServiceClient(Helpers.Settings.AzureUrl);
		private IMobileServiceSyncTable<T> _tabla;
		private MobileServiceSQLiteStore store = new MobileServiceSQLiteStore("localstore.db");

		public LoadAsync(MobileServiceClient azureclient)
		{
			_client = azureclient; //new AzureClient<T>();//azureclient; // 

			if (!_client.SyncContext.IsInitialized)
			{
				store.DefineTable<History>();
				store.DefineTable<Rewards>();
				store.DefineTable<WhiteList>();
				store.DefineTable<Place>();
				_client.SyncContext.InitializeAsync(store);
			}
		 	_tabla = _client.GetSyncTable<T>();
	
		}

		public IMobileServiceSyncTable GetTable()
		{
			return _client.GetSyncTable<T>();
		}
		public async Task<ObservableCollection<T>> Load()
		{
			ObservableCollection<T> Items= new ObservableCollection<T>();

			Items.Clear();

			var result = await _tabla.ToEnumerableAsync();
			//if (!Utils.IsAny(result))
			//{
			//	Debug.WriteLine("Load "+typeof(T)+" null -> Syncing");
			//	await InitSync();
			//	result = await _client.GetData();
			//}
			foreach (var item in result)
			{
				Debug.WriteLine("Item read");
				Items.Add(item);
			}

			return Items;
		}

		public async Task<IEnumerable<WhiteList>> LoadW(string place, string branch, string qrcode)
		{

			var tablaH = (IMobileServiceSyncTable<WhiteList>)_tabla;
			var query = tablaH.CreateQuery().IncludeTotalCount().Where(whitelist => whitelist.Place == place && whitelist.Branch == branch && whitelist.ExchangeCode == qrcode);
			return await query.ToEnumerableAsync();
		}

		public async Task Delete(WhiteList qrwhite)
		{
			var temptab = (IMobileServiceSyncTable<WhiteList>)_tabla;
			await temptab.DeleteAsync(qrwhite);
		}

		public async Task<int> Load(History _history)
		{

			var _tablaH = (IMobileServiceSyncTable<History>) _tabla;
			
			int count = 0;
			var query = _tablaH.CreateQuery().IncludeTotalCount().Take(1000).Where(history => history.Place.Contains(_history.Place) && history.Person == _history.Person);
			var result = await query.ToEnumerableAsync();
			//.Where(history => history.Place.Contains(_history.Place) && history.Person == _history.Person).Take(1000).ToEnumerableAsync();

			//var result = await query.Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();

			//var result = await query.Take(1000).ToEnumerableAsync();

			long Tcount = ((IQueryResultEnumerable<History>)result).TotalCount;

			//if (Tcount==0)
			//{
			//	Debug.WriteLine("Load " + typeof(T) + " null -> Syncing");
			//	await InitSync();
			//	result = await query.Take(1000).ToEnumerableAsync();
			//}
			int _points = 0;

			if (Tcount!=0)
			{
				foreach (History item in result)
				{
					//History _temp = (History)(object)item;
					_points += item.EarnPoints;
					count++;
					Debug.WriteLine("LoadAsync:Load(History) Type History: " + item.EarnPoints);

				};
			}
			Debug.WriteLine("LoadAsync:Load(History) Count of entries: " + count );
                                   
			//var result = await _client.GetData();

			//foreach (var item in result)
			//{

			

			//}

			return _points;
		}

		public async Task SaveHistory(History hitem)
		{
			var _tablaH = (IMobileServiceSyncTable<History>) _tabla;

			await _tablaH.InsertAsync(hitem);
			await Push();
		}

		public async Task<ObservableCollection<Rewards>> Load(Rewards _rewards)
		{
			ObservableCollection<Rewards> Ritems = new ObservableCollection<Rewards>();

			Ritems.Clear();

			IMobileServiceSyncTable<Rewards> _tablaR = (IMobileServiceSyncTable<Rewards>)_tabla;

			var result = await _tablaR.Where(rewards => rewards.Place == _rewards.Place).ToEnumerableAsync();

			//if (!Utils.IsAny(result))
			//{
			//	Debug.WriteLine("Load " + typeof(T) + " null -> Syncing");
			//	await InitSync();
			//	result = await _tabla.Where(rewards => rewards.Place == _rewards.Place).ToEnumerableAsync();
			//}

				foreach (Rewards item in result)
			{
				//History _temp = (History)(object)item;
				if (item.Place == _rewards.Place)
					Ritems.Add((Rewards)item);
				Debug.WriteLine("LoadAsync: Load(Rewards) Type Rewards: " + item.Name);
			};

			return Ritems;
		}

		public async Task<bool> Save(History _history)
		{
			var Datos = new History()
			{
				Place = _history.Place,
				Person = _history.Person,
				EarnPoints = _history.EarnPoints,
				IsDebit = _history.IsDebit,
				DateTime = _history.DateTime,
				Reward = _history.Reward,
                ExchangeCode = _history.ExchangeCode,
                Branch = _history.Branch,
				Rating = _history.Rating,
				Comment = _history.Comment
			};

			IMobileServiceSyncTable<History> Tabla = (IMobileServiceSyncTable<History>) _tabla;

			await Tabla.InsertAsync(Datos);
			await Push();

			return true;
		}

		public async Task<ObservableCollection<History>> Load(string person, WhiteList qrcode=null)
		{
			ObservableCollection<History> Hitems;
			IEnumerable<History> result;

			//Hitems.Clear();
			var _tablaH = (IMobileServiceSyncTable<History>)_tabla;

			if (qrcode == null)
			{
				result = await _tablaH.Where(history => history.Person == person)
										.Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
				//if (!Utils.IsAny(result))
				//{
				//	Debug.WriteLine("Load History null -> Syncing");
				//	await InitSync();
				//	result = await _tablaH.Where(history => history.Person == person)
				//						 .Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
				//}

			}else {
				IMobileServiceTableQuery<History> _query = _tablaH.CreateQuery().Where(history => (history.ExchangeCode == qrcode.ExchangeCode) && (history.DateTime.Day == DateTime.Now.Day) && (history.Person == person));
				result = await _query.ToEnumerableAsync();
			}

			Hitems = new ObservableCollection<History>(result);

			//foreach (History item in result)
			//{

			//		Hitems.Add((History)item);

			//}

			return Hitems;
		}

		public async Task<ObservableCollection<Place>> Load(ObservableCollection<Place> _places)
		{
			//await InitSync();
			Debug.WriteLine("Entro al Load(Places)");
			ObservableCollection<Place> Pitems = new ObservableCollection<Place>();

			Pitems.Clear();
			//IMobileServiceSyncTable<Place> _tabla = (IMobileServiceSyncTable<Place>)_client.GetTable();

				//await _client.PurgeData();

			IEnumerable<Place> result = (IEnumerable<Place>) await _tabla.ToEnumerableAsync();
			//if (!Utils.IsAny(result))
			//{
			//	Debug.WriteLine("Load Places null -> Syncing");
			//	await InitSync();
			//	result = (IEnumerable<Place>)await _client.GetData();

			//}
			if (result.IsAny())
			{
				Pitems = new ObservableCollection<Place>(result);
			}
			//	foreach (Place item in result)
			//	{
			//		Debug.WriteLine("Load Places -> " + item.Name);
			//		item.Logo = Helpers.Settings.ImgSrvProd + item.Logo;
			//		if (item.Admin == "YES")
			//		{
			//			if (Helpers.Settings.UserIsAdmin)
			//				Pitems.Add((Place)item);
			//		}
			//		else
			//			Pitems.Add((Place)item);
			//	}
			//}
			return Pitems;
		}

		public async Task SyncAsync(string queryName = null)
		{
			//string queryName;
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

			try
			{
				if (CrossConnectivity.Current.IsConnected)
				{
					Helpers.Settings.IsInternetEnabled = true;

					if (_client.SyncContext.PendingOperations > 0)
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


						if (_tabla.TableName == "History")
						{
							IMobileServiceSyncTable<History> _tablaH = (IMobileServiceSyncTable<History>)_tabla;
							var query = _tablaH.CreateQuery().Where(f => f.Person == Helpers.Settings.UserEmail);
							await _tabla.PullAsync(null, query);
							Debug.WriteLine("SyncAsync: " + typeof(T) + "Pull finished");
						}
						else
						{
							await _tabla.PullAsync(null, _tabla.CreateQuery());
							Debug.WriteLine("SyncAsync: " + typeof(T) + " Pull finished");
						}

						//var itemsInLocalTable = (await _table.ReadAsync()).Count();
						//Debug.WriteLine("There are {0} items in the local table {1}", itemsInLocalTable, typeof(T));


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
					Debug.WriteLine("Error en el SyncAsync " + exc.StackTrace);
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

		public async Task Push()
		{
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

			try
			{
				await _client.SyncContext.PushAsync();

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
