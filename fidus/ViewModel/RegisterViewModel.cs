﻿using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using HockeyApp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace fidus
{
	public class RegisterViewModel: BaseViewModel
	{
		private AzureClient<Person> _clientP = new AzureClient<Person>();
		private IMobileServiceTable<Person> _tablaP;
		private string _email;
		private string _pass;
		private string _passconfirm;
        private string _name;
		private double _size;
		private DateTime _bday = DateTime.Today;
		public Command RegButtonCommand { get; set; }
		private string _hPass;

		public string REmail { 
			get { return _email;} 
			set { _email = value; OnPropertyChanged();}}

		public string RName
		{
			get { return _name; }
			set { _name = value; OnPropertyChanged(); }
		}
		public string RPass {
			get { return _pass; }
			set { _pass = value; OnPropertyChanged(); }
		}
        public string RPass2
        {
            get { return _passconfirm; }
            set { _passconfirm = value; OnPropertyChanged(); }
        }
        public DateTime RBday
		{
			get { return _bday; }
			set { _bday = value; OnPropertyChanged(); }
		}

		public double FSize
		{
			get { return _size; }
			set { _size = value; OnPropertyChanged(); }
		}

		public RegisterViewModel()
		{
			RegButtonCommand = new Command(RegisterCmd);
			_tablaP = _clientP.GetPTable();
		}

		public async void RegisterCmd()
		{
			IsBusy = true;
			if (REmail != null && RName != null && RPass != null && RPass2 != null)
			{
                if (RPass == RPass2)
                {
                    _hPass = DependencyService.Get<IHash256>().Hash256(RPass);

					var Datos = new Person()
					{
						Email = REmail,
						Name = RName,
						Pass = _hPass,
						Birthday = RBday,
						IsAdmin = false,
						Logged = true,
						Phone = " ",
						Points = 0,
						LastLogin = DateTime.Now.ToLocalTime()
						                    
                    };


					await _tablaP.InsertAsync(Datos);

					//IEnumerable<Person> result = await Tabla.Where(person => person.Email == REmail).ToEnumerableAsync();
					//if (!Utils.IsAny(result))
					//{
					//	Datos.id = result.FirstOrDefault().id;
					//	Datos.Version = result.FirstOrDefault().Version;
					//}else{ 
						//MessagingCenter.Send(this, "NotValid");
						//IsBusy = false;
						//return;
					//}

                    Helpers.Settings.CurrentUser = Datos;
                    MetricsManager.TrackEvent("New User registered", new Dictionary<string, string>
                        {
                            {"User", Datos.Email},
                            {"Name", Datos.Name}
                        },
                        new Dictionary<string, double> { });

					Helpers.Settings.IsReturn = false;
					MessagingCenter.Send(this, "VOLVERMAIN");

					//MessagingCenter.Send(this, "Registered");
                }
                else {
                    MessagingCenter.Send(this, "PassNotSame");
                }
            }
			else {
				MessagingCenter.Send(this, "NotValid");
			};
			IsBusy = false;
		}

	}
}
