using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Foundation;
using UIKit;
using HockeyApp.iOS;

namespace fidus.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{

			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure(Settings.Hockey_iOs);
			manager.StartManager();
			manager.Authenticator.AuthenticateInstallation();

			global::Xamarin.Forms.Forms.Init();
			global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();
			global::Refractored.XamForms.PullToRefresh.iOS.PullToRefreshLayoutRenderer.Init();
			global::Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			SlideOverKit.iOS.SlideOverKit.Init();
			            
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
