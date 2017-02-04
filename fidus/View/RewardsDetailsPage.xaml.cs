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
			InitializeComponent();


			BCanje.BackgroundColor = Color.FromHex(Settings.FidusColor);
			BCanje.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			BCanje.Margin = new Thickness(5, 15, 5, 0);
			//BCanje.SetBinding(Button.CommandProperty, new Binding("debitCommand", 0));
			_place = place;
			_reward = selectedReward;

			dVM = new RewardsDetailsViewModel(_reward);

			BindingContext = dVM;

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

					exPage = null;
					await Navigation.PopAsync();
					//await Navigation.PushAsync(new ExchangePage(_reward, _place));
				}
				else
					await DisplayAlert("Puntos insuficientes", "Te faltan "
								 + (_reward.ReqPoints - Settings.CurrentUser.Points).ToString(), "OK");
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
