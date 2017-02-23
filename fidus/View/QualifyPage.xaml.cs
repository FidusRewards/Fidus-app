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

		public QualifyPage(string place="Fidus", string puntos="0", string logo=Helpers.Settings.ImgSrvProd+"logofidus.png", string category="Resto Bar", History _history=null)
		{
			InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";


			PImage.Source = logo;
			PImage.HeightRequest = 62;
			PImage.WidthRequest = 62;
			PImage.VerticalOptions = LayoutOptions.Center;

			PName.Text = place;
			PName.TextColor = Color.Black;
			PName.FontAttributes = FontAttributes.Bold;
			PName.FontSize = 18;
			PName.HorizontalOptions = LayoutOptions.StartAndExpand;
			PCat.Text = category;

			PPuntos.Text = "Sumaste " + puntos + " puntos";
			PPuntos.TextColor = Color.FromHex(Helpers.Settings.FidusColor);
			PPuntos.VerticalOptions = LayoutOptions.Center;
			PPuntos.FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, "");
			PPuntos.FontAttributes = FontAttributes.Bold;
			PPuntos.FontSize = 20;
			PPuntos.Margin = new Thickness(0, 5, 0, 5);

			PGracias.Source = "graciasporvenir.png";
			PGracias.BackgroundColor = Color.Transparent;
			PGracias.VerticalOptions = LayoutOptions.Center;
			PGracias.HorizontalOptions = LayoutOptions.Center;
			PGracias.HeightRequest = 100;

			ratingst.RatingSettings.RatedFill = Color.FromHex(Helpers.Settings.FidusColor);
			ratingst.RatingSettings.UnRatedFill = Color.Silver;
			ratingst.ItemSize = 40;
			ratingst.RatingSettings.RatedStroke = Color.Transparent;
			ratingst.RatingSettings.UnRatedStroke = Color.Transparent;
			//qVM.NRating = Convert.ToString(ratingst.Value);

			//bt_Avanzar.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			bt_Avanzar.Image = "btn_continuargris.png";
			bt_Avanzar.BackgroundColor = Color.FromHex("#646464");
			bt_Avanzar.BorderColor = Color.FromHex("#646464");


			//bt_Avanzar.BackgroundColor = Color.FromHex(Settings.FidusBlue);

			bt_Avanzar.Clicked += async (sender, e) =>
			{
				Helpers.Settings.IsReturn = false;
				var _itemsH = new LoadAsync<History>();
				_history.Rating = Convert.ToInt32(ratingst.Value);
				await Navigation.PopToRootAsync();
				await _itemsH.Save(_history);


			};
		}

	}
}