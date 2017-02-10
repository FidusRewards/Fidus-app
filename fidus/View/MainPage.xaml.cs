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
        private Image image;
        //private Image img;

        public MainPage()
        {

            InitializeComponent();

			//NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";

            mVM = new MainViewModel();

            BindingContext = mVM;
			//img = new Image();
			//img.Source = ImageSource.FromFile("userimg.png");
	
			mVM.Mname = Settings.CurrentUser.Name;
            //mVM.Mimg = img;
            mVM.Msize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

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


            MessagingCenter.Subscribe<MainViewModel, ObservableCollection<Place>>(this, "Loaded",
                                                      (obj, mplaces) => GridPlaces(mplaces));
            MessagingCenter.Subscribe<MainViewModel>(this, "NotLoaded",
                                                     async (obj) => await DisplayAlert("Error", "Problemas de conexión", "OK"));



            MessagingCenter.Subscribe<MainViewModel>(this, "ScanRequest", (obj) => Scan());

            MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Rewards", async (obj, _place) =>
            {
                Debug.WriteLine("MaingPage: OnTapp Mesg desde MainviewModel -> Rewards " + _place[0]);
                await Navigation.PushAsync(new RewardsPage(_place[0], _place[1]) { Title = "Recompensas" });
            });

            MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Rewards1", async (obj, _place) =>
            {
                Debug.WriteLine("MainPage: Command Mesg desde MainviewModel -> Rewards1 " + _place[0]);
                await Navigation.PushAsync(new RewardsPage(_place[0], _place[1]) { Title = "Recompensas" });
            });

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
					await Navigation.PushModalAsync(new loginPage());
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

                    if (words.Length > 4)
                    {
                        words[1] = words[1].Replace("\n", "");
                        words[2] = words[2].Replace("\n", "");
                        words[3] = words[3].Replace("\n", "");
                        words[4] = words[4].Replace("\n", "");

                        bool convert = Int32.TryParse(words[1], out points);

                        urllogo = Settings.ImgSrvProd + words[2];

                        string place = words[0].ToString();
                        string exchangecode = words[3].ToString();
                        string branch = words[4];


                        if (convert && urllogo.Length >= 47 && urllogo.Contains("fidusimgsrv") && exchangecode != null && branch != null)
                        {
                            bool result2 = await mVM.ConfirmQRCode(place, branch, exchangecode);
                            if (result2)
                            {
                                await DisplayAlert("Gracias por venir a " + words[0], "Ganaste : " + words[1] + " \n Puntos provenientes de: " + branch + " \n Codigo de Confirmación: " + exchangecode, "OK");
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

        //public async void Handle_Clicked(object sender, System.EventArgs e)
        //{
        //	await Navigation.PushAsync(new HistoryPage());
        //}
        

        private void GridPlaces(ObservableCollection<Place> _places)
        {

            PlaceLayout.Children.Clear();

            int _row = -1, _column = 0, i = 0;
            Grid _grid = new Grid();
            _grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            _grid.VerticalOptions = LayoutOptions.Start;
            _grid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _grid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            _grid.RowDefinitions.Add(
                new RowDefinition() { Height = 150 });
            _grid.RowDefinitions.Add(
                new RowDefinition() { Height = 150 });
            _grid.RowDefinitions.Add(
                new RowDefinition() { Height = 150 });
            _grid.RowDefinitions.Add(
                new RowDefinition() { Height = 150 });

            _grid.BackgroundColor = Color.White;

            foreach (Place item in _places)
            {
                if (i % 2 != 0)
                {
                    _column = 1;
                }
                else
                {
                    _column = 0;
                    _row++;

                }

                image = new Image
                {
                    Margin = new Thickness(5),
                    Source = item.Logo,

                };
                var OnTap = new TapGestureRecognizer();
                OnTap.CommandParameter = item;
                OnTap.SetBinding(TapGestureRecognizer.CommandProperty, "TapCommand");

                image.GestureRecognizers.Add(OnTap);

                _grid.Children.Add(image, _column, _row);

                i++;
            }

            var scrollview = new ScrollView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                Content = _grid


            };

            var refreshView = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = scrollview,
                RefreshColor = Color.FromHex("#3498db"),
            };

            //Set Bindings
            refreshView.SetBinding<MainViewModel>(PullToRefreshLayout.IsRefreshingProperty, vm => vm.IsBusy, BindingMode.TwoWay);
            //refreshView.SetBinding<MainViewModel>(PullToRefreshLayout.RefreshCommandProperty, vm => vm.RefreshCommand);

            var stackLayout1 = new StackLayout
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Horizontal,

            };
            var ScrollTitle = new Label
            {
                Text = "Comercios Adheridos",
                TextColor = Color.FromHex(Settings.FidusColor),
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            var TitleImg = new Image
            {
                Source = ImageSource.FromResource("fidus.merchantB.png"),
                WidthRequest = 25,
                HeightRequest = 25
            };

            stackLayout1.Children.Add(ScrollTitle);
            stackLayout1.Children.Add(TitleImg);

            PlaceLayout.Children.Add(stackLayout1);
            PlaceLayout.Children.Add(scrollview);//refreshView);

            mVM.IsBusy = false;
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
