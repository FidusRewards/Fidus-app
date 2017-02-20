using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace fidus
{
	public partial class App : Application
	{
		public static App instance;
		private LoadAsync<Person> _client;
		private Page content;

		public App()
		{
			InitializeComponent();

			instance = this;

			//var _rewards = new Rewards();
			//_rewards.Name = "Un Zapato Interesante";
			//_rewards.Photo = "https://fidusimgsrv.blob.core.windows.net/rewards/Bartok-BBQbondgratis.jpg";
			//_rewards.Place = "Bartok";
			//_rewards.ReqPoints = 10;
			//_rewards.Description = "No se que dice aca, pero es un texto muy largo que me va a dejar ver como qued";

			//var _place = new Place();
			//_place.Name="Bartok";
			//_place.Logo = "https://fidusimgsrv.blob.core.windows.net/logos/Bartok_logo.png";
			//_place.Category = "Resto Bar";
			//_place.Points = 20;
			content = new MainPage();


			if (Device.OS == TargetPlatform.Android)
			{
				MainPage = new SplashScreen(content);
			}
			else
			{

				MainPage =	new NavigationPage(content)
				{
					BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
					BarTextColor = Color.White
					//Title="Fidus"
				};
			}
				//MainPage = new loginPage();
		}
		public static void CleanProperties()
		{
			Application.Current.Properties.Clear();
		}
		public async static void UpdateProperties()
		{
			if (!Application.Current.Properties.ContainsKey("UserEmail")){
				Application.Current.Properties.Add("UserEmail", Settings.CurrentUser.Email);
				Application.Current.Properties.Add("UserName", Settings.CurrentUser.Name);
				Application.Current.Properties.Add("UserBirthday", Settings.CurrentUser.Birthday);
				Application.Current.Properties.Add("UserId", Settings.CurrentUser.id);
				Application.Current.Properties.Add("UserVersion", Settings.CurrentUser.Version);
				Application.Current.Properties.Add("UserIsAdmin", Settings.CurrentUser.IsAdmin);
				Application.Current.Properties.Add("ULastLogin", Settings.CurrentUser.LastLogin);
				Application.Current.Properties.Add("ULogged", Settings.CurrentUser.Logged);
				Application.Current.Properties.Add("UserPass", Settings.CurrentUser.Pass);
				Application.Current.Properties.Add("UserPhone", Settings.CurrentUser.Phone);
				Application.Current.Properties.Add("UserPoints", Settings.CurrentUser.Points);
			}
			else
			{
				Application.Current.Properties["UserEmail"]= Settings.CurrentUser.Email;
				Application.Current.Properties["UserName"]= Settings.CurrentUser.Name;
				Application.Current.Properties["UserBirthday"]= Settings.CurrentUser.Birthday;
				Application.Current.Properties["UserId"]= Settings.CurrentUser.id;
				Application.Current.Properties["UserVersion"]= Settings.CurrentUser.Version;
				Application.Current.Properties["UserIsAdmin"]= Settings.CurrentUser.IsAdmin;
				Application.Current.Properties["ULastLogin"]= Settings.CurrentUser.LastLogin;
				Application.Current.Properties["ULogged"]= Settings.CurrentUser.Logged;
				Application.Current.Properties["UserPass"]= Settings.CurrentUser.Pass;
				Application.Current.Properties["UserPhone"]= Settings.CurrentUser.Phone;
				Application.Current.Properties["UserPoints"]= Settings.CurrentUser.Points;
			}
			await Application.Current.SavePropertiesAsync();

		}

		public static void UpdateUSettings()
		{ 
			
			Settings.CurrentUser.Email= Application.Current.Properties["UserEmail"] as string;
			Settings.CurrentUser.Name = Application.Current.Properties["UserName"] as string;
			Settings.CurrentUser.Birthday = (DateTime)Application.Current.Properties["UserBirthday"];
			Settings.CurrentUser.id = Application.Current.Properties["UserId"] as string;
			Settings.CurrentUser.Version = Application.Current.Properties["UserVersion"] as string;
			Settings.CurrentUser.IsAdmin = (bool)Application.Current.Properties["UserIsAdmin"];
			Settings.CurrentUser.LastLogin = (DateTime)Application.Current.Properties["ULastLogin"];
			Settings.CurrentUser.Logged = (bool)Application.Current.Properties["ULogged"];
			Settings.CurrentUser.Pass = Application.Current.Properties["UserPass"] as string;
			Settings.CurrentUser.Phone = Application.Current.Properties["UserPhone"] as string;
			Settings.CurrentUser.Points = (int)Application.Current.Properties["UserPoints"];
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			//Settings.Default.IsLogin = await IsLoggedIn()


			base.OnStart();

		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps

			UpdateProperties();
			//await _client.InitSync();
			Debug.WriteLine("OnSleep");
			base.OnSleep();
		}

		protected override void OnResume()
		{
			// Handle when your app resumes

			if (Application.Current.Properties.ContainsKey("UserEmail"))
			{
				UpdateUSettings();
				Settings.IsLogin = false;
			}
			else
			{
				Settings.IsLogin = true;
				UpdateProperties();
			}

			Debug.WriteLine("OnResume");
			base.OnResume();
		}
	}
}
