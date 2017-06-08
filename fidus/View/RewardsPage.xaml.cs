using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms;

namespace fidus
{
	public partial class RewardsPage : ContentPage
	{
		private Place Rplace=new Place();
		private RewardsViewModel rVM;
		private ObservableCollection<Place> pepe;

		public RewardsPage(Place place)
		{
			//int count = 0, index=0;
			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Volver";
			InitializeComponent();
			Helpers.Settings.IsReturn = true;

			rVM = new RewardsViewModel(place.Name);

			BindingContext = rVM;
			Rplace = place;

			//foreach (Place _place in Settings.AllPlaces)
			//{
			//	if (_place.Name == place.Name)
			//		 index = count;
			// 	count++;
			//
			//}
			Helpers.Settings.UserPoints = place.Points;//Settings.AllPlaces[index].Points;
			Debug.WriteLine("RewardsPage: Construct - Place: :" + rVM.Place);

			MessagingCenter.Subscribe<RewardsViewModel>(this, "Loaded", (obj) => {
				//UserName.Text = Settings.CurrentUser.Name;
				UserPoints.Text = Helpers.Settings.UserPoints.ToString();
				Debug.WriteLine("RewardsPage: Construct - Actualicé " + Helpers.Settings.CurrentUser.Name 
				                + " " + UserPoints.Text);
				rVM.IsBusy = false;
			});
			MessagingCenter.Subscribe<RewardsViewModel>(this, "NotLoaded", async (obj) => {
				await DisplayAlert("Error", "Problemas de Conexión", "OK");
			});

			MessagingCenter.Subscribe<RewardsViewModel>(this, "NOINET", async (obj) =>
			{
				//Helpers.Settings.IsInternetEnabled = false;
				await DisplayAlert("Advertencia", "No hay conexión a Internet. Algunas funciones pueden no estar habilitadas", "OK");
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
				TextColor = Color.FromHex(Helpers.Settings.FidusColor),
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

			await Navigation.PushAsync(new RewardsDetailsPage(selected, Rplace ) );
			//selected = null;
			//DisplayAlert("Reward", selected, "OK");
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			//Settings.CurrentUser.Points = 0;
			rVM.IsBusy = true;
			rVM.Load();
			pepe = Helpers.Settings.AllPlaces;


		}
		protected override bool OnBackButtonPressed()
		{
			Helpers.Settings.AllPlaces = pepe;
			return base.OnBackButtonPressed();
		}
	}
}
