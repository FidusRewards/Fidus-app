using System;
using System.Threading;
using fidus.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(CloseAppImplementation_IOS))]

namespace fidus.iOS
{
	public class CloseAppImplementation_IOS : ICloseApplication
	{
		public CloseAppImplementation_IOS() {}

		public void closeApp()
		{
			Thread.CurrentThread.Abort();

		}
	}


}
