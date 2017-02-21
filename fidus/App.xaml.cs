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

			#region For Testing
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

			#endregion

			content = new MainPage();
			Debug.WriteLine("App.cs Settings : " + Helpers.Settings.CurrentUser.Email);


			if (Device.OS == TargetPlatform.Android)
			{
				MainPage = new SplashScreen(content);
			}
			else
			{

				MainPage =	new NavigationPage(content)
				{
					BarBackgroundColor = Color.FromHex(Helpers.Settings.FidusColor), //A13B35
					BarTextColor = Color.White
					//Title="Fidus"
				};
			}
				//MainPage = new loginPage();
		}


		protected override void OnStart()
		{
			// Handle when your app starts
	

			Debug.WriteLine("OnResume");
			Debug.WriteLine("Settings : " + Helpers.Settings.UserEmail);
			base.OnStart();

		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps

		;
			Debug.WriteLine("OnSleep");
			Debug.WriteLine("Settings : " + Helpers.Settings.UserEmail);

			base.OnSleep();
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		


			Debug.WriteLine("OnResume");
			Debug.WriteLine("Settings : " + Helpers.Settings.UserEmail);

			base.OnResume();
		}
	}
}
