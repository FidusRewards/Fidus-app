using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace fidus
{
	public partial class EditUserDataPage : ContentPage
	{
		private AzureClient<Person> _client;
		private EditUserViewModel eVM;
		private string _hPass;

		public EditUserDataPage()
		{
			InitializeComponent();

			eVM = new EditUserViewModel();

			BindingContext = eVM;

			eVM.FSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));



			bt_Guardar.Clicked += async (sender, e) =>
			{
				eVM.IsBusy = true;
				if (eVM.EdName != null && eVM.EdPass != null && eVM.EdPass2 != null)
				{
					if (eVM.EdPass == eVM.EdPass2)
					{

						_client = new AzureClient<Person>();

						_hPass = DependencyService.Get<IHash256>().Hash256(eVM.EdPass);

						var Datos = new Person()
						{
							Phone = eVM.EdPhone,
							Name = eVM.EdName,
							Pass = _hPass,
							Birthday = eVM.EdBday,
							IsAdmin = false
						};

						var Tabla = _client.GetTable();

						await Tabla.UpdateAsync(Datos);

						Settings.CurrentUser = Datos;

						DisplayAlert("Listo", "Tus datos han sidos cambiados exitosamente", "OK");
						var content = new MainPage();

						var _mainPage = new NavigationPage(content)
						{
							BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
							BarTextColor = Color.White,
							Title = "Fidus"
						};

						NavigationPage.SetHasNavigationBar(_mainPage, false);
						NavigationPage.SetHasBackButton(_mainPage, false);
						await Navigation.PushAsync(_mainPage);
					}
					else
					{
						DisplayAlert("Error", "Las contraseñas ingresadas no coiciden", "OK");
					}
				}
				else
				{
					DisplayAlert("Error", "Debes completar todos los campos", "OK");
				}
				eVM.IsBusy = false;
			};
		}
	}
}

