using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.SfRating.XForms;


using Xamarin.Forms;

namespace fidus
{
	public partial class QualifyPage : ContentPage
	{
		public QualifyPage(string place="Casa", string puntos="50", string logo=Settings.ImgSrvProd+"starbucks_logo.png", History _history=null)
		{
			InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";

			PImage.Source = logo;
			PImage.HeightRequest = 62;
			PImage.WidthRequest = 62;
			PName.Text = place;
			PName.TextColor = Color.Black;
			PName.VerticalOptions = LayoutOptions.Center;
			PName.FontAttributes = FontAttributes.Bold;
			PName.FontSize = 18;
			PName.HorizontalOptions = LayoutOptions.StartAndExpand;

			PPuntos.Text = "Suamaste " + puntos + " puntos";
			PPuntos.TextColor = Color.FromHex(Settings.FidusColor);
			PPuntos.VerticalOptions = LayoutOptions.Center;
			PPuntos.FontFamily = Device.OnPlatform(Settings.FidusIosFont, Settings.FidusAndFont, "");
			PPuntos.FontAttributes = FontAttributes.Bold;
			PPuntos.FontSize = 15;
			PPuntos.Margin = new Thickness(0, 5, 0, 5);

			PGracias.Source = "graciasporvenir.png";
			PGracias.BackgroundColor = Color.Transparent;
			PGracias.VerticalOptions = LayoutOptions.Center;
			PGracias.HorizontalOptions = LayoutOptions.Center;
			PGracias.HeightRequest = 100;


			ratingst.RatingSettings.RatedFill = Color.FromHex(Settings.FidusColor);
			ratingst.RatingSettings.UnRatedFill = Color.Silver;
			ratingst.ItemSize = 40;
			ratingst.RatingSettings.RatedStroke = Color.Transparent;
			ratingst.RatingSettings.UnRatedStroke = Color.Transparent;

			//bt_Avanzar.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			bt_Avanzar.Image = "btn_continuargris.png";
			//bt_Avanzar.BackgroundColor = Color.FromHex(Settings.FidusBlue);

			bt_Avanzar.Clicked += async (sender, e) =>
			{
				var _itemsH = new LoadAsync<History>();
				_history.Rating = Convert.ToInt32(ratingst.Value);
				await Navigation.PopToRootAsync();
				await _itemsH.Save(_history);


			};
		}

	}
}