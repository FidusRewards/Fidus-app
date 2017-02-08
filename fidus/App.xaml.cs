using Xamarin.Forms;

namespace fidus
{
	public partial class App : Application
	{
		public static App instance;

		public App()
		{
			InitializeComponent();
			var content = new loginPage();
			instance = this;

			if (Device.OS == TargetPlatform.Android)
			{
				MainPage = new SplashScreen();
			}
			else
			{

				MainPage =	new NavigationPage(content)
				{
					BarBackgroundColor = Color.FromHex(Settings.FidusColor), //A13B35
					BarTextColor = Color.White
					//Title="Fidus"
				};
			}
				//MainPage = new loginPage();
		}


		protected override void OnStart()
		{
			// Handle when your app starts

		
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
