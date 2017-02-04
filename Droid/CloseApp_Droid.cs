using System;
using Android.App;
using fidus.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CloseAppImplementation_Droid))]

namespace fidus.Droid
{
	public class CloseAppImplementation_Droid: ICloseApplication
	{
		public CloseAppImplementation_Droid() {}

		public void closeApp()
		{
			var activity = (Activity)Forms.Context;
			if (activity!=null)
				activity.FinishAffinity();
		}
	}

}
