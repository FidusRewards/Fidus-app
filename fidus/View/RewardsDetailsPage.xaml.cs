using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fidus
{
	public partial class RewardsDetailsPage : ContentPage
	{
		private RewardsDetailsViewModel dVM;
		private Rewards _reward;
		private Place _place;

		public RewardsDetailsPage(Rewards selectedReward, Place place)
		{
			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Volver";
			InitializeComponent();
			Helpers.Settings.IsReturn = true;


			BCanje.BackgroundColor = Color.FromHex(Helpers.Settings.FidusColor);
			BCanje.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			//BCanje.SetBinding(Button.CommandProperty, new Binding("debitCommand", 0));
			_place = place;
			_reward = selectedReward;

			dVM = new RewardsDetailsViewModel(_reward);

			BindingContext = dVM;

		 	PImage.Source = place.Logo;
			PImage.HeightRequest = 62;
			PImage.WidthRequest = 62;

			PName.Text = place.Name;
			PName.FontSize = 20;
			PName.FontAttributes = FontAttributes.Bold;
			PName.TextColor = Color.Black;

			PCat.Text = place.Category;
			PCat.FontSize = 18;
			PCat.TextColor = Color.Gray;


			Debug.WriteLine("RewardsDetailPage: Construct - Selected reward: "+_reward.Name);
			//MessagingCenter.Subscribe<RewardsDetailsViewModel, bool>(this, "Debited", (obj, isExchanged) => exchangeOK(isExchanged));

		}

		private async void OnExClicked(object obj, EventArgs args)
		{
			var canjea = await DisplayAlert("Confirmación", "¿Seguro que querés canjear?", "SI", "NO");
			Debug.WriteLine("RewardsDetail: Confirm - " + canjea);
			if (canjea)
			{
				var res = await dVM.OnExClicked();
				if (res)
				{
					//var RDPage=Navigation.NavigationStack.GetEnumerator();

					var exPage = new ExchangePage(_reward, _place);
					await Navigation.PushModalAsync(exPage);

					Helpers.Settings.AllPlaces[Helpers.Settings.AllPlaces.IndexOf(_place)].Points += -1*_reward.ReqPoints;
					Helpers.Settings.UserPoints += -1 * _reward.ReqPoints;

					exPage = null;
					Helpers.Settings.IsReturn = false;

					IsBusy = false;
					await Navigation.PopAsync();
					//await Navigation.PushAsync(new ExchangePage(_reward, _place));
				}
				else
					await DisplayAlert("Puntos insuficientes", "Te faltan "
								 + (_reward.ReqPoints - Helpers.Settings.UserPoints).ToString(), "OK");
				}

			}

		/*public async void exchangeOK(bool isExchanged) {
			if (isExchanged)
			{
				//var RDPage=Navigation.NavigationStack.GetEnumerator();
				var exPage = new ExchangePage(_reward, _place);

				await Navigation.PushModalAsync(exPage);

				exPage = null;
				await Navigation.PopAsync();
				//await Navigation.PushAsync(new ExchangePage(_reward, _place));
			}
			else
				await DisplayAlert("Puntos insuficientes", "Te faltan "
							 + (_reward.ReqPoints - Settings.CurrentUser.Points).ToString(), "OK");
		}
		*/
	}
}
