using Xamarin.Forms;

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
				FontFamily=Device.OnPlatform(Helpers.Settings.FidusIosFont,Helpers.Settings.FidusAndFont,""),
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
				FontFamily=Device.OnPlatform(Helpers.Settings.FidusIosFont,Helpers.Settings.FidusAndFont,""),
				BackgroundColor = Color.Transparent,
				TextColor = Color.Gray,
				HorizontalOptions=LayoutOptions.FillAndExpand

			};
			Button Ingresar = new Button
			{
				Text = "Ingresá",
				FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, ""),
				FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromHex(Helpers.Settings.FidusColor),
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = new Thickness(0, 20, 0, 0)
            };
			Button Registro = new Button
			{
				Text = "Registrate",
				TextColor = Color.White,
				FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, ""),
				FontAttributes = FontAttributes.Bold,
				BackgroundColor = Color.FromHex(Helpers.Settings.FidusBlue),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				Margin = new Thickness(0, 15, 0, 0)
            };

			var activityIndicator = new ActivityIndicator
			{
				Color = Color.FromHex(Helpers.Settings.FidusColor),
			};
			activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			var Label = new Label
			{
				Text = "Ingresá tu email y contraseña",
				Margin = new Thickness(2,15,2,2),
				HorizontalTextAlignment = TextAlignment.Center,
				FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, ""),
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
					new Label {Text=Helpers.Settings.AppVersion,
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

						Helpers.Settings.IsReturn = false;
						//Helpers.Settings.IsLogin = false;
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

			MessagingCenter.Subscribe<RegisterViewModel>(this, "VOLVERMAIN", async (obj) => {
				Helpers.Settings.IsReturn = false;
				Helpers.Settings.IsLogin = false;

				var Mcount=Navigation.ModalStack.Count;
				if (Mcount == 2)
				{
					await Navigation.PopModalAsync(false);
					await Navigation.PopModalAsync(false);
					MessagingCenter.Send(this, "LOGGEDIN");


				}else{
					await DisplayAlert("Error","En el PopModal: "+Mcount,"OK");
				}
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
