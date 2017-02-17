using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
			//_client = new AzureClient<Person>();

		}

		public async Task<bool> LoginQuery(string userEmail, string userPass)
		{
			//await _client.PurgeData();
			Settings.CurrentUser.Name = userEmail;
			//await _client.SyncAsync();

			IMobileServiceTable<Person> _tabla = _client.GetPTable();
			string _hPass = DependencyService.Get<IHash256>().Hash256(userPass);

			try
			{
				IEnumerable<Person> result = await _tabla.Where(email => email.Email == userEmail && email.Pass == _hPass).ToEnumerableAsync();
				foreach (Person _person in result)
				{
					Settings.CurrentUser = _person;
					if (_person.Phone == null)
						Settings.CurrentUser.Phone = " ";
					Debug.WriteLine("foreachperson: " + _person.Name);

				};

				if (result.GetEnumerator().MoveNext())
				{

					Settings.CurrentUser.Logged = true;
					Settings.CurrentUser.LastLogin = System.DateTime.Now;
					App.UpdateProperties();
					await _tabla.UpdateAsync(Settings.CurrentUser);
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
