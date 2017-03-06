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
using Plugin.Connectivity;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;

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

		public static MobileServiceClient _client = new MobileServiceClient(Helpers.Settings.AzureUrl);

		//private LoadAsync<WhiteList> _itemsW = new LoadAsync<WhiteList>(_client);
		//private LoadAsync<History> LoadHistory = new LoadAsync<History>(_client);

		private string cuser, cmail;
		public string CurrUser { get { return cuser; } set { cuser = value; OnPropertyChanged(); } }
		public string CurrUmail { get { return cmail; } set { cmail = value; OnPropertyChanged(); } }
		private int counter, index;

		public static MobileServiceClient _mclientPl = new MobileServiceClient(Helpers.Settings.AzureUrl);
		private LoadAsync<Place> _clientPl = new LoadAsync<Place>(_mclientPl);
		public static MobileServiceClient _mclientH = _mclientPl;//new MobileServiceClient(Helpers.Settings.AzureUrl);
		private LoadAsync<History> _clientH = new LoadAsync<History>(_mclientH);
		public static MobileServiceClient _mclientW = _mclientPl;//new MobileServiceClient(Helpers.Settings.AzureUrl);
		private LoadAsync<WhiteList> _clientW = new LoadAsync<WhiteList>(_mclientW);

		private IMobileServiceTable<Person> _tablaP;

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
			_tablaP = _client.GetTable<Person>();

			Debug.WriteLine("MainPage : Antes del IF del DoLogin");


		}

		private void ScanCommand()
		{
			MessagingCenter.Send(this, "ScanRequest");
		}

		private async Task InitDb()
		{
			
			var dif = (DateTime.Now - Helpers.Settings.LastPlaceInit.ToLocalTime()).TotalMinutes;
			if (CrossConnectivity.Current.IsConnected)
			{	if (dif > 10)
					{
					var synctokenPL = "Places" + Helpers.Settings.UserID;
					await _clientPl.SyncAsync(synctokenPL);
					Helpers.Settings.LastPlaceInit = DateTime.Now.ToLocalTime();
					}
			}else
				MessagingCenter.Send(this, "NOINET");
			
			Helpers.Settings.AllPlaces = await _clientPl.Load(Helpers.Settings.AllPlaces);
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

			CurrUser = Helpers.Settings.CurrentUser.Name;
			CurrUmail = Helpers.Settings.CurrentUser.Email;

			if (!Helpers.Settings.IsReturn || Helpers.Settings.IsBoot)
			{
				IsBusy = true;

				await InitDb();

				PItems.Clear();

				if (Helpers.Settings.AllPlaces.IsAny())
				{
					var dif = (DateTime.Now - Helpers.Settings.LastHistoryInit.ToLocalTime()).TotalMinutes;
					if (CrossConnectivity.Current.IsConnected)
					{	if (dif > 10)
							{	
							var synctokenH = "History" + Helpers.Settings.UserID;
							await _clientH.SyncAsync(synctokenH);

							if (Helpers.Settings.UserEmail!="fidus@com")
								Helpers.Settings.LastHistoryInit = DateTime.Now.ToLocalTime();
							}
					}else
							MessagingCenter.Send(this, "NOINET");
					
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

						var hpoints = await _clientH.Load(placeH);

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

			try
			{
				
				var dif = (DateTime.Now - Helpers.Settings.LastWhiteListInit.ToLocalTime()).TotalMinutes;
				if (CrossConnectivity.Current.IsConnected)
				{ if (dif > 10)
					{
						var synctokenW = "Whitelist" + Helpers.Settings.UserID;
						await _clientW.SyncAsync(synctokenW);//Helpers.Settings.UserID + "WhiteList");
						Helpers.Settings.LastWhiteListInit = DateTime.Now.ToLocalTime();
					}
				}else
					MessagingCenter.Send(this, "NOINET");
				
				var result = await _clientW.LoadW(place, branch, qrcode);

				if (result.FirstOrDefault() != null)
				{
					var tempcheck = IsQRtempValid(result.FirstOrDefault());
					IsBusy = false;

					if (tempcheck)
					{
						if (qrcode.StartsWith("T", StringComparison.CurrentCultureIgnoreCase))
						{
							await _clientW.Delete(result.FirstOrDefault());
						}
						return true;
					}
					return false;
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine("MainVM: Error en Conexíon qrcode" + ex);
				MessagingCenter.Send(this, "NOINET");

			}
			IsBusy = false;
			return false;
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
			_Uhistory.Reward = " ";
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

							if (difference.TotalHours < 1)
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

		public async void DoLogout()
		{ 
			try
			{
				//this.IsEnabled = false;
				//await App.instance.UpdateDB();
				IsBusy = true;
				Helpers.Settings.CurrentUser.Logged = false;

				if (CrossConnectivity.Current.IsConnected)
				{

					JObject data = new JObject {
								{ "id", Helpers.Settings.CurrentUser.id },
								{ "Logged", false }
							};
					await _tablaP.UpdateAsync(data);
				}

				Helpers.Settings.UserName = "fidus";
				Helpers.Settings.UserEmail = "fidus@com";
				Helpers.Settings.UserPoints = 0;
				Helpers.Settings.UserID = "AA";
				Helpers.Settings.AllPlaces.Clear();
				Helpers.Settings.Hitem.Place = "";
				Helpers.Settings.Hitem.id = "";

				//var pclient = new LoadAsync<Place>();
				//pclient.PurgeTable();
				Helpers.Settings.IsLogin = false;
				Helpers.Settings.IsReturn = false;
				//_client.CloseDB();
				Helpers.Settings.LastPlaceInit = DateTime.Parse("2017-01-01 00:00:01");
				Helpers.Settings.LastHistoryInit = DateTime.Parse("2017-01-01 00:00:01");
				Helpers.Settings.LastWhiteListInit = DateTime.Parse("2017-01-01 00:00:01");
				Helpers.Settings.LastRewardsInit = DateTime.Parse("2017-01-01 00:00:01");

				//this.HideWithoutAnimations();
				MessagingCenter.Send(this, "LOGOUT");
			}
			catch (Exception ex)
			{
				Debug.WriteLine("MainPage: Exit stack " + ex);
			}

			IsBusy = false;
			Debug.WriteLine("Menu : Logout");
		}
		

	}
}
