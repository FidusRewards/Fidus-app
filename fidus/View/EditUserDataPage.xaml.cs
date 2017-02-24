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

			bt_Guardar.FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, "");

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
							Email = Helpers.Settings.CurrentUser.Email,
							id = Helpers.Settings.CurrentUser.id,
							Version = Helpers.Settings.CurrentUser.Version,
							Phone = eVM.EdPhone,
							Name = eVM.EdName,
							Birthday = eVM.EdBday,
							IsAdmin = Helpers.Settings.CurrentUser.IsAdmin,
							Logged = true,
							LastLogin = System.DateTime.Now.ToLocalTime(),
							Pass = Helpers.Settings.CurrentUser.Pass,
							Points = Helpers.Settings.CurrentUser.Points
						};

						if (eVM.EdPass != null && eVM.EdPass != " ")
						{
							_hPass = DependencyService.Get<IHash256>().Hash256(eVM.EdPass);
							Datos.Pass = _hPass;
						}



						var Tabla = _client.GetPTable();

						await Tabla.UpdateAsync(Datos);

						Helpers.Settings.CurrentUser = Datos;

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

