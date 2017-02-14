using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace fidus
{
	public partial class HistoryPage : ContentPage
	{
		private HistoryViewModel hVM;

		public HistoryPage()
		{

			InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Volver";

			hVM = new HistoryViewModel();

			BindingContext = hVM;

			MessagingCenter.Subscribe<HistoryViewModel, History>(this, "Loaded", (obj, _history) 
			                                                     => { });

			MessagingCenter.Subscribe<HistoryViewModel>(this, "Loaded", async (obj) => {
				await DisplayAlert("Error", "Problemas de Conexión", "OK");
			});

			hVM.Load();

		}


	}
}
