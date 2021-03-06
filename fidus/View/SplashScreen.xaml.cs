﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace fidus
{


	public partial class SplashScreen : ContentPage
	{
		private Page Mcontent;

		public SplashScreen(Page content)
		{
			Mcontent = content;
			InitializeComponent();
			FSplash.BackgroundColor = Color.FromHex(Helpers.Settings.FidusColor);
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			//Page content = new MainPage();

			// await a new task
			await Task.Factory.StartNew(async () =>
			{

				// delay for a few seconds on the splash screen
				await Task.Delay(3000);

				var mainPage = new NavigationPage(Mcontent)
				{
					BarBackgroundColor = Color.FromHex(Helpers.Settings.FidusColor),
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
