using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Refractored.XamForms.PullToRefresh;
//using SlideOverKit;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using Plugin.Connectivity;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Syncfusion.SfNavigationDrawer.XForms;

namespace fidus
{
	public partial class MainPage : ContentPage  //, IMenuContainerPage
	{

        private MainViewModel mVM;
        private ZXingScannerPage scanPage;
        //private Image img;
		private ListView listview = new ListView()
		{
			VerticalOptions = LayoutOptions.FillAndExpand,
			SeparatorVisibility = SeparatorVisibility.None,
			BackgroundColor = Color.Transparent
		};
		private AzureClient<Person> _client;
		public ListView ListView { get { return listview; } }


		public MainPage()
        {

            InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";
			NavigationPage.SetBackButtonTitle(this, "Volver");

			if (Application.Current.Properties.ContainsKey("UserEmail"))
			{
				App.UpdateUSettings();
				Settings.IsLogin = false;
			}
			else
			{
				Settings.IsLogin = true;
				App.UpdateProperties();
			}

			mVM = new MainViewModel();

            BindingContext = mVM;
			//img = new Image();
			//img.Source = ImageSource.FromFile("userimg.png");

			//mVM.Mname = Settings.CurrentUser.Name;
			//mVM.Mimg = img;
			//mVM.Msize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			MenuDrawer();
			//SlideMenu = new MasterMenuPage();

			//You can add a ToolBar button to show the Menu.
			this.ToolbarItems.Add(new ToolbarItem
			{
				Command = new Command(() =>
			   {
				   DependencyService.Get<IToggleDrawer>().ToggleDrawer();
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

		private void MenuDrawer ()
		{

			var BkgImage = new Image()
			{
				Source = ImageSource.FromResource("fidus.giftly.png"),
				Aspect = Aspect.AspectFill
			};

			MenuStack.VerticalOptions = LayoutOptions.FillAndExpand;

			MenuStack.Children.Add(BkgImage);

			Image UserImg = new Image()
			{
				Source = "userimg.png",
				HeightRequest = 70,
				WidthRequest = 70,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 10, 0, 0)
			};

			Label UsrLab = new Label()
			{
				Text = Settings.CurrentUser.Name,
				FontFamily = Device.OnPlatform("Helvetica-bold", "Roboto-bold", ""),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			Label Usrmail = new Label()
			{
				Text = Settings.CurrentUser.Email,
				FontFamily = Device.OnPlatform("Helvetica-bold", "Roboto-bold", ""),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			Image linea = new Image()
			{
				Source = "menuline.png",
				HeightRequest = 20,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(10, 10, 10, 0)
			};

			Label menutext = new Label()
			{
				Text = "Menú Principal",
				TextColor = Color.FromHex(Settings.FidusColor),
				FontFamily = Device.OnPlatform("Helvetica", "Roboto", ""),
				FontSize = 14,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				FontAttributes = FontAttributes.Bold,
				Margin = new Thickness(10, 0, 5, 0)
			};
			var cell = new DataTemplate(typeof(Controls.CustomImageCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.TextColorProperty, "TextColor");
			cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");

			listview.ItemTemplate = cell;

			var masterPageItem = new List<MasterPageItem>();


			//masterPageItem.Add(new MasterPageItem
			//{
			//    Title = "Inicio",
			//    TextColor = Color.FromHex(Settings.FidusColor),
			//    IconSource = "geopin.png",
			//    TargetType = typeof(MainPage)
			//});
			masterPageItem.Add(new MasterPageItem
			{
				Title = "Historial de Canjes",
				TextColor = Color.Black,
				IconSource = "historial.png",
				TargetType = typeof(HistoryPage)
			});
			masterPageItem.Add(new MasterPageItem
			{
				Title = "Editar Perfil",
				TextColor = Color.Black,
				IconSource = "iconousuario.png",
				TargetType = typeof(EditUserDataPage)
			});
			masterPageItem.Add(new MasterPageItem
			{
				Title = "Logout",
				TextColor = Color.Black,
				IconSource = "logout.png"
			});
			listview.ItemsSource = masterPageItem;
			StackLayout MenuObjects = new StackLayout()
			{
				Orientation = StackOrientation.Vertical
			};

			MenuObjects.Children.Add(UserImg);
			MenuObjects.Children.Add(UsrLab);
			MenuObjects.Children.Add(Usrmail);
			MenuObjects.Children.Add(linea);
			MenuObjects.Children.Add(menutext);

			MenuObjects.Children.Add(listview);

			MenuStack.Children.Add(MenuObjects);

			//this.Content = MenuStack;

			listview.ItemSelected += OnItemSelected;
					
		}

		internal async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			//if (e.SelectedItem == null) return;
			var item = e.SelectedItem as MasterPageItem;

			if (item != null)
			{
				if (item.TargetType != null)
				{
					Page Detail = (Page)Activator.CreateInstance(item.TargetType);

					await Navigation.PushAsync(Detail);

					Debug.WriteLine("Menu : " + item.Title);
					//this.HideWithoutAnimations();
					ListView.SelectedItem = null;
				}
				else if (item.Title == "Logout")
				{
					try
					{
						//this.IsEnabled = false;
						//await App.instance.UpdateDB();
						Settings.CurrentUser.Logged = false;

						if (CrossConnectivity.Current.IsConnected)
						{
							_client = new AzureClient<Person>();
							IMobileServiceTable<Person> _tabla = _client.GetPTable();
							JObject data = new JObject {
								{ "id", Settings.CurrentUser.id },
								{ "Logged", false }
							};
							await _tabla.UpdateAsync(data);
						}
						Settings.CurrentUser.Name = "";
						Settings.CurrentUser.Email = "";
						Settings.CurrentUser.Points = 0;
						Settings.CurrentUser.id = "";
						Settings.AllPlaces.Clear();
						Settings.Hitem.Place = "";
						Settings.Hitem.id = "";
						App.CleanProperties();

						//var pclient = new LoadAsync<Place>();
						//pclient.PurgeTable();
						Settings.IsLogin = true;
						//_client.CloseDB();

						//this.HideWithoutAnimations();

						await Navigation.PushModalAsync(new loginPage(), false);

						//App.instance.ClearNavigationAndGoLogin();

					}
					catch (Exception ex)
					{
						Debug.WriteLine("MainPage: Exit stack " + ex);
					}

					Debug.WriteLine("Menu : Logout");
				}
				DependencyService.Get<IToggleDrawer>().ToggleDrawer();

				((ListView)sender).SelectedItem = null;

			}

		}
    }
}
