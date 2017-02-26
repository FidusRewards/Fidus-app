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
			IndicatorRegister.Color = Color.FromHex(Helpers.Settings.FidusColor);
            //DisplayAlert(email, pass, "OK");
            rVM.REmail = email;
            rVM.RPass = pass;
            rVM.FSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

            MessagingCenter.Subscribe<RegisterViewModel>(this, "Registered", async (obj) => {
                await DisplayAlert("Bienvenido", "Te registraste en forma exitosa", "OK");
            });

            MessagingCenter.Subscribe<RegisterViewModel>(this, "NotValid", async (obj) =>{
                await DisplayAlert("Error", "Debes completar todos los campos", "OK");
            });

            MessagingCenter.Subscribe<RegisterViewModel>(this, "PassNotSame", async (obj) => {
                await DisplayAlert("Error", "Las contraseñas ingresadas no coinciden", "OK");
            });
        }

		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

    }
}
