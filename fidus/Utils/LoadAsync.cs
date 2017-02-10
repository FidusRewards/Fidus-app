using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace fidus
{
	public class LoadAsync<T> where T: class
	{
		private AzureClient<T> _client;


		public LoadAsync()
		{
			_client = new AzureClient<T>();

		}

		public async Task<ObservableCollection<T>> Load()
		{
			ObservableCollection<T> Items;

			Items = new ObservableCollection<T>();

			Items.Clear();

			var result = await _client.GetData();
			if (!Utils.IsAny(result))
			{
				Debug.WriteLine("Load "+typeof(T)+" null -> Syncing");
				await InitSync();
				result = await _client.GetData();
			}
			foreach (var item in result)
			{
				Debug.WriteLine("Item readed");
				Items.Add(item);
			}

			return Items;
		}

		public async Task<ObservableCollection<History>> Load(History _history)
		{
			ObservableCollection<History> Hitems;
			Hitems = new ObservableCollection<History>();

			Hitems.Clear();
			IMobileServiceSyncTable<History> _tabla = (IMobileServiceSyncTable<History>) _client.GetTable();

			int count = 0;
			var query = _tabla.IncludeTotalCount().Where(history => history.Place == _history.Place && history.Person == _history.Person);
			var result = await query.Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
			long Tcount = ((IQueryResultEnumerable<History>)result).TotalCount;

			if (Tcount==0)
			{
				Debug.WriteLine("Load " + typeof(T) + " null -> Syncing");
				await InitSync();
				result = await query.Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
			}


			foreach (History item in result)
			{
				//History _temp = (History)(object)item;

				Hitems.Add((History)item);
				Settings.CurrentUser.Points += item.EarnPoints;
				count++;
				Debug.WriteLine("LoadAsync:Load(History) Type History: " + item.EarnPoints);

			};
			Debug.WriteLine("LoadAsync:Load(History) Count of entries: " + count );
                                   
			//var result = await _client.GetData();

			//foreach (var item in result)
			//{

			

			//}

			return Hitems;
		}

		public async Task<ObservableCollection<Rewards>> Load(Rewards _rewards)
		{
			ObservableCollection<Rewards> Ritems;
			Ritems = new ObservableCollection<Rewards>();

			Ritems.Clear();

			IMobileServiceSyncTable<Rewards> _tabla = (IMobileServiceSyncTable<Rewards>)_client.GetTable();

			var result = await _tabla.Where(rewards => rewards.Place == _rewards.Place).ToEnumerableAsync();

			if (!Utils.IsAny(result))
			{
				Debug.WriteLine("Load " + typeof(T) + " null -> Syncing");
				await InitSync();
				result = await _tabla.Where(rewards => rewards.Place == _rewards.Place).ToEnumerableAsync();
			}

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
                Branch = _history.Branch
				                   
			};

			IMobileServiceSyncTable<History> Tabla = (IMobileServiceSyncTable<History>) _client.GetTable();

			await Tabla.InsertAsync(Datos);

			return true;
		}

		public async Task<ObservableCollection<History>> Load(string person)
		{
			ObservableCollection<History> Hitems;
			Hitems = new ObservableCollection<History>();

			Hitems.Clear();
			IMobileServiceSyncTable<History> _tabla = (IMobileServiceSyncTable<History>)_client.GetTable();

			var result = await _tabla.Where(history => history.Person == person)
			                         .Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
			if (!Utils.IsAny(result))
			{
				Debug.WriteLine("Load History null -> Syncing");
				await InitSync();
				result = await _tabla.Where(history => history.Person == person)
									 .Take(1000).OrderByDescending(x => x.DateTime).ToEnumerableAsync();
			}
			foreach (History item in result)
			{

					Hitems.Add((History)item);

			}

			return Hitems;
		}

		public async Task<ObservableCollection<Place>> Load(ObservableCollection<Place> _places)
		{
			//InitSync();
			Debug.WriteLine("Entro al Load(Places)");
			ObservableCollection<Place> Pitems;
			Pitems = new ObservableCollection<Place>();

			Pitems.Clear();

			IEnumerable<Place> result = (IEnumerable<Place>) await _client.GetData();
			if (!Utils.IsAny(result))
			{
				Debug.WriteLine("Load Places null -> Syncing");
				await InitSync();
				result = (IEnumerable<Place>)await _client.GetData();

			}
			foreach (Place item in result)
			{
				Debug.WriteLine("Load Places -> " + item.Name);
				item.Logo = Settings.ImgSrvProd + item.Logo;
				if (item.Admin == "YES")
				{	if (Settings.CurrentUser.IsAdmin)
						Pitems.Add((Place)item);
				}
				else
					Pitems.Add((Place)item);
			}

			return Pitems;
		}

		public async Task InitSync()
		{
			await _client.SyncAsync();
		}

	}
}
