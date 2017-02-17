using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace fidus
{
	public class RewardsViewModel : BaseViewModel
	{
		private LoadAsync<Rewards> LoadCollection;
		private LoadAsync<History> LoadHistory;
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

			LoadCollection = new LoadAsync<Rewards>();
			Items = new ObservableCollection<Rewards>();
			LoadHistory = new LoadAsync<History>();
			_history = new ObservableCollection<History>();

			RefreshCommand = new Command(Load);

		}

		public async void Load()
		{
			//Settings.CurrentUser.Points = 0;
			//History placeH = new History();
			//placeH.Person = Settings.CurrentUser.Email;
			//placeH.Place = Place;
			PPoints = Settings.CurrentUser.Points.ToString();
			Rewards _rew = new Rewards();
			_rew.Place = Place;

			//IsBusy = true;
			Items.Clear();
			Items = await LoadCollection.Load(_rew);

			//_history.Clear();
			//_history = await LoadHistory.Load(placeH);
			//IsBusy = false;

			if (Items !=null && _history != null)
			{
				Debug.WriteLine("RewardsVM: Load - AddedPoints " + Settings.CurrentUser.Points);
				MessagingCenter.Send(this, "Loaded");
			}
			else {
				MessagingCenter.Send(this, "NotLoaded");
			}
		}
	}
}
