using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace fidus
{
	public class loginViewModel: BaseViewModel
	{
		private AzureClient<Person> _client;
		public ObservableCollection<Person> Items { get; set; }
        public bool BusyHide = true;

		public loginViewModel()
		{
			_client = AzureClient<Person>.defaultInstance; // new AzureClient<Person>();

		}

		public async Task<bool> LoginQuery(string userEmail, string userPass)
		{
			//await _client.PurgeData();
			Helpers.Settings.UserName = userEmail;
			//await _client.SyncAsync();

			IMobileServiceTable<Person> _tabla = _client.GetPTable();
			string _hPass = DependencyService.Get<IHash256>().Hash256(userPass);

			try
			{

				IEnumerable<Person> result = await _tabla.Where(email => email.Email == userEmail && email.Pass == _hPass).ToEnumerableAsync();

				Person _person = result.FirstOrDefault();

				if (_person !=null)
				{
					Helpers.Settings.CurrentUser = _person;
					if (_person.Phone == null)
						Helpers.Settings.UserPhone = " ";
					Debug.WriteLine("foreachperson: " + _person.Name);

					Helpers.Settings.CurrentUser.Logged = true;
					Helpers.Settings.CurrentUser.LastLogin = System.DateTime.Now.ToLocalTime();
				
					await _tabla.UpdateAsync(Helpers.Settings.CurrentUser);
					//await _tabla.UpdateAsync(Settings.CurrentUser);
					return true;
				}

				return false;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("LoginVM: Error en Conexíon usr-pass"+ex);
				MessagingCenter.Send(this, "NOINET");
				return false;
			}
			
		}
	}
}
