using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;
using System.Diagnostics;

namespace fidus
{
	public partial class loginPage : ContentPage
	{
		private loginViewModel loginVM;

		public loginPage()
		{
			InitializeComponent();
            this.Title = "Ingreso";
			loginVM = new loginViewModel();

			BindingContext = loginVM;

			Image image = new Image { HeightRequest = 100, WidthRequest = 100  };//,Aspec = Aspect.AspectFit };

			//Items = new ObservableCollection<Person>();


			image.Source = ImageSource.FromResource("fidus.logofidus.png");
			var Email = new Entry
			{
				Placeholder = "Email",
				BackgroundColor = Color.Transparent,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Gray,
				Keyboard = Keyboard.Email
			};
			var Pass = new Entry
			{
				Placeholder = "Password",
				IsPassword = true,
				BackgroundColor = Color.Transparent,
				TextColor = Color.Gray
			};
            Button Ingresar = new Button
            {
                Text = "Ingresá",
                BackgroundColor = Color.FromHex(Settings.FidusColor),
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = new Thickness(0, 15, 0, 0)
            };
			Button Registro = new Button
			{
				Text = "Registrate",
				TextColor = Color.White,
				BackgroundColor = Color.Gray,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				Margin = new Thickness(0, 15, 0, 0)
            };

			var activityIndicator = new ActivityIndicator
			{
				Color = Color.FromHex(Settings.FidusColor),
			};
			activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");


			Content = new StackLayout
			{
				Padding = new Thickness(20),

				Children = {
					image,
					new Label {Text="Ingresá tu email y pass", Margin=new Thickness(10),
						FontSize= Device.GetNamedSize(NamedSize.Large, typeof(Label))},
					Email,
					Pass,
					activityIndicator,
					new StackLayout {
						HorizontalOptions=LayoutOptions.FillAndExpand,
						Orientation = StackOrientation.Vertical,
						VerticalOptions=LayoutOptions.Start,
						Padding = new Thickness(5),
						Children = {Ingresar, Registro}
					},
					new Label {Text=Settings.AppVersion,
						HorizontalOptions=LayoutOptions.StartAndExpand,
						TextColor=Color.Gray
					}
				}
				};


			Ingresar.Clicked += async (sender, e) =>
			{
				loginVM.IsBusy = true;
                Ingresar.IsVisible = false;
				if (Email.Text != null && Pass.Text != null)
				{
					bool result = await loginVM.LoginQuery(Email.Text, Pass.Text);

					if (result)
					{
						//await DisplayAlert("Bienvenido", Email.Text, "OK");
						loginVM.IsBusy = false;

						//Application.Current.MainPage = new HistoryPage();
						var content = new UserMenuPage();

                        //var _mainPage = new NavigationPage(content);
                        //{
                        //	BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
                        //	BarTextColor = Color.White,
                        //	Title = "Fidus"
                        //};

						//NavigationPage.SetHasBackButton(content, false);
                        //NavigationPage.SetHasNavigationBar(_mainPage, false);

                        await Navigation.PushModalAsync(content);

					}
					else
					{
						await DisplayAlert("Datos Incorrectos", "Si no sos usuario registrate. " +
										   "Si no recordas tu contraseña, escribinos a fidusrewards@gmail.com", "OK");
						loginVM.IsBusy = false;
                        Ingresar.IsVisible = true;

                    }
				}
				else {
					await DisplayAlert("Error", "El usuario y pass no pueden estar vacíos", "OK");
					loginVM.IsBusy = false;
                    Ingresar.IsVisible = true;
                }
			};

			Registro.Clicked += async (sender, e) =>
			{
				var regpage = new RegisterPage(Email.Text, Pass.Text);
				//NavigationPage.SetHasNavigationBar(regpage, false);
				//NavigationPage.SetHasBackButton(regpage, false);

				await Navigation.PushAsync(regpage);
			};

			MessagingCenter.Subscribe<loginViewModel>(this, "NOINET", async (obj) => {
				await DisplayAlert("Error", "No hay Conexión", "OK");
			});
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			bool Reach = await CrossConnectivity.Current.IsRemoteReachable("www.google.com");

			if (!Reach || !CrossConnectivity.Current.IsConnected)
			{
				Settings.Default.IsInternetEnabled = false;
				await DisplayAlert("Error", "No hay conexión a Internet. La App se cerrará", "OK");
				DependencyService.Get<ICloseApplication>().closeApp();
			};
				
		}


	}
}
