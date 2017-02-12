using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fidus
{
	public class EditUserViewModel : BaseViewModel
	{
		private string _phone;
		private string _pass;
		private string _passconfirm;
		private string _name;
		private string _email;
		private double _size;
		private DateTime _bday = DateTime.Today;


		public EditUserViewModel()
		{
			EdPhone = Settings.CurrentUser.Phone;
			EdName = Settings.CurrentUser.Name;
			EdBday = Settings.CurrentUser.Birthday;
			EdEmail = Settings.CurrentUser.Email;

		}

		public string EdEmail
		{
			get { return _email; }
			set { _email = value; OnPropertyChanged(); }
		}

		public string EdPhone
		{
			get { return _phone; }
			set { _phone = value; OnPropertyChanged(); }
		}

		public string EdName
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged(); }
		}
		public string EdPass
		{
			get { return _pass; }
			set { _pass = value; OnPropertyChanged(); }
		}
		public string EdPass2
		{
			get { return _passconfirm; }
			set { _passconfirm = value; OnPropertyChanged(); }
		}
		public DateTime EdBday
		{
			get { return _bday; }
			set { _bday = value; OnPropertyChanged(); }
		}

		public double FSize
		{
			get { return _size; }
			set { _size = value; OnPropertyChanged(); }
		}

		/*public async void EditUserCmd()
        {
            IsBusy = true;
            if (EdPhone != null && EdName != null && EdPass != null && EdPass2 != null)
            {
                if (EdPass == EdPass2)
                {

                    _client = new AzureClient<Person>();

                    _hPass = DependencyService.Get<IHash256>().Hash256(EdPass);

                    var Datos = new Person()
                    {
                        Phone = EdPhone,
                        Name = EdName,
                        Pass = _hPass,
                        Birthday = EdBday,
                        IsAdmin = false
                    };

                    var Tabla = _client.GetTable();
                    
                    await Tabla.UpdateAsync(Datos);

                    Settings.CurrentUser = Datos;

                    MessagingCenter.Send(this, "Edited");
                }
                else
                {
                    MessagingCenter.Send(this, "PassNotSame");
                }
            }
            else
            {
                MessagingCenter.Send(this, "NotValid");
            };
            IsBusy = false;
        }*/
	}
}
