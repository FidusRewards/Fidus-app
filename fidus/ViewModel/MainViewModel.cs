using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace fidus
{
	public class MainViewModel: BaseViewModel, INotifyPropertyChanged
	{
		private string _name;
		private double _size;
		private ObservableCollection<Place> _item;
		public Command ScanButtonCommand { get; set; }
		public Command RefreshCommand { get; set; }
		public LoadAsync<Place> Items;
		ICommand tapCommand;
		public Command SettingsCommand { get; set;}
		public Command ExitCommand { get; set; }
        private AzureClient<WhiteList> _client;

        public ObservableCollection<Place> Places
		{
			get { return _item; }
			set { _item = value; OnPropertyChanged(); }
		}

		public string Mname
		{
			get { return _name; }
			set { _name= value; OnPropertyChanged(); }
		}
		public double Msize
		{
			get { return _size; }
			set { _size = value; OnPropertyChanged(); }
		}

		public MainViewModel()
		{
			Items = new LoadAsync<Place>();
			Places = new ObservableCollection<Place>();
			ScanButtonCommand = new Command(ScanCommand);
			RefreshCommand = new Command(Load);
			tapCommand = new Command(OnTapped);
			SettingsCommand = new Command(SettingsTap);
			ExitCommand = new Command(ExitTap);
            _client = new AzureClient<WhiteList>();

			Debug.WriteLine("MainPage : Antes del IF del DoLogin");


        }

        private void ScanCommand() {
			MessagingCenter.Send(this, "ScanRequest");
		}

		public async void Load()
		{
			//Places = new List<Place>();
			//Places.Clear();
			Places=await Items.Load(Places);
			IsBusy = false;

			if (Places != null)
				MessagingCenter.Send(this, "Loaded", Places);
			else
				MessagingCenter.Send(this, "NotLoaded");
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
		void SettingsTap()
		{
			//DependencyService.Get<ICloseApplication>().closeApp();

			MessagingCenter.Send(this, "Settings");

		}
		void ExitTap()
		{
			//DependencyService.Get<ICloseApplication>().closeApp();
			Debug.WriteLine("MainVW: exit command");
			MessagingCenter.Send(this, "Exit");

		}

        public async Task<bool> ConfirmQRCode(string place, string branch, string qrcode)
        {
            IMobileServiceTable<WhiteList> _tabla = _client.GetTable();

            try
            {
                IEnumerable<WhiteList> result = await _tabla.Where(whitelsit => whitelsit.Place == place && whitelsit.Branch == branch && whitelsit.ExchangeCode == qrcode).ToEnumerableAsync();
                
                if (result.GetEnumerator().MoveNext())
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


        public async void UpdatePoints(String[] points)
		{

			var _items = new LoadAsync<History>();
			History _history = new History();

			_history.DateTime = DateTime.Now;
			_history.EarnPoints =  Convert.ToInt32(points[1]);
			_history.Place = points[0];
			_history.IsDebit = false;
			_history.Person = Settings.CurrentUser.Email;
            _history.Branch = points[4];
            _history.ExchangeCode = points[3];
			string _placelogo = Settings.ImgSrvProd + points[2];
			String[] _array2 = { points[0], _placelogo};
			await _items.Save(_history);

			MessagingCenter.Send(this, "Rewards1", _array2);

			IsBusy = false;
		}
	}

}
