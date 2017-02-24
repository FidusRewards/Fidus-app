using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace fidus
{
	public class MainViewModel : BaseViewModel, INotifyPropertyChanged
	{
		private ObservableCollection<Place> _item;
		public Command ScanButtonCommand { get; set; }
		public Command RefreshCommand { get; set; }
		ICommand tapCommand;
		public Command SettingsCommand { get; set; }
		public Command ExitCommand { get; set; }
		private AzureClient<WhiteList> _client;
		private LoadAsync<WhiteList> _itemsW;
		private LoadAsync<History> LoadHistory;
		private string cuser, cmail;
		public string CurrUser { get { return cuser; } set { cuser = value; OnPropertyChanged(); } }
		public string CurrUmail { get { return cmail; } set { cmail = value; OnPropertyChanged(); } }
		private int counter, index;

		public ObservableCollection<Place> PItems
		{
			get { return _item; }
			set { _item = value; OnPropertyChanged(); }
		}

		public MainViewModel()
		{
			PItems = new ObservableCollection<Place>();
			ScanButtonCommand = new Command(ScanCommand);
			RefreshCommand = new Command(Load1);
			tapCommand = new Command(OnTapped);
			//SettingsCommand = new Command(SettingsTap);
			//ExitCommand = new Command(ExitTap);
			_client = new AzureClient<WhiteList>();
			LoadHistory = new LoadAsync<History>();

			Debug.WriteLine("MainPage : Antes del IF del DoLogin");


		}

		private void ScanCommand()
		{
			MessagingCenter.Send(this, "ScanRequest");
		}

		private async Task InitDb()
		{
			LoadAsync<Place> LoadItems = new LoadAsync<Place>();
			await LoadItems.InitSync(Helpers.Settings.UserEmail + "InitDb");

			Helpers.Settings.AllPlaces = await LoadItems.Load(Helpers.Settings.AllPlaces);
			index = 0;

			if (Helpers.Settings.AllPlaces.IsAny())
			{
				foreach (Place item in Helpers.Settings.AllPlaces)
				{
					if (Helpers.Settings.AllPlaces[index].Logo != null)
					{
						string fullpath = Helpers.Settings.ImgSrvProd + Helpers.Settings.AllPlaces[index].Logo;
						Helpers.Settings.AllPlaces[index].Logo = fullpath;
					}
					index++;
				}
			}
			else
				Debug.WriteLine(" -- Error de Inicio al Cargar Comercios -- ");

			Debug.WriteLine("InitDB en Main ejecución nro: " + counter);
			counter++;
			return;
		}

		public void Load1()
		{
			Helpers.Settings.IsReturn = false;
			Load();
		}

		public async void Load()
		{
			var placeH = new History();
			var _places = new ObservableCollection<Place>();
			//await _client.PurgeData();

			//Places = new List<Place>();
			//await LoadItems.InitSync();
			CurrUser = Helpers.Settings.CurrentUser.Name;
			CurrUmail = Helpers.Settings.CurrentUser.Email;

			if (!Helpers.Settings.IsReturn || Helpers.Settings.IsBoot)
			{
				IsBusy = true;

				await InitDb();

				PItems.Clear();

				if (Helpers.Settings.AllPlaces.IsAny())
				{
					await LoadHistory.InitSync("history" + Helpers.Settings.UserEmail);

					if (Helpers.Settings.UserIsAdmin)
					{
						_places = new ObservableCollection<Place>(Helpers.Settings.AllPlaces);

					}
					else
					{
						_places = new ObservableCollection<Place>(Helpers.Settings.AllPlaces.Where(place => place.Admin == "NO"));
					}

					foreach (Place items in _places)
					{
						placeH.Person = Helpers.Settings.CurrentUser.Email;
						placeH.Place = items.Name;

						var hpoints = await LoadHistory.Load(placeH);

						_places[_places.IndexOf(items)].Points = hpoints;

					}

					if (_places != null)
					{
						Helpers.Settings.AllPlaces = _places;
						PItems = _places;
						//MessagingCenter.Send(this, "Loaded", PItems);
					}
					else
						MessagingCenter.Send(this, "NotLoaded");
				}
				else
				{
					MessagingCenter.Send(this, "NotLoaded");
				}
				Helpers.Settings.IsReturn = true;

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
			var _place = (Place)place;
			String[] _array = { _place.Name, _place.Logo };
			MessagingCenter.Send(this, "Rewards", _array);

		}


		public async Task<bool> ConfirmQRCode(string place, string branch, string qrcode)
		{
			IsBusy = true;
			_itemsW = new LoadAsync<WhiteList>();


			await _itemsW.InitSync();

			IMobileServiceSyncTable<WhiteList> _tabla = _client.GetTable();

			try
			{

				var query = _tabla.CreateQuery().IncludeTotalCount().Where(whitelist => whitelist.Place == place && whitelist.Branch == branch && whitelist.ExchangeCode == qrcode);
				var result = await query.ToEnumerableAsync();

				if (result.FirstOrDefault() != null)
				{
					var tempcheck = IsQRtempValid(result.FirstOrDefault());
					IsBusy = false;

					if (tempcheck)
					{
						if (qrcode.StartsWith("T", StringComparison.CurrentCultureIgnoreCase))
						{
							await _tabla.DeleteAsync(result.FirstOrDefault());
						}
						return true;
					}
					return false;

				}

				IsBusy = false;
				return false;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("MainVM: Error en Conexíon qrcode" + ex);
				MessagingCenter.Send(this, "NOINET");

				IsBusy = false;
				return false;
			}
		}


		public void UpdatePoints(String[] points)
		{

			History _Uhistory = new History();

			_Uhistory.DateTime = DateTime.Now.ToLocalTime();
			_Uhistory.EarnPoints = Convert.ToInt32(points[1]);
			_Uhistory.Place = points[0];
			_Uhistory.IsDebit = false;
			_Uhistory.Person = Helpers.Settings.UserEmail;
			_Uhistory.Branch = points[4];
			_Uhistory.ExchangeCode = points[3];
			string _placelogo = Helpers.Settings.ImgSrvProd + points[2];

			string puntos = points[1];

			String[] _array2 = { points[0], puntos, _placelogo, points[5] };

			Helpers.Settings.Hitem = _Uhistory;

			MessagingCenter.Send(this, "Thanks", _array2);

		}

		public bool IsQRtempValid(WhiteList qrcode)
		{
			TimeSpan difference;
			bool flag = false;

			if (qrcode != null)
			{

				try
				{
					// CONSULTA DE LOS REGISTROS EN LOS QUE EL CODIGO QR SEA = AL ESCANEADO, EL ESCANEO SE HAYAN HECHO HOY 
					//  Y LA PERSONA QUE ESCANEO ES EL QUE AHORA ESTA ESCANEANDO      
									
					var _qrdatetime = Helpers.Settings.QRLastTimes.Split(',');
					var _qrbranch = Helpers.Settings.QRLastBranches.Split(',');
					Helpers.Settings.qrdate.Clear();
					Helpers.Settings.qrbranch.Clear();
					       
					if (!_qrdatetime.Contains("nada"))
					{
						for (int i = 0; i < _qrbranch.Count(); i++)
						{
							
						
							DateTime convertedDate = DateTime.SpecifyKind(
									DateTime.Parse(_qrdatetime[i]),
									DateTimeKind.Local);
							if (_qrbranch[i] == qrcode.Branch)  //_qrlastcode[i]==qrcode.ExchangeCode && 
							{
								difference = (DateTime.Now.ToLocalTime() - convertedDate);
							}
							else{
								difference = TimeSpan.Parse("01:01:00");
							}

							if (i > 0)
							{
								_qrbranch[i] = "," + _qrbranch[i];
								_qrdatetime[i] = "," + _qrdatetime[i];
							}
							Helpers.Settings.qrdate.Enqueue(_qrdatetime[i]);
							Helpers.Settings.qrbranch.Enqueue(_qrbranch[i]);

							if (difference.Hours < 1)
							{
								flag=true;
							}

						}
						Helpers.Settings.qrdate.Enqueue(","+DateTime.Now.ToString());
						Helpers.Settings.qrbranch.Enqueue(","+qrcode.Branch);

						if (Helpers.Settings.qrbranch.Count > Helpers.Settings.CantQR)
						{
							Helpers.Settings.qrdate.Dequeue();
							Helpers.Settings.qrbranch.Dequeue();
						}

						if (flag)
							return false;
						else
							return true;
					}

					Helpers.Settings.qrbranch.Enqueue(qrcode.Branch);
					Helpers.Settings.qrdate.Enqueue(DateTime.Now.ToString());
					return true;


				}
				catch (Exception ex)
				{
					Debug.WriteLine("MainVM: Error en Conexíon qrcode" + ex);
					MessagingCenter.Send(this, "NOINET");
					return false;
				}
			}
			return false;
		}
	}
}
