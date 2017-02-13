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
		double FSize;
		public QualifyPage(string place, string puntos, string logo, History _history)
		{
			InitializeComponent();


			PImage.Source = logo;
			PImage.HeightRequest = 50;
			PImage.WidthRequest = 50;
			PName.Text = place;
			PName.TextColor = Color.FromHex(Settings.FidusColor);
			PName.VerticalOptions = LayoutOptions.Center;
			PName.FontAttributes = FontAttributes.Bold;
			PName.FontSize = 30;

			PPuntos.Text = "Has obtenido : " + puntos;
			PPuntos.TextColor = Color.FromHex(Settings.FidusColor);
			PPuntos.VerticalOptions = LayoutOptions.Start;
			PPuntos.FontAttributes = FontAttributes.Bold;
			PPuntos.FontSize = 15;

			PGracias.Text = "GRACIAS POR VENIR A " + place;
			PGracias.TextColor = Color.FromHex(Settings.FidusColor);
			PGracias.VerticalOptions = LayoutOptions.Center;
			PGracias.FontAttributes = FontAttributes.Bold;
			PGracias.FontSize = 50;

			ratingst.RatingSettings.RatedFill = Color.FromHex(Settings.FidusColor);
			ratingst.RatingSettings.UnRatedFill = Color.Transparent;

			FSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));

			bt_Avanzar.BackgroundColor = Color.FromHex(Settings.FidusBlue);

			bt_Avanzar.Clicked += async (sender, e) =>
			{
				var _itemsH = new LoadAsync<History>();
				//_history.Rating = ratingst.GetValue.
				await Navigation.PopToRootAsync();
				await _itemsH.Save(_history);


			};
		}

	}
}