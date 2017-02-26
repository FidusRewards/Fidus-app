using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace fidus
{
	public class HistoryViewModel: BaseViewModel
	{
		private LoadAsync<History> LoadCollection;
		public Command RefreshCommand { get; set; }
		private ObservableCollection<History> _items;

		public ObservableCollection<History> Items
		{
			get { return _items; }
			set { _items = value; OnPropertyChanged(); }
		}
		public HistoryViewModel()
		{
			LoadCollection = new LoadAsync<History>(MainViewModel._client);
			Items = new ObservableCollection<History>();
			RefreshCommand = new Command(Load);

		}

		public async void Load()
		{
			IsBusy = true;
			Items.Clear();
			Items = await LoadCollection.Load(Helpers.Settings.CurrentUser.Email);

			IsBusy = false;
			if (Items != null)

				MessagingCenter.Send(this, "Loaded", Items);

			else
				MessagingCenter.Send(this, "NotLoaded");
		}
	}
}
