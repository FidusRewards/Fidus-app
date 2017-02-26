using System;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace fidus
{
	public class RewardsDetailsViewModel: BaseViewModel
	{
		private string _photo, _name, _description;
		private double _reqpoints;
		public Rewards _reward { get; set;}
		//public Command debitCommand { get; set; }

		public string Photo
		{
			get { return _photo;} 
			set { _photo = value; OnPropertyChanged();}
		}

		public string Name
		{
			get { return _name;}
			set { _name = value; OnPropertyChanged(); }
		}

		public string Description { 
			get { return _description;}
			set { _description = value; OnPropertyChanged(); }
		}

		public double ReqPoints { 
			get { return _reqpoints;}
			set { _reqpoints = value; OnPropertyChanged();}
		}

		public RewardsDetailsViewModel(Rewards selectedReward)
		{

			_reward = selectedReward;
			Photo = _reward.Photo;
			Name = _reward.Name;
			Description = _reward.Description;
			ReqPoints = _reward.ReqPoints;
			//debitCommand = new Command(exchange);
		}

		public async Task<bool> OnExClicked() {
			IsBusy = true;
			int _points = Helpers.Settings.UserPoints;
			if (_reward.ReqPoints <= _points)
			{


				var _items = new LoadAsync<History>(MainViewModel._client);
				History _history = new History();

				_history.DateTime = DateTime.Now.ToLocalTime();
				_history.EarnPoints = -1*Convert.ToInt32(_reward.ReqPoints);
				_history.Place = _reward.Place;
				_history.IsDebit = true;
				_history.Person = Helpers.Settings.CurrentUser.Email;
				_history.Reward = _reward.Name;

				await _items.Save(_history);

				//if (CrossConnectivity.Current.IsConnected)
				//	await _items.InitSync("history"+Helpers.Settings.UserEmail);
				//MessagingCenter.Send(this, "Debited", true);
				return true;
				//await Navigation.PushAsync(new ExchangePage(_reward, _place));
			}
			else
				//MessagingCenter.Send(this, "Debited", false);
				return false;
		}
	
	}
}
