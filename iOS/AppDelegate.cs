﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Foundation;
using UIKit;
using HockeyApp.iOS;
using Syncfusion.SfRating.XForms.iOS;
using Syncfusion.SfNavigationDrawer.XForms.iOS;

namespace fidus.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure(Helpers.Settings.Hockey_iOs);
			manager.StartManager();
			manager.Authenticator.AuthenticateInstallation();
			manager.DisableUpdateManager = true;
			global::Xamarin.Forms.Forms.Init();
			
			global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();
			global::Refractored.XamForms.PullToRefresh.iOS.PullToRefreshLayoutRenderer.Init();
			global::Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            //SQLitePCL.CurrentPlatform.Init();
            //SlideOverKit.iOS.SlideOverKit.Init();
		 	

			LoadApplication(new App());
			new SfRatingRenderer();
			new SfNavigationDrawerRenderer();

			return base.FinishedLaunching(app, options);
		}

	}
}
