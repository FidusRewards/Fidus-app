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

			content = new QualifyPage();//new MainPage();

			if (Device.OS == TargetPlatform.Android)
			{
				MainPage = new SplashScreen();
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
				await Application.Current.SavePropertiesAsync();
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
				await Application.Current.SavePropertiesAsync();
			}
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
			//Settings.Default.IsLogin = await IsLoggedIn();
			if (Application.Current.Properties.ContainsKey("UserEmail"))
			{
				UpdateUSettings();
			}
			else {
				UpdateProperties();
			}
				Debug.WriteLine("OnStart");

		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps

			UpdateProperties();
			//await _client.InitSync();
			Debug.WriteLine("OnSleep");

		}

		protected override void OnResume()
		{
			// Handle when your app resumes
			Debug.WriteLine("OnResume");

		}
	}
}
