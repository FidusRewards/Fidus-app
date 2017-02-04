using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace fidus
{
	public partial class ExchangePage : ContentPage
	{
		public ExchangePage(Rewards selectedreward, Place place)
		{
			InitializeComponent();
			xChangeTit.BackgroundColor = Color.FromHex(Settings.FidusColor);
			xChangeLab.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

			EImage.Source = place.Logo;
			EImage.HeightRequest = 50;
			EImage.WidthRequest = 50;
			EName.Text = place.Name;
			EName.VerticalOptions = LayoutOptions.Center;
			EName.FontAttributes = FontAttributes.Bold;
			EName.FontSize = 30;

			Felicidades.TextColor = Color.FromHex(Settings.FidusColor);
			Felicidades.FontAttributes = FontAttributes.Italic;

			reName.Text = selectedreward.Name;
			reName.TextColor = Color.Black;
			reName.HorizontalOptions = LayoutOptions.CenterAndExpand;
			reName.FontSize = 20;

			reImg.Source = selectedreward.Photo;
			reImg.HorizontalOptions = LayoutOptions.CenterAndExpand;

			reDate.Text = DateTime.Now.ToString("U");
			reDate.HorizontalOptions = LayoutOptions.CenterAndExpand;

			OKBut.BackgroundColor = Color.FromHex(Settings.FidusColor);
			OKBut.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			Settings.CurrentUser.Points += selectedreward.ReqPoints;

		}

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
		protected override bool OnBackButtonPressed()
		{
			return true;
		}

	}
}
