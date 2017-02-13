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

			bt_Guardar.FontFamily = Device.OnPlatform(Settings.FidusIosFont, Settings.FidusAndFont, "");

			bt_Guardar.Clicked += async (sender, e) =>
			{
				eVM.IsBusy = true;
				if (eVM.EdName != null)
				{
					if (eVM.EdPass == eVM.EdPass2)
					{

						_client = new AzureClient<Person>();

						var Datos = new Person()
						{
							Email = Settings.CurrentUser.Email,
							id = Settings.CurrentUser.id,
							Version = Settings.CurrentUser.Version,
							Phone = eVM.EdPhone,
							Name = eVM.EdName,
							Birthday = eVM.EdBday,
							IsAdmin = Settings.CurrentUser.IsAdmin,
							Logged = true
						};

						if (eVM.EdPass != null && eVM.EdPass != " ")
						{
							_hPass = DependencyService.Get<IHash256>().Hash256(eVM.EdPass);
							Datos.Pass = _hPass;
						}



						var Tabla = _client.GetPTable();

						await Tabla.UpdateAsync(Datos);

						Settings.CurrentUser = Datos;
						App.UpdateProperties();

						await DisplayAlert("Listo", "Tus datos han sidos cambiados exitosamente", "OK");
						await Navigation.PopToRootAsync();
					}
					else
					{
						await DisplayAlert("Error", "Las contraseñas ingresadas no coinciden", "OK");
					}
				}
				else
				{
					await DisplayAlert("Error", "Por favor completá tu nombre", "OK");
				}
				eVM.IsBusy = false;
			};
		}
	}
}

