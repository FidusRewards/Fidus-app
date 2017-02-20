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
			//NavigationPage.SetTitleIcon(this, "fidus_text.png");
			//this.Title = "Volver";
			InitializeComponent();


			EImage.Source = place.Logo;
			EImage.HeightRequest = 62;
			EImage.WidthRequest = 62;

			EName.Text = place.Name;
			EName.FontSize = 20;
			EName.FontAttributes = FontAttributes.Bold;
			EName.TextColor = Color.Black;

			PCat.Text = place.Category;
			PCat.FontSize = 18;
			PCat.TextColor = Color.Gray;


			reName.Text = selectedreward.Name;

			reImg.Source = selectedreward.Photo;

			reDate.Text = DateTime.Now.ToLocalTime().ToString("U");
			reDate.HorizontalOptions = LayoutOptions.CenterAndExpand;

			//Settings.CurrentUser.Points += -1*selectedreward.ReqPoints;

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
