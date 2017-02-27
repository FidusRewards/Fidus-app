using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace fidus
{
	public class RewardsViewModel : BaseViewModel
	{
		private LoadAsync<Rewards> _clientR= new LoadAsync<Rewards>(MainViewModel._client);
		public Command RefreshCommand { get; set; }

		private ObservableCollection<Rewards> _items;
		private ObservableCollection<History> _history;
		public string Place;
		private string _points;

		public string PPoints { 
			get { return _points;}
			set { _points = value; OnPropertyChanged();}
		}

		public ObservableCollection<Rewards> Items
		{
			get { return _items; }
			set { _items = value; OnPropertyChanged(); }
		}

		public RewardsViewModel(String _place)
		{
			Place = _place;

			Items = new ObservableCollection<Rewards>();
			_history = new ObservableCollection<History>();

			RefreshCommand = new Command(Load);

		}

		public async void Load()
		{
			//Settings.CurrentUser.Points = 0;
			//History placeH = new History();
			//placeH.Person = Settings.CurrentUser.Email;
			//placeH.Place = Place;
			IsBusy = true;
			int _pointsL = Helpers.Settings.UserPoints;
			PPoints = _pointsL.ToString();
			Rewards _rew = new Rewards();
			_rew.Place = Place;

			//IsBusy = true;
			var dif = (DateTime.Now - Helpers.Settings.LastRewardsInit.ToLocalTime()).TotalMinutes;
			if (CrossConnectivity.Current.IsConnected)
			{ if (dif > 10)
				{
					await _clientR.SyncAsync();
					Helpers.Settings.LastRewardsInit = DateTime.Now.ToLocalTime();
				}
			}else
				MessagingCenter.Send(this, "NOINET");

			Items.Clear();
			Items = await _clientR.Load(_rew);

			//_history.Clear();
			//_history = await LoadHistory.Load(placeH);
			//IsBusy = false;

			if (Items !=null && _history != null)
			{
				Debug.WriteLine("RewardsVM: Load - AddedPoints " + Helpers.Settings.CurrentUser.Points);
				MessagingCenter.Send(this, "Loaded");
			}
			else {
				MessagingCenter.Send(this, "NotLoaded");
			}
			IsBusy = false;
		}
	}
}
