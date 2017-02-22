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
			_client = AzureClient<T>.defaultInstance;  //new AzureClient<T>();

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

		public async Task<int> Load(History _history)
		{

			IMobileServiceSyncTable<History> _tabla = (IMobileServiceSyncTable<History>) _client.GetTable();
			
			int count = 0;
			var query = _tabla.CreateQuery().IncludeTotalCount().Take(1000).Where(history => history.Place.Contains(_history.Place) && history.Person == _history.Person);
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
                Branch = _history.Branch,
				Rating = _history.Rating                   
			};

			IMobileServiceSyncTable<History> Tabla = (IMobileServiceSyncTable<History>) _client.GetTable();

			await Tabla.InsertAsync(Datos);
			await _client.Push();

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
			//await InitSync();
			Debug.WriteLine("Entro al Load(Places)");
			ObservableCollection<Place> Pitems;
			Pitems = new ObservableCollection<Place>();

			Pitems.Clear();
			//IMobileServiceSyncTable<Place> _tabla = (IMobileServiceSyncTable<Place>)_client.GetTable();

				//await _client.PurgeData();

			IEnumerable<Place> result = (IEnumerable<Place>) await _client.GetData();
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

		public async Task InitSync(string qName=null)
		{
			await _client.SyncAsync(qName);
		}

		public async void PurgeTable()
		{
			await _client.PurgeData();
		}

	}
}
