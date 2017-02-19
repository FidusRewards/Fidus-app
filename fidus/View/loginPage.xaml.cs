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

			//NavigationPage.SetTitleIcon(this, "fidus_text.png");
			//this.Title = "Fidus";
			Settings.IsLogin = true;

			loginVM = new loginViewModel();

			BindingContext = loginVM;

			Image image = new Image { HeightRequest = 134, WidthRequest = 100  };//,Aspec = Aspect.AspectFit };

			//Items = new ObservableCollection<Person>();


			image.Source = ImageSource.FromResource("fidus.Fiduslogored.png");
			image.Margin = new Thickness(0, Device.OnPlatform(60, 40, 0), 0, 0);

			var MIcon = new Image
			{
				HeightRequest = 15,
				WidthRequest = 15,
				Margin = new Thickness(0, 10, 0, 0),
				Source = ImageSource.FromResource("fidus.icono-email.png") };

			var Email = new Entry
			{
				Placeholder = "Email",
				BackgroundColor = Color.Transparent,
				FontFamily=Device.OnPlatform(Settings.FidusIosFont,Settings.FidusAndFont,""),
				TextColor = Color.Gray,
				Keyboard = Keyboard.Email,
				Margin = new Thickness(0, 10, 0, 0),
				HorizontalOptions=LayoutOptions.FillAndExpand

			};

			var PIcon = new Image { 
				HeightRequest = 15,
				WidthRequest = 15,
				Source = ImageSource.FromResource("fidus.icono-pass.png")};
			var Pass = new Entry
			{
				Placeholder = "Contraseña",
				IsPassword = true,
				FontFamily=Device.OnPlatform(Settings.FidusIosFont,Settings.FidusAndFont,""),
				BackgroundColor = Color.Transparent,
				TextColor = Color.Gray,
				HorizontalOptions=LayoutOptions.FillAndExpand

			};
			Button Ingresar = new Button
			{
				Text = "Ingresá",
				FontFamily = Device.OnPlatform(Settings.FidusIosFont, Settings.FidusAndFont, ""),
				FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromHex(Settings.FidusColor),
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = new Thickness(0, 20, 0, 0)
            };
			Button Registro = new Button
			{
				Text = "Registrate",
				TextColor = Color.White,
				FontFamily = Device.OnPlatform(Settings.FidusIosFont, Settings.FidusAndFont, ""),
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.FromHex(Settings.FidusBlue),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				Margin = new Thickness(0, 15, 0, 0)
            };

			var activityIndicator = new ActivityIndicator
			{
				Color = Color.FromHex(Settings.FidusColor),
			};
			activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			var Label = new Label
			{
				Text = "Ingresá tu email y contraseña",
				Margin = new Thickness(2,15,2,2),
				HorizontalTextAlignment = TextAlignment.Center,
				FontFamily = Device.OnPlatform(Settings.FidusIosFont, Settings.FidusAndFont, ""),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				FontAttributes=FontAttributes.Bold
			};

			var BkgImage = new Image()
			{
				Source = ImageSource.FromResource("fidus.giftly.png"),
				Aspect = Aspect.AspectFill
			};

			RelativeLayout layout = new RelativeLayout();

			layout.Children.Add(BkgImage,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => { return parent.Width; }),
				Constraint.RelativeToParent((parent) => { return parent.Height; }));


			var Staklay = new StackLayout
			{
				Padding = new Thickness(35,10,35,5),

				Children = {
					image,
					Label,
					new StackLayout{
						Orientation=StackOrientation.Horizontal,
						HorizontalOptions=LayoutOptions.FillAndExpand,
						VerticalOptions=LayoutOptions.Center,
						Padding = new Thickness(5),
						Children={MIcon,Email}},
					new StackLayout{
						Orientation=StackOrientation.Horizontal,
						VerticalOptions=LayoutOptions.Center,
						HorizontalOptions=LayoutOptions.FillAndExpand,
						Padding = new Thickness(5),
						Children={PIcon,Pass}},
						activityIndicator,
					new StackLayout {
						HorizontalOptions=LayoutOptions.FillAndExpand,
						Orientation = StackOrientation.Vertical,
						VerticalOptions=LayoutOptions.Start,
						Padding = new Thickness(5),
						Children = {Ingresar, Registro}
					},
					new Label {Text=Settings.AppVersion,
						HorizontalOptions=LayoutOptions.EndAndExpand,
						VerticalOptions=LayoutOptions.End,
						TextColor=Color.Gray
					}
				}
				};
			layout.Children.Add(Staklay,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => { return parent.Width; }),
				Constraint.RelativeToParent((parent) => { return parent.Height; }));
			Content = layout;


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

						//Application.Current.MainPage = new HistoryPage();
						//var content = new MainPage();


						//Application.Current.MainPage 
						// var content = new NavigationPage(new MainPage())
						//{
						//	BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
						//	BarTextColor = Color.White,
						//Title = "Fidus"
						//};

						//NavigationPage.SetHasBackButton(content, false);
						//NavigationPage.SetTitleIcon(content, "fidus_text.png");

						//await Navigation.PushAsync(content);

						//NavigationPage.SetHasNavigationBar(_mainPage, false);
						Settings.IsReturn = false;
						await Navigation.PopModalAsync();


					}
					else
					{
						await DisplayAlert("Datos Incorrectos", "Si no sos usuario registrate. " +
										   "Si no recordas tu contraseña, escribinos a fidusrewards@gmail.com", "OK");
						
                    }
					loginVM.IsBusy = false;
					Ingresar.IsVisible = true;
				}
				else {
					await DisplayAlert("Error", "El usuario y la contraseña no pueden estar vacíos", "OK");
					loginVM.IsBusy = false;
                    Ingresar.IsVisible = true;
                }


			};

			Registro.Clicked += async (sender, e) =>
			{
				var regpage = new RegisterPage(Email.Text, Pass.Text);
				NavigationPage.SetHasNavigationBar(regpage, true);
				NavigationPage.SetHasBackButton(regpage, true);
				//await Navigation.PopModalAsync();
				await Navigation.PushModalAsync(regpage);

			};

			MessagingCenter.Subscribe<loginViewModel>(this, "NOINET", async (obj) => {
				await DisplayAlert("Advertencia", "No hay conexión a internet, algunas funciones pueden no estar disponibles", "OK");
			});

			MessagingCenter.Subscribe<RegisterPage>(this, "PoptoRoot", async (obj) => {
				await Navigation.PopModalAsync(false);
			});
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

		}

		protected override bool OnBackButtonPressed()
		{

			return true;
		}

	}
}
