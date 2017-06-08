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
using System.Threading.Tasks;

namespace fidus
{
	public partial class MainPage : ContentPage  //, IMenuContainerPage
	{

        private MainViewModel mVM;
        private ZXingScannerPage scanPage;
		private Button ckbut;
		//private Image img;

		private ListView listview = new ListView()
		{
			VerticalOptions = LayoutOptions.FillAndExpand,
			SeparatorVisibility = SeparatorVisibility.None,
			BackgroundColor = Color.Transparent
		};
		public ListView ListView { get { return listview; } }
		public static ContentPage instance;



		public MainPage()
		{
			instance = this;
			InitializeComponent();
			Debug.WriteLine("MainPage Settings : " + Helpers.Settings.UserEmail);
			Helpers.Settings.IsBoot = true;
			Helpers.Settings.IsLogin = false;
			Helpers.Settings.IsReturn = false;

			if (Helpers.Settings.UserEmail == "fidus@com" || Helpers.Settings.UserEmail == null)
			{
				Helpers.Settings.IsLogin = true;
			}

			mVM = new MainViewModel();

			BindingContext = mVM;

			MenuDrawer();

			DrawMain();

			MessagingCenter.Subscribe<MainViewModel>(this, "NotLoaded",async (obj) =>{
				mVM.IsBusy = false;
				await DisplayAlert("Error", "Problemas cargando los Datos. Cerrá por favor la App y volvé a intentar", "OK");
				});

			MessagingCenter.Subscribe<MainViewModel>(this, "ScanRequest", (obj) => Scan());

			MessagingCenter.Subscribe<MainViewModel>(this, "Settings", async (obj) =>{
				await Navigation.PushAsync(new HistoryPage());
				});
			MessagingCenter.Subscribe<MainViewModel>(this, "Exit", async (obj) =>{
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

			MessagingCenter.Subscribe<MainViewModel, string[]>(this, "Thanks", async (obj, _place) =>{
				var qPage = new QualifyPage(_place[0], _place[1], _place[2], _place[3], Helpers.Settings.Hitem) { Title = "Califica" };
				NavigationPage.SetHasBackButton(qPage, false);
				await Navigation.PushAsync(qPage);
			});

			MessagingCenter.Subscribe<MainViewModel>(this, "LOGOUT", async (obj) => {
				await Navigation.PushModalAsync(new loginPage(), false);                                        
			});

			MessagingCenter.Subscribe<loginViewModel>(this, "LOGGEDIN", (obj) => {
				ReDraw();
			});

			MessagingCenter.Subscribe<MainViewModel>(this, "NOINET", async (obj) => { 
				//Helpers.Settings.IsInternetEnabled = false;
				await DisplayAlert("Advertencia", "No hay conexión a Internet. Algunas funciones pueden no estar habilitadas", "OK");
			});
		}


		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (Helpers.Settings.IsLogin)
			{
				await Navigation.PushModalAsync(new loginPage(), false);
				Helpers.Settings.IsLogin = false;

			}

			Debug.WriteLine("MainPage Settings after update : " + Helpers.Settings.UserEmail);

			//bool Reach = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");

			if (!CrossConnectivity.Current.IsConnected) //!Reach || 
			{
				//Helpers.Settings.IsInternetEnabled = false;
				await DisplayAlert("Advertencia", "No hay conexión a Internet. Algunas funciones pueden no estar habilitadas", "OK");
				//DependencyService.Get<ICloseApplication>().closeApp();
			}

			if (Helpers.Settings.UserEmail!="fidus@com")
				mVM.Load();


		}

		public void ReDraw() {
			Helpers.Settings.IsReturn = false;
			mVM.Load();
		}

		private void DrawMain()
		{ 
			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";
			NavigationPage.SetBackButtonTitle(this, "Volver");

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

			ckbut = new Button()
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

		}


        private async void Scan()
        {
            int points;
			//string[] words;							//DECOMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
			scanPage = new ZXingScannerPage();			//COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
            bool scanFinished = false;
            scanPage.OnScanResult += (result) =>        //COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
            {                                           //COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
                scanPage.IsScanning = false;                //COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
                char[] delimiterChars = { ',', '\t' };		//COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE

                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (!scanFinished)
                    {
                        scanFinished = true;
                        await Navigation.PopAsync();        //COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
                        mVM.IsBusy = true;
                        string[] words = result.Text.Split(delimiterChars);  //COMENTAR ESTA LINEA PARA PRUEBA DEL SCANCODE
                        #region Demo QR    //DESCOMENTAR ESTA REGION PARA PRUEBA DEL SCANCODE
                        //words = new string[6];
                        //words[0] = "Starbucks";
                        //words[1] = "20";
                        //words[2] = "Starbucks_logo.png";
                        //words[3] = "S-00A";
                        //words[4] = "Malabia1";
                        //words[5] = "DEMOTEST";
                        #endregion
                        string urllogo;

                        if (words.Length > 5)
                        {
                            words[1] = words[1].Replace("\n", "");
                            words[2] = words[2].Replace("\n", "");
                            words[3] = words[3].Replace("\n", "");
                            words[4] = words[4].Replace("\n", "");
                            words[5] = words[5].Replace("\n", "");

                            bool convert = Int32.TryParse(words[1], out points);

                            urllogo = Helpers.Settings.ImgSrvProd + words[2];

                            string place = words[0];
                            string exchangecode = words[3];
                            string branch = words[4];
                            string category = words[5];

                            if (convert && urllogo.Length >= 47 && urllogo.Contains("fidusimgsrv") && exchangecode != null && branch != null && category != null)
                            {
                                Debug.WriteLine("Datos enviados \n Lugar " + words[0] + " \n Puntos : " + words[1] + " \n Sucursal : " + branch + " \n Codigo de Confirmación: " + exchangecode + " \n Categoria: " + category);
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
                        }
                    });
            
				mVM.IsBusy = false;
			};			//COMENTAR el }; PARA PRUEBA DE SCAN CODE  

            await Navigation.PushAsync(scanPage);  // COMENTAR ESTA LINEA PARA PRUEBA DEL SCAN CODE
            
			//Ejemplo para abir una URL desde el QR     
			//Device.OpenUri(new Uri(url));

        }



		async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			Place selected = (e.SelectedItem as Place);
			Helpers.Settings.IsReturn = false;
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
				FontFamily = Device.OnPlatform("Helvetica-bold", "Roboto-bold", ""),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			UsrLab.SetBinding(Label.TextProperty, "CurrUser");

			Label Usrmail = new Label()
			{
				FontFamily = Device.OnPlatform("Helvetica-bold", "Roboto-bold", ""),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			Usrmail.SetBinding(Label.TextProperty, "CurrUmail");

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
				TextColor = Color.FromHex(Helpers.Settings.FidusColor),
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

			MenuGrid.VerticalOptions = LayoutOptions.FillAndExpand;

			MenuGrid.Children.Add(BkgImage);
			MenuGrid.Children.Add(MenuObjects);

			//this.Content = MenuStack;

			listview.ItemSelected += MenuItemSelected;
					
		}

		internal async void MenuItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			var item = e.SelectedItem as MasterPageItem;

			if (item.TargetType != null)
			{
				Helpers.Settings.IsReturn = true;
				Page Detail = (Page)Activator.CreateInstance(item.TargetType);

				await Navigation.PushAsync(Detail);

				Debug.WriteLine("Menu : " + item.Title);
				//this.HideWithoutAnimations();
				//listview.SelectedItem = null;
			}
			else if (item.Title == "Logout")
			{
				mVM.DoLogout();

			}	
			DependencyService.Get<IToggleDrawer>().ToggleDrawer();

			((ListView)sender).SelectedItem = null;

		}

    }
}
