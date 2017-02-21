using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;
using Plugin.Connectivity;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;

namespace fidus
{
	public class MainViewModel: BaseViewModel, INotifyPropertyChanged
	{
		private ObservableCollection<Place> _item;
		public Command ScanButtonCommand { get; set; }
		public Command RefreshCommand { get; set; }
		public LoadAsync<Place> LoadItems;
		ICommand tapCommand;
		public Command SettingsCommand { get; set;}
		public Command ExitCommand { get; set; }
        private AzureClient<WhiteList> _client;
		private LoadAsync<WhiteList> _itemsW;
        private LoadAsync<History> LoadHistory;
		private ObservableCollection<History> _history;
		private string cuser, cmail;
		public string CurrUser { get { return cuser;} set { cuser = value; OnPropertyChanged();} }
		public string CurrUmail { get{ return cmail; } set { cmail = value; OnPropertyChanged(); } }

		public ObservableCollection<Place> PItems
		{
			get { return _item; }
			set { _item = value; OnPropertyChanged(); }
		}

		public MainViewModel()
		{
			LoadItems = new LoadAsync<Place>();
			PItems = new ObservableCollection<Place>();
			ScanButtonCommand = new Command(ScanCommand);
			RefreshCommand = new Command(Load);
			tapCommand = new Command(OnTapped);
			//SettingsCommand = new Command(SettingsTap);
			//ExitCommand = new Command(ExitTap);
            //_client = new AzureClient<WhiteList>();
			LoadHistory = new LoadAsync<History>();
			_history = new ObservableCollection<History>();


			Debug.WriteLine("MainPage : Antes del IF del DoLogin");


        }

        private void ScanCommand() {
			MessagingCenter.Send(this, "ScanRequest");
		}

		public async void Load()
		{
			History placeH = new History();
			//await _client.PurgeData();

			//Places = new List<Place>();
			//await LoadItems.InitSync();
			if (Helpers.Settings.CurrentUser.Email != null)
			{
				CurrUser = Helpers.Settings.CurrentUser.Name;
				CurrUmail = Helpers.Settings.CurrentUser.Email;

				IsBusy = true;

				if (!Helpers.Settings.IsReturn || Helpers.Settings.IsBoot)
				{
					PItems.Clear();

					Helpers.Settings.AllPlaces = await LoadItems.Load(Helpers.Settings.AllPlaces);

					_history.Clear();
					await LoadHistory.InitSync();

					foreach (Place _place in Helpers.Settings.AllPlaces)
					{
						placeH.Person = Helpers.Settings.CurrentUser.Email;
						placeH.Place = _place.Name;

						_history = await LoadHistory.Load(placeH);

						Helpers.Settings.AllPlaces[Helpers.Settings.AllPlaces.IndexOf(_place)].Points = Helpers.Settings.UserPoints;

					}
					Helpers.Settings.IsLogin = false;
					PItems = Helpers.Settings.AllPlaces;

				}
				Helpers.Settings.IsReturn = false;

				if (PItems != null)
					MessagingCenter.Send(this, "Loaded", PItems);
				else
					MessagingCenter.Send(this, "NotLoaded");
			}
			IsBusy = false;
			Helpers.Settings.IsBoot = false;
		}

		public ICommand TapCommand
		{
			get { return tapCommand; }
		}


		void OnTapped(object place)
		{
			Place _place = (Place) place;
			String[] _array = { _place.Name, _place.Logo };
			MessagingCenter.Send(this, "Rewards", _array);

		}


        public async Task<bool> ConfirmQRCode(string place, string branch, string qrcode)
        {
			_itemsW = new LoadAsync<WhiteList>();


			await _itemsW.InitSync();

			IMobileServiceSyncTable<WhiteList> _tabla = _client.GetTable();

            try
            {

				var result = (await _tabla.Where(whitelist => whitelist.Place == place && whitelist.Branch == branch && whitelist.ExchangeCode == qrcode)
															.Take(1)
				              .ToEnumerableAsync()).FirstOrDefault();                
               if (result!=null)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainVM: Error en Conexíon qrcode" + ex);
                MessagingCenter.Send(this, "NOINET");
                return false;
            }
        }


        public void UpdatePoints(String[] points)
		{

			History _history = new History();

			_history.DateTime = DateTime.Now.ToLocalTime();
			_history.EarnPoints =  Convert.ToInt32(points[1]);
			_history.Place = points[0];
			_history.IsDebit = false;
			_history.Person = Helpers.Settings.CurrentUser.Email;
            _history.Branch = points[4];
            _history.ExchangeCode = points[3];
			string _placelogo = Helpers.Settings.ImgSrvProd + points[2];

			string puntos = points[1];

			//String[] _array2 = { points[0], _placelogo};
			String[] _array2 = { points[0], puntos, _placelogo, points[5] };

			Helpers.Settings.Hitem = _history;

			//await _itemsH.Save(_history);


			//MessagingCenter.Send(this, "Rewards1", _array2);
			MessagingCenter.Send(this, "Thanks", _array2);

		}
	}

}
