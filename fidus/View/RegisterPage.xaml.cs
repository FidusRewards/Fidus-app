using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fidus
{
	public partial class RegisterPage : ContentPage
	{
        private RegisterViewModel rVM;

        public RegisterPage(string email, string pass)
        {
            InitializeComponent();

            rVM = new RegisterViewModel();

            BindingContext = rVM;

            //DisplayAlert(email, pass, "OK");
            rVM.REmail = email;
            rVM.RPass = pass;
            rVM.FSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

            MessagingCenter.Subscribe<RegisterViewModel>(this, "Registered", async (obj) =>
            {
                await DisplayAlert("Bienvenido", "Te registraste en forma exitosa", "OK");
				//var content = new MainPage();

				//var _mainPage = new NavigationPage(content)
				//{
				//    BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
				//    BarTextColor = Color.White,
				//    Title = "Fidus"
				//};

				//NavigationPage.SetHasNavigationBar(_mainPage, false);
				//NavigationPage.SetHasBackButton(_mainPage, false);
				//await Navigation.PushAsync(_mainPage);
				//await Navigation.PopModalAsync(false);

            });

            MessagingCenter.Subscribe<RegisterViewModel>(this, "NotValid", async (obj) =>
            {
                await DisplayAlert("Error", "Debes completar todos los campos", "OK");

            });

            MessagingCenter.Subscribe<RegisterViewModel>(this, "PassNotSame", async (obj) =>
            {
                await DisplayAlert("Error", "Las contraseñas ingresadas no coinciden", "OK");

            });

        }
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

    }
}
