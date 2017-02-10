using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace fidus
{


	public partial class SplashScreen : ContentPage
	{
		public SplashScreen()
		{
			InitializeComponent();
			FSplash.BackgroundColor = Color.FromHex(Settings.FidusColor);
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			Page content = new MainPage();

			// await a new task
			await Task.Factory.StartNew(async () =>
			{

				// delay for a few seconds on the splash screen
				await Task.Delay(3000);

				var mainPage = new NavigationPage(content)
				{
					BarBackgroundColor = Color.FromHex(Settings.FidusColor),
					BarTextColor = Color.White
					//Title="Ingreso"
				};

				// on the main UI thread, set the MainPage to the navPage
				Device.BeginInvokeOnMainThread(() =>
				{
					Application.Current.MainPage = mainPage;
				});
			});


		}
	}
}
