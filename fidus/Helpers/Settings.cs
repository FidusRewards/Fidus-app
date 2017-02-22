// Helpers/Settings.cs
using System;
using System.Collections.ObjectModel;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace fidus.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }

    #region Setting Constants

		private const string UEmail = "UserEmail";
		private static readonly string uEmail = "fidus@com";
		private const string UPass = "UserPass";
		private static readonly string uPass = "333";
		private const string UPhone = "UserPhone";
		private static readonly string uPhone = "555-5555";
		private const string UName = "UserName";
		private static readonly string uName = "fidus";
		private const string UBirth = "UserBirthday";
		private static readonly DateTime uBirth = DateTime.Now.Date;
		private const string Uid = "UserId";
		private static readonly string uID = "AA";
		private const string UVer = "UserVersion";
		private static readonly string uVer = "AA";
		private const string UPoints = "UserPoints";
		private static int uPoints = 0;
		private const string UIsAdmin = "UserIsAdmin";
		private static readonly bool uIsAdmin = false;
		private const string UIsLogged = "UserIsLogged";
		private static readonly bool uIsLogged = false;
		private const string ULastLogin = "UserLastLogin";
		private static readonly DateTime uLastLogin = System.DateTime.Now.ToLocalTime();
	#endregion
	#region Fidus Settings
		private static bool _isLogin = false, _isInetEnabled=true;

		public static bool IsLogin { get { return _isLogin; } set { _isLogin = value; } }
		public static bool IsReturn = true;
		public static bool IsBoot = true;
		public static bool IsInternetEnabled { get { return _isInetEnabled; } set { _isInetEnabled = value; } }

		public const string AzureUrl = "http://fidus.azurewebsites.net";

		private static Person currentuser = new Person();
		private static History History = new History();
		public static History Hitem { get { return History; } set { History = value; } }
		private static ObservableCollection<Place> _allPlaces = new ObservableCollection<Place>();
		public static ObservableCollection<Place> AllPlaces { get { return _allPlaces; } set { _allPlaces = value; } }
		public static int Points;

		public const string FidusColor = "#9E3B37";
		public const string FidusBlue = "#01baef";
		public const string FidusIosFont = "Helvetica";
		public const string FidusAndFont = "sans-serif";
		public const string AppVersion = "Versión: 2.0.15";

		public const string Hockey_iOs = "bf5bd0e001fe4cf0928002a4dd273e66";
		public const string Hockey_And = "d21e6cb5b8214000a98e63313150813d";
		public const string ImgSrvProd = "http://fidusimgsrv.blob.core.windows.net/logos/";
  	#endregion

	public static Person CurrentUser
		{ 
			get {
				currentuser.Email = UserEmail;
				currentuser.Name = UserName;
				currentuser.Birthday = UserBirth;
				currentuser.id = UserID;
				currentuser.IsAdmin = UserIsAdmin;
				currentuser.LastLogin = UserLastLogin;
				currentuser.Logged = UserLogged;
				currentuser.Pass = UserPass;
				currentuser.Phone = UserPhone;
				currentuser.Points = UserPoints;
				currentuser.Version = UserVersion;
				return currentuser;
				}

			set {
				UserEmail=value.Email;
				UserName=value.Name;
				UserPass= value.Pass;
				UserID= value.id;
				UserVersion= value.Version;
				UserPhone= value.Phone;
				UserBirth= value.Birthday;
				UserLastLogin= value.LastLogin;
				UserIsAdmin= value.IsAdmin;
				UserLogged= value.Logged;
				UserPoints= value.Points;
			}
		
		}
	public static string UserEmail
		{
			get{ return AppSettings.GetValueOrDefault<string>(UEmail, uEmail);}
			set{ AppSettings.AddOrUpdateValue<string>(UEmail, value);}
    	}
	
	public static string UserName
		{
			get { return AppSettings.GetValueOrDefault<string>(UName, uName); }
			set { AppSettings.AddOrUpdateValue<string>(UName, value); }
		}
	public static string UserPass
		{
			get { return AppSettings.GetValueOrDefault<string>(UPass, uPass); }
			set { AppSettings.AddOrUpdateValue<string>(UPass, value); }
		}

	public static string UserPhone
		{
			get { return AppSettings.GetValueOrDefault<string>(UPhone, uPhone); }
			set { AppSettings.AddOrUpdateValue<string>(UPhone, value); }
		}

	public static string UserID
		{
			get { return AppSettings.GetValueOrDefault<string>(Uid, uID); }
			set { AppSettings.AddOrUpdateValue<string>(Uid, value); }
		}

	public static string UserVersion
		{
			get { return AppSettings.GetValueOrDefault<string>(UVer, uVer); }
			set { AppSettings.AddOrUpdateValue<string>(UVer, value); }
		}

	public static DateTime UserBirth
		{
			get { return AppSettings.GetValueOrDefault<DateTime>(UBirth, uBirth); }
			set { AppSettings.AddOrUpdateValue<DateTime>(UBirth, value); }
		}

	public static DateTime UserLastLogin
		{
			get { return AppSettings.GetValueOrDefault<DateTime>(ULastLogin, uLastLogin); }
			set { AppSettings.AddOrUpdateValue<DateTime>(ULastLogin, value); }
		}
	public static int UserPoints
		{
			get { return AppSettings.GetValueOrDefault<int>(UPoints, uPoints); }
			set { AppSettings.AddOrUpdateValue<int>(UPoints, value); }
		}
	public static bool UserIsAdmin
		{
			get { return AppSettings.GetValueOrDefault<bool>(UIsAdmin, uIsAdmin); }
			set { AppSettings.AddOrUpdateValue<bool>(UIsAdmin, value); }
		}
	public static bool UserLogged
		{
			get { return AppSettings.GetValueOrDefault<bool>(UIsLogged, uIsLogged); }
			set { AppSettings.AddOrUpdateValue<bool>(UIsLogged, value); }
		}
	}
}