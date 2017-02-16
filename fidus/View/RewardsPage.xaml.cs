using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace fidus
{
	public partial class RewardsPage : ContentPage
	{
		private Place Rplace=new Place();
		private RewardsViewModel rVM;

		public RewardsPage(Place place)
		{
			int count=0, index=0;
			InitializeComponent();

			rVM = new RewardsViewModel(place.Name);

			BindingContext = rVM;
			Rplace = place;

			foreach (Place _place in Settings.AllPlaces)
			{
				if (_place.Name == place.Name)
					 index = count;
			 	count++;

			}
			UserPoints.Text = place.Points.ToString();//Settings.AllPlaces[index].Points.ToString();
			Settings.CurrentUser.Points = place.Points;//Settings.AllPlaces[index].Points;
			Debug.WriteLine("RewardsPage: Construct - Place: :" + rVM.Place);

			MessagingCenter.Subscribe<RewardsViewModel>(this, "Loaded", (obj) => {
				//UserName.Text = Settings.CurrentUser.Name;
				UserPoints.Text = Settings.CurrentUser.Points.ToString();
				Debug.WriteLine("RewardsPage: Construct - Actualicé " + Settings.CurrentUser.Name 
				                + " " + UserPoints.Text);
				IsBusy = false;
			});
			MessagingCenter.Subscribe<RewardsViewModel>(this, "NotLoaded", async (obj) => {
				await DisplayAlert("Error", "Problemas de Conexión", "OK");
			});

			PImage.Source = place.Logo;
			PImage.HeightRequest = 62;
			PImage.WidthRequest = 62;

			PName.Text = place.Name;
			PName.FontSize = 20;
			PName.TextColor = Color.Black;

			PArea.Text = place.Category;
			PArea.FontSize = 18;
			PArea.TextColor=Color.Gray;

			var ScrollTitle = new Label
			{
				Text = "Recompensas Disponibles",
				TextColor = Color.FromHex(Settings.FidusColor),
				FontSize = 16,
				FontAttributes = FontAttributes.Bold,
				VerticalOptions= LayoutOptions.Center,
				Margin = new Thickness(0, 0, 10, 0)
			};
			var TitleImg = new Image
			{
				Source = ImageSource.FromResource("fidus.rewardsicon.png"),
				WidthRequest = 25,
				HeightRequest = 25,
				VerticalOptions=LayoutOptions.Center,
					
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
			//Settings.CurrentUser.Points = 0;
			IsBusy = true;
			rVM.Load();

		}
	}
}
