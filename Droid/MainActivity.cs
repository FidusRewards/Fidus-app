using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;

namespace fidus.Droid
{
	[Activity(Label = "fidus", Icon = "@drawable/ic_launcher", MainLauncher= true, Theme = "@style/MyTheme", 
	          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			CrashManager.Register(this, Settings.Hockey_And);
			MetricsManager.Register(Application, Settings.Hockey_And);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			global::ZXing.Net.Mobile.Forms.Android.Platform.Init();
			global::Refractored.XamForms.PullToRefresh.Droid.PullToRefreshLayoutRenderer.Init();
			global::Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			LoadApplication(new App());
			//CheckForUpdates();
		}

		private void CheckForUpdates()
		{
			// Remove this for store builds!
			UpdateManager.Register(this, Settings.Hockey_And);
		}

		private void UnregisterManagers()
		{
			UpdateManager.Unregister();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
		protected override void OnPause()
		{
			base.OnPause();
			UnregisterManagers();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnregisterManagers();
		}

	}
}
