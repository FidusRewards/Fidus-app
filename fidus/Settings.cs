﻿using System;
using Xamarin.Forms;

namespace fidus
{
	public class Settings: Application
	{
		private bool _isLogin, _isInetEnabled;

		public readonly static Settings Default = new Settings();

		public bool IsLogin { get {return _isLogin; } set {_isLogin=value; } } 
			
		public bool IsInternetEnabled { get { return _isInetEnabled; } set {_isInetEnabled=value; } }

		public const string AzureUrl = "http://fidus.azurewebsites.net";

		public static Person CurrentUser = new Person();
		public Person User { get { return CurrentUser; } set { CurrentUser = value;} }
		public const string FidusColor = "#9E3B37";
		public const string AppVersion = "Versión: 2.0.01";

		public const string Hockey_iOs = "bf5bd0e001fe4cf0928002a4dd273e66";
		public const string Hockey_And = "d21e6cb5b8214000a98e63313150813d";
		public const string ImgSrvProd = "http://fidusimgsrv.blob.core.windows.net/logos/";
	}
}
