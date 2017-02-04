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
			_client = new AzureClient<Person>();

		}

		public async Task<bool> LoginQuery(string userEmail, string userPass)
		{
			IMobileServiceTable<Person> _tabla = _client.GetTable();
			string _hPass = DependencyService.Get<IHash256>().Hash256(userPass);

			try
			{
				IEnumerable<Person> result = await _tabla.Where(email => email.Email == userEmail && email.Pass == _hPass).ToEnumerableAsync();
				foreach (Person _person in result)
				{
					Settings.CurrentUser = _person;

					//Debug.WriteLine("foreachperson: " + _person.Name);

				};

				if (result.GetEnumerator().MoveNext())
				{

					Settings.Default.IsLogin = true;
					return true;
				}

				Settings.Default.IsLogin = false;
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
