using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Refractored.XamForms.PullToRefresh;
using SlideOverKit;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Plugin.Connectivity;

namespace fidus
{
	public partial class MainPage : ContentPage, IMenuContainerPage
	{

        private MainViewModel mVM;
        private ZXingScannerPage scanPage;
        //private Image img;

        public MainPage()
        {

            InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";
			NavigationPage.SetBackButtonTitle(this, "Volver");

			mVM = new MainViewModel();

            BindingContext = mVM;
			//img = new Image();
			//img.Source = ImageSource.FromFile("userimg.png");
	
			//mVM.Mname = Settings.CurrentUser.Name;
            //mVM.Mimg = img;
            //mVM.Msize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

			SlideMenu = new MasterMenuPage();
			// You can add a ToolBar button to show the Menu.
			this.ToolbarItems.Add(new ToolbarItem
			{
				Command = new Command( () =>
				{
					if (SlideMenu.IsShown)
					{
						HideMenuAction?.Invoke();
					}
					else
					{
						ShowMenuAction?.Invoke();
					}
				}),
				Icon = "settings1.png",
				Text = "Settings",

				Priority = 0
			});

			var ckbut = new Button()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 60,
				Image = "checkinbutton.png",
				BackgroundColor = Color.Transparent,
				BorderColor = Color.Transparent,
				BorderWidth = 0
			};
			ckbut.SetBinding(Button.CommandProperty, new Binding("ScanButtonCommand", 0));
			CheckBut.Children.Add(ckbut);

			MessagingCenter.Subscribe<MainViewModel, ObservableCollection<Place>>(this, "Loaded",
															  (obj, mplaces) => IsBusy = false);
			MessagingCenter.Subscribe<MainViewModel>(this, "NotLoaded",
													 async (obj) =>
													 {
														 IsBusy = false;
														 await DisplayAlert("Error", "Problemas de conexión", "OK");
													});



            MessagingCenter.Subscribe<MainViewModel>(this, "ScanRequest", (obj) => Scan());

            //MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Rewards", async (obj, _place) =>
            //{
			//	Settings.IsReturn = true;
            //    Debug.WriteLine("MaingPage: OnTapp Mesg desde MainviewModel -> Rewards " + _place[0]);
            //    await Navigation.PushAsync(new RewardsPage(_place[0], _place[1]) { Title = "Recompensas" });
            //});

            //MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Rewards1", async (obj, _place) =>
            //{
			//	Settings.IsReturn = true;
            //    Debug.WriteLine("MainPage: Command Mesg desde MainviewModel -> Rewards1 " + _place[0]);
            //    await Navigation.PushAsync(new RewardsPage(_place[0], _place[1]) { Title = "Recompensas" });
            //});

            MessagingCenter.Subscribe<MainViewModel>(this, "Settings", async (obj) => {
                await Navigation.PushAsync(new HistoryPage());
            });
            MessagingCenter.Subscribe<MainViewModel>(this, "Exit", async (obj) =>
            {
                Debug.WriteLine("MainPage: Exit via MessagingCenter");
                try
                {
					await Navigation.PopToRootAsync();
                    //App.instance.ClearNavigationAndGoLogin();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MainPage: Exit stack " + ex);
                }

            });

			MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Thanks", async (obj, _place) =>
			{

				await Navigation.PushAsync(new QualifyPage(_place[0], _place[1], _place[2], _place[3], Settings.Hitem) { Title = "Califica" });
			});



		}


		protected override async void OnAppearing()
		{
			base.OnAppearing();
			bool Reach = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");

			if (!Reach || !CrossConnectivity.Current.IsConnected)
			{
				Settings.Default.IsInternetEnabled = false;
				await DisplayAlert("Advertencia", "No hay conexión a Internet. Algunas funciones pueden no estar habilitadas", "OK");
				//DependencyService.Get<ICloseApplication>().closeApp();
			};

			if (Application.Current.Properties.ContainsKey("ULogged"))
			{
				if (!(bool)Application.Current.Properties["ULogged"])
					await Navigation.PushModalAsync(new loginPage(), false);
			}
			mVM.Load();


		}


        private async void Scan()
        {
            int points;
            scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                char[] delimiterChars = { ',', '\t' };

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    string[] words = result.Text.Split(delimiterChars);
                    string urllogo;

                    if (words.Length > 5)
                    {
                        words[1] = words[1].Replace("\n", "");
                        words[2] = words[2].Replace("\n", "");
                        words[3] = words[3].Replace("\n", "");
                        words[4] = words[4].Replace("\n", "");
						words[5] = words[5].Replace("\n", "");

                        bool convert = Int32.TryParse(words[1], out points);

                        urllogo = Settings.ImgSrvProd + words[2];

                        string place = words[0].ToString();
                        string exchangecode = words[3].ToString();
                        string branch = words[4];
						//string category = words[5];

                        if (convert && urllogo.Length >= 47 && urllogo.Contains("fidusimgsrv") && exchangecode != null && branch != null)
                        {
                            bool result2 = await mVM.ConfirmQRCode(place, branch, exchangecode);
                            if (result2)
                            {
                                //await DisplayAlert("Gracias por venir a " + words[0], "Ganaste : " + words[1] + " \n Puntos provenientes de: " + branch + " \n Codigo de Confirmación: " + exchangecode, "OK");
                                mVM.UpdatePoints(words);
                            }
                            else
                            {
                                await DisplayAlert("El Código leído es Inválido", "Reintentá", "OK");
                            }

                        }
                        else
                            await DisplayAlert("El Código leído es Inválido", "Reintentá", "OK");
                    }
                    else
                        await DisplayAlert("El Código leído es Inválido", "Reintentá", "OK");
                });
            };

            await Navigation.PushAsync(scanPage);
            //Device.OpenUri(new Uri(url));



        }



		async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			Place selected = (e.SelectedItem as Place);
			Settings.IsReturn = true;
			await Navigation.PushAsync(new RewardsPage(selected));
			((ListView)sender).SelectedItem = null;	
		}

		public Action HideMenuAction
		{
			get;
			set;
		}

		public Action ShowMenuAction
		{
			get;
			set;
		}

		SlideMenuView slideMenu;
		public SlideMenuView SlideMenu
		{
			get
			{
				return slideMenu;
			}
			set
			{
				if (slideMenu != null)
					slideMenu.Parent = null;
				slideMenu = value;
				if (slideMenu != null)
					slideMenu.Parent = this;
			}
		}

    }
}
