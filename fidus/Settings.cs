using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace fidus
{
	public class Settings: Application
	{
		private static bool _isLogin = false, _isInetEnabled;

		public readonly static Settings Default = new Settings();

		public static bool IsLogin { get {return _isLogin; } set {_isLogin=value; } }
		public static bool IsReturn = false;	
		public bool IsInternetEnabled { get { return _isInetEnabled; } set {_isInetEnabled=value; } }

		public const string AzureUrl = "http://fidus.azurewebsites.net";

		public static Person CurrentUser;// = new Person();
		public Person User { get { return CurrentUser; } set { CurrentUser = value;} }
		private static History History;// = new History();
		public static History Hitem { get { return History; } set { History = value; } }
		private static ObservableCollection<Place> _allPlaces = new ObservableCollection<Place>();
		public static ObservableCollection<Place> AllPlaces { get { return _allPlaces; } set { _allPlaces = value;} }

		public const string FidusColor = "#9E3B37";
		public const string FidusBlue = "#01baef";
		public const string FidusIosFont = "Helvetica";
		public const string FidusAndFont = "sans-serif";
		public const string AppVersion = "Versión: 2.0.13";

		public const string Hockey_iOs = "bf5bd0e001fe4cf0928002a4dd273e66";
		public const string Hockey_And = "d21e6cb5b8214000a98e63313150813d";
		public const string ImgSrvProd = "http://fidusimgsrv.blob.core.windows.net/logos/";

		public Settings() {
			CurrentUser = new Person();
			History = new History();
		}
	}
}
