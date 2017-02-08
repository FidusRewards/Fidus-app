using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using fidus.Droid;
using fidus.Controls;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomImageCell), typeof(CustomImageCellRendererDroid))]
namespace fidus.Droid
{
	public class CustomImageCellRendererDroid : ImageCellRenderer
	{
		protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
		{
			LinearLayout cell = (LinearLayout)base.GetCellCore(item, convertView, parent, context);
			ImageView image = (ImageView)cell.GetChildAt(0);
			image.SetScaleType(ImageView.ScaleType.Center);
			return cell;
		}
	}
}