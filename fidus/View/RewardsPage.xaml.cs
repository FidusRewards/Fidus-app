using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace fidus
{
	public partial class RewardsPage : ContentPage
	{
		private RewardsViewModel rVM;
		private Place Rplace;

		public RewardsPage(string name, string logo)
		{
			InitializeComponent();

			rVM = new RewardsViewModel(name);

			BindingContext = rVM;


			Rplace = new Place();
			Rplace.Name = name;
			Rplace.Logo = logo;

			Debug.WriteLine("RewardsPage: Construct - Place: :" + rVM.Place);
			MessagingCenter.Subscribe<RewardsViewModel>(this, "Loaded", (obj) => {
				//UserName.Text = Settings.CurrentUser.Name;
				UserPoints.Text = Settings.CurrentUser.Points.ToString();
				Debug.WriteLine("RewardsPage: Construct - Actualicé " + Settings.CurrentUser.Name 
				                + " " + UserPoints.Text);
			});
			MessagingCenter.Subscribe<RewardsViewModel>(this, "NotLoaded", async (obj) => {
				await DisplayAlert("Error", "Problemas de Conexión", "OK");
			});

			PImage.Source = Rplace.Logo;
			PImage.HeightRequest = 50;
			PImage.WidthRequest = 50;
			PName.Text = Rplace.Name;
			PName.VerticalOptions = LayoutOptions.Center;
			PName.FontAttributes = FontAttributes.Bold;
			PName.FontSize = 30;
			var ScrollTitle = new Label
			{
				Text = "Recompensas Disponibles",
				TextColor = Color.FromHex(Settings.FidusColor),
				FontAttributes = FontAttributes.Bold,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 0, 10, 0)
			};
			var TitleImg = new Image
			{
				Source = ImageSource.FromResource("fidus.rewardsicon.png"),
				WidthRequest = 25,
				HeightRequest = 25
			};
			RewardsTittle.Children.Add(ScrollTitle);
			RewardsTittle.Children.Add(TitleImg);

		}

		async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			Rewards selected = (e.SelectedItem as Rewards);

			await Navigation.PushAsync(new RewardsDetailsPage(selected, Rplace ) { Title=selected.Name});
			//selected = null;
			//DisplayAlert("Reward", selected, "OK");
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Settings.CurrentUser.Points = 0;

			rVM.Load();

		}
	}
}
